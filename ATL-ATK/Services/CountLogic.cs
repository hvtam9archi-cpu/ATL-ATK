using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using ATL_ATK.Models;
using ATL_ATK.Helpers;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ATL_ATK.Services
{
    public class CountLogic
    {
        public static List<string> BuildSumRow(List<List<string>> tableData, int totalColumns)
        {
            var sumRow = new List<string>();
            var dataRows = tableData.Skip(2).ToList();

            for (int colIndex = 0; colIndex < totalColumns; colIndex++)
            {
                double sum = 0;
                bool hasNumber = false;

                foreach (var row in dataRows)
                {
                    if (colIndex < row.Count && double.TryParse(row[colIndex], out double val))
                    {
                        sum += val;
                        hasNumber = true;
                    }
                }

                if (hasNumber)
                    sumRow.Add(StringHelper.FormatNumber(sum));
                else
                    sumRow.Add(colIndex == 0 ? "TỔNG" : "");
            }

            return sumRow;
        }

        public static void GenerateCountTable(Database database, Document document, AtkSettings settings, Point3d insertionPoint, List<List<string>> tableData)
        {
            string tableStyleName = GetTableStyleName(database, settings);
            string textStyleName = GetTextStyleName(database, settings);
            string layerName = Application.GetSystemVariable("CLAYER")?.ToString() ?? "0";

            using (DocumentLock docLock = document.LockDocument())
            {
                TableGeneratorService.CreateTableAutoWidth(
                    database, tableData,
                    settings.TextHeight,
                    settings.TextHeight * 3.0,
                    null,
                    insertionPoint,
                    tableStyleName,
                    textStyleName,
                    layerName);
            }
        }

        public static string GetTableStyleName(Database database, AtkSettings settings)
        {
            List<string> styles = BlockQueryService.GetTableStyleNames(database);
            if (!string.IsNullOrEmpty(settings.TableStyleName) && styles.Contains(settings.TableStyleName))
                return settings.TableStyleName;
            return Application.GetSystemVariable("CTABLESTYLE")?.ToString() ?? "Standard";
        }

        public static string GetTextStyleName(Database database, AtkSettings settings)
        {
            List<string> styles = BlockQueryService.GetTextStyleNames(database);
            if (!string.IsNullOrEmpty(settings.TextStyleName) && styles.Contains(settings.TextStyleName))
                return settings.TextStyleName;
            return Application.GetSystemVariable("TEXTSTYLE")?.ToString() ?? "Standard";
        }
    }
}
