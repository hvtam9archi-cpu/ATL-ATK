using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using ATL_ATK.Helpers;
using ATL_ATK.Models;
using ATL_ATK.Services;
using ATL_ATK.UI;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ATL_ATK.Commands
{
    /// <summary>
    /// Lệnh ATK - Thống kê chi tiết Block Att (có thiết lập đầy đủ).
    /// Tương đương C:atk trong LISP.
    /// </summary>
    public class AtkCommand
    {
        [CommandMethod("ATK")]
        public void Execute()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();
                AtkSettings settings = AtlCommand.GlobalSettings;

                // Bước 1: Mở hộp thoại Options (chọn, sắp xếp)
                var optionsWindow = new AtkOptionsWindow(settings);
                Application.ShowModalWindow(optionsWindow);

                if (!optionsWindow.IsConfirmed)
                    return;

                // Bước 2: Kiểm tra Block đã khai báo
                if (settings.SelectOneBlock && string.IsNullOrEmpty(settings.BlockName))
                {
                    editor.WriteMessage("\nChưa khai báo Block Att! Dùng lệnh ATL để thiết lập.");
                    return;
                }

                // Bước 3: Chọn Block
                List<BlockData> blockDataList = SelectBlocks(editor, database, settings);

                if (blockDataList == null || blockDataList.Count == 0)
                {
                    editor.WriteMessage("\nKhông chọn được Block nào.");
                    return;
                }

                // Bước 4: Lọc theo 1 Block (nếu cần)
                if (settings.SelectOneBlock && !string.IsNullOrEmpty(settings.BlockName))
                {
                    blockDataList = blockDataList.Where(b =>
                        string.Equals(b.BlockName, settings.BlockName, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (blockDataList.Count == 0)
                    {
                        editor.WriteMessage($"\nKhông tìm thấy Block \"{settings.BlockName}\" trong selection.");
                        return;
                    }
                }

                // Bước 5: Sắp xếp theo tọa độ (nếu bật)
                if (settings.SortByPosition)
                {
                    blockDataList = SortingService.SortByPosition(
                        blockDataList,
                        settings.SortDirection1,
                        settings.SortDirection2,
                        settings.SortFuzz);
                }

                // Bước 6: Chọn điểm chèn bảng
                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK)
                    return;

                Point3d insertionPoint = pointResult.Value;

                // Bước 7: Xây dựng dữ liệu bảng
                int precision = Convert.ToInt32(Application.GetSystemVariable("LUPREC"));
                List<List<string>> tableRows = BuildTableData(blockDataList, settings, precision);

                // Bước 8: Sắp xếp theo cột (nếu bật)
                if (settings.SortByColumn && tableRows.Count > 0)
                {
                    tableRows = SortingService.SortByColumnValue(tableRows, settings.SortColumnIndex);
                }

                // Bước 9: Thêm hàng tổng (nếu bật)
                if (settings.ShowSumRow)
                {
                    tableRows.Add(BuildSumRow(tableRows, settings.TotalColumns));
                }

                // Bước 10: Thay thế %%STT bằng số thứ tự thực
                ReplaceSequenceNumbers(tableRows);

                // Bước 11: Tạo dữ liệu hoàn chỉnh (title + header + data)
                var fullTableData = new List<List<string>>();
                fullTableData.Add(new List<string> { settings.Title }); // Title row

                // Header row
                var headerRow = new List<string>();
                for (int i = 0; i < settings.TotalColumns; i++)
                {
                    headerRow.Add(settings.Columns[i].Title);
                }
                fullTableData.Add(headerRow);

                fullTableData.AddRange(tableRows);

                // Bước 12: Tạo bảng trong AutoCAD
                string tableStyleName = settings.TableStyleName ?? "";
                List<string> tableStyles = BlockQueryService.GetTableStyleNames(database);
                if (!tableStyles.Contains(tableStyleName))
                    tableStyleName = Application.GetSystemVariable("CTABLESTYLE")?.ToString() ?? "Standard";

                string textStyleName = settings.TextStyleName ?? "";
                List<string> textStyles = BlockQueryService.GetTextStyleNames(database);
                if (!textStyles.Contains(textStyleName))
                    textStyleName = Application.GetSystemVariable("TEXTSTYLE")?.ToString() ?? "Standard";

                string layerName = Application.GetSystemVariable("CLAYER")?.ToString() ?? "0";

                using (DocumentLock docLock = document.LockDocument())
                {
                    if (settings.AutoColumnWidth)
                    {
                        TableGeneratorService.CreateTableAutoWidth(
                            database, fullTableData,
                            settings.TextHeight,
                            settings.TextHeight * 3.0,
                            null,
                            insertionPoint,
                            tableStyleName,
                            textStyleName,
                            layerName);
                    }
                    else
                    {
                        var columnWidths = new Dictionary<int, double>();
                        for (int i = 0; i < settings.TotalColumns; i++)
                        {
                            columnWidths[i] = settings.Columns[i].Width;
                        }

                        var rowHeights = new Dictionary<int, double>
                        {
                            { 0, settings.RowHeight }
                        };

                        TableGeneratorService.CreateTableManualWidth(
                            database, fullTableData,
                            rowHeights, columnWidths,
                            settings.TextHeight,
                            null,
                            insertionPoint,
                            tableStyleName,
                            textStyleName,
                            layerName);
                    }
                }

                editor.WriteMessage($"\nĐã tạo bảng thống kê: {blockDataList.Count} block, {settings.TotalColumns} cột.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh ATK: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  PRIVATE METHODS
        // -------------------------------------------------------

        /// <summary>Chọn Block theo thiết lập</summary>
        private List<BlockData> SelectBlocks(Editor editor, Database database, AtkSettings settings)
        {
            if (settings.SelectByLayout || settings.SelectAll)
            {
                // Chọn từ Layout
                var allBlocks = new List<BlockData>();
                List<string> layouts = BlockQueryService.GetLayoutNames(database);

                if (settings.SelectAll)
                    layouts.Insert(0, "Model");

                foreach (string layout in layouts)
                {
                    List<BlockData> layoutBlocks = BlockQueryService.GetBlocksInLayout(database, layout);

                    if (settings.SortByPosition)
                    {
                        layoutBlocks = SortingService.SortByPosition(
                            layoutBlocks,
                            settings.SortDirection1,
                            settings.SortDirection2,
                            settings.SortFuzz);
                    }

                    allBlocks.AddRange(layoutBlocks);
                }

                return allBlocks;
            }
            else
            {
                // Chọn thủ công
                return BlockQueryService.SelectAndExtractBlocks(editor, database);
            }
        }

        /// <summary>Xây dựng dữ liệu bảng từ danh sách Block</summary>
        private List<List<string>> BuildTableData(
            List<BlockData> blockDataList,
            AtkSettings settings,
            int precision)
        {
            const string STT_PLACEHOLDER = "!@#$%^&*(STT)*&^%$#@!";
            var tableRows = new List<List<string>>();

            foreach (BlockData blockData in blockDataList)
            {
                var row = new List<string>();

                for (int colIndex = 0; colIndex < settings.TotalColumns; colIndex++)
                {
                    string expression = settings.Columns[colIndex].TagExpression;
                    List<TagToken> tokens = TagParserService.ParseExpression(expression);

                    string cellValue = TagParserService.EvaluateExpression(
                        tokens, blockData, 0, precision);

                    // Thay %%STT bằng placeholder (sẽ thay sau khi sắp xếp)
                    if (expression.Trim().Equals("%%STT", StringComparison.OrdinalIgnoreCase))
                    {
                        cellValue = STT_PLACEHOLDER;
                    }

                    row.Add(cellValue);
                }

                tableRows.Add(row);
            }

            return tableRows;
        }

        /// <summary>Xây dựng hàng tổng</summary>
        private List<string> BuildSumRow(List<List<string>> tableRows, int totalColumns)
        {
            var sumRow = new List<string>();

            for (int colIndex = 0; colIndex < totalColumns; colIndex++)
            {
                double sum = 0;
                bool hasNumber = false;

                foreach (var row in tableRows)
                {
                    if (colIndex < row.Count)
                    {
                        if (double.TryParse(row[colIndex], out double value))
                        {
                            sum += value;
                            hasNumber = true;
                        }
                    }
                }

                if (hasNumber)
                {
                    sumRow.Add(StringHelper.FormatNumber(sum));
                }
                else
                {
                    sumRow.Add(colIndex == 0 ? "TỔNG" : "");
                }
            }

            return sumRow;
        }

        /// <summary>Thay thế placeholder STT bằng số thứ tự thực</summary>
        private void ReplaceSequenceNumbers(List<List<string>> tableRows)
        {
            const string STT_PLACEHOLDER = "!@#$%^&*(STT)*&^%$#@!";

            // Bỏ qua hàng tổng (nếu có)
            int dataRowCount = tableRows.Count;

            // Kiểm tra hàng cuối có phải hàng tổng không
            if (dataRowCount > 0)
            {
                var lastRow = tableRows[dataRowCount - 1];
                if (lastRow.Count > 0 && lastRow[0] == "TỔNG")
                    dataRowCount--;
            }

            for (int i = 0; i < dataRowCount; i++)
            {
                for (int j = 0; j < tableRows[i].Count; j++)
                {
                    tableRows[i][j] = StringHelper.SubstituteString(
                        tableRows[i][j], STT_PLACEHOLDER, (i + 1).ToString());
                }
            }
        }
    }
}
