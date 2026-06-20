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
                List<BlockData> blockDataList = AtkLogic.SelectBlocks(editor, database, settings);

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

                // Bước 6: Chọn điểm chèn bảng (Bỏ qua nếu đang là chế độ cập nhật)
                Point3d insertionPoint = Point3d.Origin;
                if (!optionsWindow.IsUpdateMode)
                {
                    PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                    if (pointResult.Status != PromptStatus.OK)
                        return;

                    insertionPoint = pointResult.Value;
                }

                // Bước 7: Xây dựng dữ liệu bảng
                int precision = Convert.ToInt32(Application.GetSystemVariable("LUPREC"));
                List<List<string>> tableRows = AtkLogic.BuildTableData(blockDataList, settings, precision);

                // Bước 8: Sắp xếp theo cột (nếu bật)
                if (settings.SortByColumn && tableRows.Count > 0)
                {
                    tableRows = SortingService.SortByColumnValue(tableRows, settings.SortColumnIndex);
                }

                // Bước 9: Thêm hàng tổng (nếu bật)
                if (settings.ShowSumRow)
                {
                    tableRows.Add(AtkLogic.BuildSumRow(tableRows, settings.TotalColumns));
                }

                // Bước 10: Thay thế %%STT bằng số thứ tự thực
                AtkLogic.ReplaceSequenceNumbers(tableRows);

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
                    if (optionsWindow.IsUpdateMode)
                    {
                        TableUpdateService.UpdateAllAtkTablesWithData(document, fullTableData);
                        editor.WriteMessage($"\nĐã cập nhật bảng thống kê: {blockDataList.Count} block, {settings.TotalColumns} cột.");
                    }
                    else
                    {
                        if (settings.AutoColumnWidth)
                        {
                            ObjectId tableId = TableGeneratorService.CreateTableAutoWidth(
                                database, fullTableData,
                                settings.TextHeight,
                                settings.TextHeight * 3.0,
                                null,
                                insertionPoint,
                                tableStyleName,
                                textStyleName,
                                layerName);
                            TableGeneratorService.AttachAtkTableXData(database, tableId, settings.BlockName);
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

                            ObjectId tableId = TableGeneratorService.CreateTableManualWidth(
                                database, fullTableData,
                                rowHeights, columnWidths,
                                settings.TextHeight,
                                null,
                                insertionPoint,
                                tableStyleName,
                                textStyleName,
                                layerName);
                            TableGeneratorService.AttachAtkTableXData(database, tableId, settings.BlockName);
                        }
                    }
                }

                if (!optionsWindow.IsUpdateMode)
                {
                    editor.WriteMessage($"\nĐã tạo bảng thống kê: {blockDataList.Count} block, {settings.TotalColumns} cột.");
                }
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh ATK: {ex.Message}");
            }
        }
    }
}
