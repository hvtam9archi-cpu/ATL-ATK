using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ATL_ATK.Models;
using ATL_ATK.Helpers;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ATL_ATK.Services
{
    public class AtkLogic
    {
        public static List<BlockData> SelectBlocks(Editor editor, Database database, AtkSettings settings)
        {
            if (settings.SelectByLayout || settings.SelectAll)
            {
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
                return BlockQueryService.SelectAndExtractBlocks(editor, database);
            }
        }

        public static List<List<string>> BuildTableData(
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

        public static List<string> BuildSumRow(List<List<string>> tableRows, int totalColumns)
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

        public static void ReplaceSequenceNumbers(List<List<string>> tableRows)
        {
            const string STT_PLACEHOLDER = "!@#$%^&*(STT)*&^%$#@!";

            int dataRowCount = tableRows.Count;

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

        public static void GenerateTable(Database database, Document document, AtkSettings settings, Point3d insertionPoint, List<List<string>> fullTableData)
        {
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
        }
    }
}
