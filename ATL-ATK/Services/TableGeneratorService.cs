using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using ATL_ATK.Helpers;

namespace ATL_ATK.Services
{
    /// <summary>
    /// Service tạo AutoCAD Table từ dữ liệu danh sách.
    /// Tương đương ND:list->table và ND:list->table-AC trong LISP.
    /// </summary>
    public static class TableGeneratorService
    {
        /// <summary>
        /// Tạo AutoCAD Table với cột tự động (Auto Column Width).
        /// Tương đương ND:list->table-AC trong LISP.
        /// 
        /// tableData: danh sách hàng (hàng 0 = title, hàng 1 = header, còn lại = data)
        /// textHeight: cao chữ
        /// rowHeight: cao hàng
        /// mergeCells: danh sách merge (mỗi item = {topRow, bottomRow, leftCol, rightCol})
        /// insertionPoint: điểm chèn
        /// tableStyleName: tên Table Style
        /// textStyleName: tên Text Style
        /// layerName: tên Layer
        /// </summary>
        public static ObjectId CreateTableAutoWidth(
            Database database,
            List<List<string>> tableData,
            double textHeight,
            double rowHeight,
            List<int[]> mergeCells,
            Point3d insertionPoint,
            string tableStyleName,
            string textStyleName,
            string layerName)
        {
            int totalColumns = tableData.Max(row => row.Count);
            int totalRows = tableData.Count;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                // Lấy BlockTableRecord (ModelSpace hoặc PaperSpace)
                BlockTableRecord currentSpace = GetCurrentSpace(database, transaction);

                // Tạo Table
                Table table = new Table();
                table.TableStyle = GetTableStyleId(database, transaction, tableStyleName);
                table.Position = insertionPoint;
                table.SetDatabaseDefaults();

                // Thiết lập số hàng cột
                table.SetSize(totalRows, totalColumns);

                // Thiết lập Layer
                table.Layer = layerName;

                // Tắt regenerate để tăng tốc
                table.SuppressRegenerateTable(true);

                // Thiết lập margins
                table.Cells.Borders.Horizontal.Margin = 0;
                table.Cells.Borders.Vertical.Margin = 0;

                // Track column widths (lấy max width mỗi cột)
                double[] columnWidths = new double[totalColumns];

                for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
                {
                    List<string> row = tableData[rowIndex];

                    for (int colIndex = 0; colIndex < totalColumns; colIndex++)
                    {
                        Cell cell = table.Cells[rowIndex, colIndex];

                        // Set text style
                        ObjectId textStyleId = GetTextStyleId(database, transaction, textStyleName);
                        if (!textStyleId.IsNull)
                        {
                            cell.TextStyleId = textStyleId;
                        }

                        // Set alignment
                        cell.Alignment = CellAlignment.MiddleCenter;

                        // Set text height
                        cell.TextHeight = textHeight;

                        // Set cell content
                        string cellValue = colIndex < row.Count ? row[colIndex] : "";

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            if (cellValue.StartsWith("%%BLK"))
                            {
                                // Chèn Block vào cell
                                string blockName = cellValue.Substring(5);
                                ObjectId blockId = GetBlockId(database, transaction, blockName);

                                if (!blockId.IsNull)
                                {
                                    cell.BlockTableRecordId = blockId;
                                    columnWidths[colIndex] = Math.Max(columnWidths[colIndex], rowHeight * 2.0);
                                }
                            }
                            else
                            {
                                cell.SetValue(cellValue, ParseOption.ParseOptionNone);

                                // Tính chiều rộng cần thiết cho text
                                double estimatedWidth = cellValue.Length * textHeight * 0.7 + textHeight * 4.0;
                                columnWidths[colIndex] = Math.Max(columnWidths[colIndex], estimatedWidth);
                            }
                        }
                    }

                    // Set row height
                    table.Rows[rowIndex].Height = rowHeight;
                }

                // Set column widths
                for (int colIndex = 0; colIndex < totalColumns; colIndex++)
                {
                    if (columnWidths[colIndex] <= 0)
                        columnWidths[colIndex] = textHeight * 8;

                    table.Columns[colIndex].Width = columnWidths[colIndex];
                }

                // Merge cells
                if (mergeCells != null)
                {
                    foreach (int[] merge in mergeCells)
                    {
                        if (merge.Length >= 4)
                        {
                            CellRange range = CellRange.Create(table, merge[0], merge[2], merge[1], merge[3]);
                            table.MergeCells(range);
                        }
                    }
                }

                // Bật regenerate
                table.SuppressRegenerateTable(false);
                table.GenerateLayout();

                // Thêm vào drawing
                currentSpace.AppendEntity(table);
                transaction.AddNewlyCreatedDBObject(table, true);

