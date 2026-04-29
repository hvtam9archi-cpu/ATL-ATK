using System;
using System.Collections.Generic;
using System.IO;
using ATL_ATK.Models;

namespace ATL_ATK.Services
{
    /// <summary>
    /// Service lưu/đọc thiết lập từ file .txt.
    /// Tương đương ND:ATK_savesettings và ND:ATK_loadsettings trong LISP.
    /// Format file: mỗi dòng là "KEY\tVALUE"
    /// </summary>
    public static class SettingsIOService
    {
        /// <summary>
        /// Lưu thiết lập ra file .txt.
        /// Tương đương ND:ATK_savesettings trong LISP.
        /// </summary>
        public static void SaveSettings(AtkSettings settings, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                if (!string.IsNullOrEmpty(settings.BlockName))
                    writer.WriteLine("BLOCK_NAME\t" + settings.BlockName);

                writer.WriteLine("TOTAL_COLUMNS\t" + settings.TotalColumns);
                writer.WriteLine("TABLE_STYLE\t" + (settings.TableStyleName ?? ""));
                writer.WriteLine("TEXT_STYLE\t" + (settings.TextStyleName ?? ""));
                writer.WriteLine("TITLE\t" + settings.Title);
                writer.WriteLine("TEXT_HEIGHT\t" + settings.TextHeight);
                writer.WriteLine("ROW_HEIGHT\t" + settings.RowHeight);
                writer.WriteLine("AUTO_COLUMN\t" + (settings.AutoColumnWidth ? "1" : "0"));
                writer.WriteLine("SHOW_SUM\t" + (settings.ShowSumRow ? "1" : "0"));

                // Lưu các cột
                for (int i = 0; i < 9; i++)
                {
                    ColumnDefinition col = settings.Columns[i];
                    writer.WriteLine($"TAG{i + 1}\t{col.TagExpression}");
                    writer.WriteLine($"TIT{i + 1}\t{col.Title}");
                    writer.WriteLine($"WID{i + 1}\t{col.Width}");
                }

                // Lưu thiết lập chọn / sắp xếp
                writer.WriteLine("SEL_MANUAL\t" + (settings.SelectManual ? "1" : "0"));
                writer.WriteLine("SEL_LAYOUT\t" + (settings.SelectByLayout ? "1" : "0"));
                writer.WriteLine("SEL_ALL\t" + (settings.SelectAll ? "1" : "0"));
                writer.WriteLine("SEL_1BLK\t" + (settings.SelectOneBlock ? "1" : "0"));
                writer.WriteLine("SEL_NBLK\t" + (settings.SelectMultiBlock ? "1" : "0"));
                writer.WriteLine("SORT_POS\t" + (settings.SortByPosition ? "1" : "0"));
                writer.WriteLine("SORT_COL\t" + (settings.SortByColumn ? "1" : "0"));
                writer.WriteLine("SORT_DIR1\t" + settings.SortDirection1);
                writer.WriteLine("SORT_DIR2\t" + settings.SortDirection2);
                writer.WriteLine("SORT_FUZZ\t" + settings.SortFuzz);
                writer.WriteLine("SORT_COL_IDX\t" + settings.SortColumnIndex);
            }
        }

        /// <summary>
        /// Đọc thiết lập từ file .txt.
        /// Tương đương ND:ATK_loadsettings trong LISP.
        /// </summary>
        public static AtkSettings LoadSettings(string filePath)
        {
            var settings = new AtkSettings();

            if (!File.Exists(filePath))
                return settings;

            string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                int tabIndex = line.IndexOf('\t');
                if (tabIndex < 0)
                    continue;

                string key = line.Substring(0, tabIndex).Trim();
                string value = tabIndex + 1 < line.Length ? line.Substring(tabIndex + 1) : "";

                ApplySetting(settings, key, value);
            }

            return settings;
        }

        // -------------------------------------------------------
        //  PRIVATE HELPERS
        // -------------------------------------------------------

        private static void ApplySetting(AtkSettings settings, string key, string value)
        {
            switch (key)
            {
                case "BLOCK_NAME":
                    settings.BlockName = value;
                    break;
                case "TOTAL_COLUMNS":
                    if (int.TryParse(value, out int totalCols))
                        settings.TotalColumns = Math.Max(1, Math.Min(9, totalCols));
                    break;
                case "TABLE_STYLE":
                    settings.TableStyleName = value;
                    break;
                case "TEXT_STYLE":
                    settings.TextStyleName = value;
                    break;
                case "TITLE":
                    settings.Title = value;
                    break;
                case "TEXT_HEIGHT":
                    if (double.TryParse(value, out double textHeight))
                        settings.TextHeight = textHeight;
                    break;
                case "ROW_HEIGHT":
                    if (double.TryParse(value, out double rowHeight))
                        settings.RowHeight = rowHeight;
                    break;
                case "AUTO_COLUMN":
                    settings.AutoColumnWidth = value == "1";
                    break;
                case "SHOW_SUM":
                    settings.ShowSumRow = value == "1";
                    break;

                case "SEL_MANUAL":
                    settings.SelectManual = value == "1";
                    break;
                case "SEL_LAYOUT":
                    settings.SelectByLayout = value == "1";
                    break;
                case "SEL_ALL":
                    settings.SelectAll = value == "1";
                    break;
                case "SEL_1BLK":
                    settings.SelectOneBlock = value == "1";
                    break;
                case "SEL_NBLK":
                    settings.SelectMultiBlock = value == "1";
                    break;
                case "SORT_POS":
                    settings.SortByPosition = value == "1";
                    break;
                case "SORT_COL":
                    settings.SortByColumn = value == "1";
                    break;
                case "SORT_DIR1":
                    if (int.TryParse(value, out int dir1))
                        settings.SortDirection1 = dir1;
                    break;
                case "SORT_DIR2":
                    if (int.TryParse(value, out int dir2))
                        settings.SortDirection2 = dir2;
                    break;
                case "SORT_FUZZ":
                    if (double.TryParse(value, out double fuzz))
                        settings.SortFuzz = fuzz;
                    break;
                case "SORT_COL_IDX":
                    if (int.TryParse(value, out int colIdx))
                        settings.SortColumnIndex = colIdx;
                    break;

                default:
                    // Parse TAG1..TAG9, TIT1..TIT9, WID1..WID9
                    if (key.StartsWith("TAG") && key.Length <= 4)
                    {
                        int idx = ParseColumnIndex(key, 3);
                        if (idx >= 0 && idx < 9)
                            settings.Columns[idx].TagExpression = value;
                    }
                    else if (key.StartsWith("TIT") && key.Length <= 4)
                    {
                        int idx = ParseColumnIndex(key, 3);
                        if (idx >= 0 && idx < 9)
                            settings.Columns[idx].Title = value;
                    }
                    else if (key.StartsWith("WID") && key.Length <= 4)
                    {
                        int idx = ParseColumnIndex(key, 3);
                        if (idx >= 0 && idx < 9)
                        {
                            if (double.TryParse(value, out double width))
                                settings.Columns[idx].Width = width;
                        }
                    }
                    break;
            }
        }

        /// <summary>Trích chỉ số cột (0-based) từ key "TAG1"→0, "TAG9"→8</summary>
        private static int ParseColumnIndex(string key, int prefixLength)
        {
            string numStr = key.Substring(prefixLength);
            if (int.TryParse(numStr, out int num))
                return num - 1; // Convert 1-based to 0-based
            return -1;
        }
    }
}