                transaction.Commit();
                return table.ObjectId;
            }
        }

        /// <summary>
        /// Tạo AutoCAD Table với cột thủ công (Manual Column Width).
        /// Tương đương ND:list->table trong LISP.
        /// </summary>
        public static ObjectId CreateTableManualWidth(
            Database database,
            List<List<string>> tableData,
            Dictionary<int, double> rowHeights,
            Dictionary<int, double> columnWidths,
            double textHeight,
            List<int[]> mergeCells,
            Point3d insertionPoint,
            string tableStyleName,
            string textStyleName,
            string layerName)
        {
            int totalColumns = tableData.Max(row => row.Count);
            int totalRows = tableData.Count;

            // Lấy row height mặc định
            double defaultRowHeight = rowHeights.ContainsKey(0) ? rowHeights[0] : 360;
            double defaultTextHeight = textHeight;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTableRecord currentSpace = GetCurrentSpace(database, transaction);

                Table table = new Table();
                table.TableStyle = GetTableStyleId(database, transaction, tableStyleName);
                table.Position = insertionPoint;
                table.SetDatabaseDefaults();
                table.SetSize(totalRows, totalColumns);
                table.Layer = layerName;
                table.SuppressRegenerateTable(true);

                // Set cell margins
                table.Cells.Borders.Horizontal.Margin = textHeight;
                table.Cells.Borders.Vertical.Margin = textHeight;

                for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
                {
                    List<string> row = tableData[rowIndex];

                    // Lấy row height cho hàng này
                    double rowHeight;
                    if (!rowHeights.TryGetValue(rowIndex, out rowHeight))
                        rowHeight = defaultRowHeight;

                    for (int colIndex = 0; colIndex < totalColumns; colIndex++)
                    {
                        Cell cell = table.Cells[rowIndex, colIndex];

                        ObjectId textStyleId = GetTextStyleId(database, transaction, textStyleName);
                        if (!textStyleId.IsNull)
                        {
                            cell.TextStyleId = textStyleId;
                        }

                        // Alignment: title/header = MiddleCenter, data rows phân biệt theo cột
                        if (rowIndex <= 1)
                        {
                            cell.Alignment = CellAlignment.MiddleCenter;
                        }
                        else
                        {
                            // Cột 0 (STT) và cột cuối: MiddleCenter
                            // Các cột giữa: MiddleLeft
                            if (colIndex == 0 || colIndex >= totalColumns - 1)
                                cell.Alignment = CellAlignment.MiddleCenter;
                            else
                                cell.Alignment = CellAlignment.MiddleLeft;
                        }

                        cell.TextHeight = defaultTextHeight;

                        string cellValue = colIndex < row.Count ? row[colIndex] : "";

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            if (cellValue.StartsWith("%%BLK"))
                            {
                                string blockName = cellValue.Substring(5);
                                ObjectId blockId = GetBlockId(database, transaction, blockName);

                                if (!blockId.IsNull)
                                {
                                    cell.BlockTableRecordId = blockId;
                                }
                            }
                            else
                            {
                                cell.SetValue(cellValue, ParseOption.ParseOptionNone);
                            }
                        }
                    }

                    table.Rows[rowIndex].Height = rowHeight;
                }

                // Set column widths
                double defaultColWidth = columnWidths.ContainsKey(0) ? columnWidths[0] : 1000;
                for (int colIndex = 0; colIndex < totalColumns; colIndex++)
                {
                    double width;
                    if (!columnWidths.TryGetValue(colIndex, out width))
                        width = defaultColWidth;

                    table.Columns[colIndex].Width = width;
                }

                // Merge cells
                if (mergeCells != null)
                {
                    foreach (int[] merge in mergeCells)
                    {
                        if (merge.Length >= 4)
                        {
                            CellRange range = CellRange.Create(table, merge[0], merge[2], merge[1], merge[3]);
                            table.MergeCells(range);
                        }
                    }
                }

                table.SuppressRegenerateTable(false);
                table.GenerateLayout();

                currentSpace.AppendEntity(table);
                transaction.AddNewlyCreatedDBObject(table, true);

                transaction.Commit();
                return table.ObjectId;
            }
        }

        // -------------------------------------------------------
        //  AUTO-UPDATE HELPERS
        // -------------------------------------------------------

        public static void AttachAtkTableXData(Database database, ObjectId tableId, string blockName)
        {
            using (Transaction tr = database.TransactionManager.StartTransaction())
            {
                RegAppTable regTable = (RegAppTable)tr.GetObject(database.RegAppTableId, OpenMode.ForRead);
                if (!regTable.Has("ATL_ATK_TABLE"))
                {
                    regTable.UpgradeOpen();
                    RegAppTableRecord regRecord = new RegAppTableRecord();
                    regRecord.Name = "ATL_ATK_TABLE";
                    regTable.Add(regRecord);
                    tr.AddNewlyCreatedDBObject(regRecord, true);
                }

                Table table = tr.GetObject(tableId, OpenMode.ForWrite) as Table;
                if (table != null)
                {
                    ResultBuffer rb = new ResultBuffer(
                        new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ATL_ATK_TABLE"),
                        new TypedValue((int)DxfCode.ExtendedDataAsciiString, blockName ?? "")
                    );
                    table.XData = rb;
                }
                tr.Commit();
            }
        }

        public static void UpdateTableData(Database database, ObjectId tableId, List<List<string>> fullTableData)
        {
            using (Transaction tr = database.TransactionManager.StartTransaction())
            {
                Table table = tr.GetObject(tableId, OpenMode.ForWrite) as Table;
                if (table == null) return;

                int targetRows = fullTableData.Count;
                int targetCols = fullTableData.Max(r => r.Count);

                table.SuppressRegenerateTable(true);

                // Bỏ merge cũ ở hàng tiêu đề để tránh lỗi khi thay đổi kích thước
                try
                {
                    if (table.Rows.Count > 0 && table.Columns.Count > 1)
                    {
                        table.UnmergeCells(CellRange.Create(table, 0, 0, 0, table.Columns.Count - 1));
                    }
                }
                catch { }

                // Cập nhật số cột
                while (table.Columns.Count < targetCols)
                {
                    double width = table.Columns.Count > 0 ? table.Columns[table.Columns.Count - 1].Width : 1000;
                    table.InsertColumns(table.Columns.Count, width, 1);
                }
                while (table.Columns.Count > targetCols)
                    table.DeleteColumns(table.Columns.Count - 1, 1);

                // Cập nhật số hàng
                while (table.Rows.Count < targetRows)
                {
                    double height = table.Rows.Count > 1 ? table.Rows[table.Rows.Count - 1].Height : 360;
                    table.InsertRows(table.Rows.Count, height, 1);
                }
                while (table.Rows.Count > targetRows)
                    table.DeleteRows(table.Rows.Count - 1, 1);

                // Cập nhật dữ liệu
                for (int rowIndex = 0; rowIndex < targetRows; rowIndex++)
                {
                    List<string> row = fullTableData[rowIndex];
                    for (int colIndex = 0; colIndex < targetCols; colIndex++)
                    {
                        Cell cell = table.Cells[rowIndex, colIndex];
                        
                        string cellValue = colIndex < row.Count ? row[colIndex] : "";

                        if (!string.IsNullOrEmpty(cellValue) && cellValue.StartsWith("%%BLK"))
                        {
                            string blockName = cellValue.Substring(5);
                            ObjectId blockId = GetBlockId(database, tr, blockName);
                            if (!blockId.IsNull)
                                cell.BlockTableRecordId = blockId;
                        }
                        else
                        {
                            // Thay vì Clear() làm mất định dạng (màu, canh lề, font mà người dùng có thể đã sửa),
                            // chỉ SetValue để ghi đè nội dung chữ.
                            cell.SetValue(cellValue, ParseOption.ParseOptionNone);
                        }
                    }
                }

                // Tiêu đề merge lại
                if (targetRows > 0 && targetCols > 1)
                {
                    table.MergeCells(CellRange.Create(table, 0, 0, 0, targetCols - 1));
                }

                table.SuppressRegenerateTable(false);
                tr.Commit();
            }
        }

        // -------------------------------------------------------
        //  PRIVATE HELPERS
        // -------------------------------------------------------

        private static BlockTableRecord GetCurrentSpace(Database database, Transaction transaction)
        {
            return (BlockTableRecord)transaction.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);
        }

        private static ObjectId GetTableStyleId(Database database, Transaction transaction, string styleName)
        {
            DBDictionary tableStyleDict = (DBDictionary)transaction.GetObject(
                database.TableStyleDictionaryId, OpenMode.ForRead);

            if (tableStyleDict.Contains(styleName))
                return tableStyleDict.GetAt(styleName);

            return database.Tablestyle;
        }

        private static ObjectId GetTextStyleId(Database database, Transaction transaction, string styleName)
        {
            TextStyleTable textStyleTable = (TextStyleTable)transaction.GetObject(
                database.TextStyleTableId, OpenMode.ForRead);

            if (textStyleTable.Has(styleName))
                return textStyleTable[styleName];

            return database.Textstyle;
        }

        private static ObjectId GetBlockId(Database database, Transaction transaction, string blockName)
        {
            BlockTable blockTable = (BlockTable)transaction.GetObject(
                database.BlockTableId, OpenMode.ForRead);

            if (blockTable.Has(blockName))
                return blockTable[blockName];

            return ObjectId.Null;
        }
    }
}
