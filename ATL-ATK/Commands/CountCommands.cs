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
    /// Các lệnh đếm số lượng: ATC, DYC, ATDYC, BLC, NDC.
    /// Tương đương C:atc, C:dyc, C:atdyc, C:blc, C:ndc trong LISP.
    /// </summary>
    public class CountCommands
    {
        // -------------------------------------------------------
        //  ATC - Đếm số lượng Att
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh ATC - Đếm số lượng Block theo Attribute values.
        /// Tương đương C:atc trong LISP.
        /// </summary>
        [CommandMethod("ATC")]
        public void ExecuteATC()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();

                List<BlockData> blockDataList = BlockQueryService.SelectAndExtractBlocks(editor, database);

                if (blockDataList == null || blockDataList.Count == 0)
                    return;

                // Chỉ giữ block có attribute
                blockDataList = blockDataList.Where(b => b.Attributes.Count > 0).ToList();
                if (blockDataList.Count == 0)
                {
                    editor.WriteMessage("\nKhông có Block Attribute nào.");
                    return;
                }

                // Lấy danh sách unique tags
                List<string> uniqueBlockNames = blockDataList.Select(b => b.BlockName).Distinct().ToList();
                var allTags = new List<string>();
                foreach (string name in uniqueBlockNames)
                {
                    foreach (string tag in BlockQueryService.GetAttributeTagsFromDefinition(database, name))
                    {
                        if (!allTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                            allTags.Add(tag);
                    }
                }

                // Chọn Tags để đếm (nếu > 1)
                List<string> selectedTags = allTags;
                if (allTags.Count > 1)
                {
                    var dialog = new ListBoxDialog(allTags, "Chọn 1 hoặc nhiều Attribute Tag", true);
                    Application.ShowModalWindow(dialog);

                    if (!dialog.IsConfirmed || dialog.SelectedItems.Count == 0)
                        return;

                    selectedTags = dialog.SelectedItems;
                }

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Đếm theo giá trị attribute + block name
                var countDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                blockDataList = SortingService.SortByBlockName(blockDataList);

                foreach (BlockData block in blockDataList)
                {
                    var keyParts = new List<string> { block.BlockName };

                    foreach (string tag in selectedTags)
                    {
                        string value;
                        if (block.Attributes.TryGetValue(tag, out value))
                            keyParts.Add(StringHelper.UnformatMText(value));
                        else
                            keyParts.Add("");
                    }

                    string key = string.Join("\t", keyParts);

                    if (countDict.ContainsKey(key))
                        countDict[key]++;
                    else
                        countDict[key] = 1;
                }

                // Build table
                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                tableData.Add(new List<string> { "BẢNG THỐNG KÊ SỐ LƯỢNG ATTRIBUTE" });

                var header = new List<string> { "STT", "Tên Block" };
                header.AddRange(selectedTags);
                header.Add("Số lượng");
                tableData.Add(header);

                var sortedKeys = countDict.Keys.ToList();
                sortedKeys.Sort(new AlphanumComparer());

                int stt = 0;
                foreach (string key in sortedKeys)
                {
                    stt++;
                    string[] parts = key.Split('\t');
                    var row = new List<string> { stt.ToString() };
                    row.AddRange(parts);
                    row.Add(countDict[key].ToString());
                    tableData.Add(row);
                }

                // Sum row
                if (settings.ShowSumRow)
                {
                    tableData.Add(CountLogic.BuildSumRow(tableData, header.Count));
                }

                using (document.LockDocument())
                {
                    TableGeneratorService.CreateTableAutoWidth(
                        database, tableData,
                        settings.TextHeight,
                        settings.TextHeight * 3.0,
                        null,
                        pointResult.Value,
                        QuickStatCommands.GetTableStyleName(database, settings),
                        QuickStatCommands.GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng đếm: {sortedKeys.Count} mục.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh ATC: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  DYC - Đếm số lượng Parameter
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh DYC - Đếm số lượng Block theo Dynamic Parameter values.
        /// Tương đương C:dyc trong LISP.
        /// </summary>
        [CommandMethod("DYC")]
        public void ExecuteDYC()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();

                List<BlockData> blockDataList = BlockQueryService.SelectAndExtractBlocks(editor, database);

                if (blockDataList == null || blockDataList.Count == 0)
                    return;

                blockDataList = blockDataList.Where(b => b.DynamicProperties.Count > 0).ToList();
                if (blockDataList.Count == 0)
                {
                    editor.WriteMessage("\nKhông có Block Dynamic nào.");
                    return;
                }

                // Lấy unique parameters
                var allParams = blockDataList
                    .SelectMany(b => b.DynamicProperties.Keys)
                    .Where(k => !string.Equals(k, "Origin", StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(k => k)
                    .ToList();

                // Chọn parameters
                List<string> selectedParams = allParams;
                if (allParams.Count > 1)
                {
                    var dialog = new ListBoxDialog(allParams, "Chọn 1 hoặc nhiều Parameter", true);
                    Application.ShowModalWindow(dialog);

                    if (!dialog.IsConfirmed || dialog.SelectedItems.Count == 0)
                        return;

                    selectedParams = dialog.SelectedItems;
                }

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Đếm
                blockDataList = SortingService.SortByBlockName(blockDataList);
                var countDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (BlockData block in blockDataList)
                {
                    var keyParts = new List<string>();

                    foreach (string param in selectedParams)
                    {
                        string value;
                        if (block.DynamicProperties.TryGetValue(param, out value))
                            keyParts.Add(value);
                        else
                            keyParts.Add("");
                    }

                    string key = string.Join("\t", keyParts);

                    if (countDict.ContainsKey(key))
                        countDict[key]++;
                    else
                        countDict[key] = 1;
                }

                // Build table
                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                tableData.Add(new List<string> { "BẢNG THỐNG KÊ SỐ LƯỢNG PARAMETER" });

                var header = new List<string> { "STT" };
                header.AddRange(selectedParams);
                header.Add("Số lượng");
                tableData.Add(header);

                var sortedKeys = countDict.Keys.ToList();
                sortedKeys.Sort(new AlphanumComparer());

                int stt = 0;
                foreach (string key in sortedKeys)
                {
                    stt++;
                    string[] parts = key.Split('\t');
                    var row = new List<string> { stt.ToString() };
                    row.AddRange(parts);
                    row.Add(countDict[key].ToString());
                    tableData.Add(row);
                }

                if (settings.ShowSumRow)
                {
                    tableData.Add(CountLogic.BuildSumRow(tableData, header.Count));
                }

                using (document.LockDocument())
                {
                    TableGeneratorService.CreateTableAutoWidth(
                        database, tableData,
                        settings.TextHeight,
                        settings.TextHeight * 3.0,
                        null,
                        pointResult.Value,
                        QuickStatCommands.GetTableStyleName(database, settings),
                        QuickStatCommands.GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng đếm: {sortedKeys.Count} mục.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh DYC: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  ATDYC - Đếm số lượng Att và Parameter
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh ATDYC - Đếm số lượng Block theo cả Attribute và Dynamic Parameter.
        /// Tương đương C:atdyc trong LISP.
        /// </summary>
        [CommandMethod("ATDYC")]
        public void ExecuteATDYC()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();

                List<BlockData> blockDataList = BlockQueryService.SelectAndExtractBlocks(editor, database);

                if (blockDataList == null || blockDataList.Count == 0)
                    return;

                // Lấy unique tags + dynamic props
                List<string> uniqueBlockNames = blockDataList.Select(b => b.BlockName).Distinct().ToList();
                var allTags = new List<string>();
                foreach (string name in uniqueBlockNames)
                {
                    foreach (string tag in BlockQueryService.GetAttributeTagsFromDefinition(database, name))
                    {
                        if (!allTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                            allTags.Add(tag);
                    }
                }

                var allParams = blockDataList
                    .SelectMany(b => b.DynamicProperties.Keys)
                    .Where(k => !string.Equals(k, "Origin", StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(k => k)
                    .ToList();

                // Chọn items
                var allItems = new List<string>();
                allItems.AddRange(allTags.Select(t => "[ATT] " + t));
                allItems.AddRange(allParams.Select(p => "[DYN] " + p));

                List<string> selectedItems = allItems;
                if (allItems.Count > 1)
                {
                    var dialog = new ListBoxDialog(allItems, "Chọn Attribute/Parameter", true);
                    Application.ShowModalWindow(dialog);

                    if (!dialog.IsConfirmed || dialog.SelectedItems.Count == 0)
                        return;

                    selectedItems = dialog.SelectedItems;
                }

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Tách lại thành tags và params
                var selTags = selectedItems.Where(s => s.StartsWith("[ATT] "))
                    .Select(s => s.Substring(6)).ToList();
                var selParams = selectedItems.Where(s => s.StartsWith("[DYN] "))
                    .Select(s => s.Substring(6)).ToList();

                // Đếm
                blockDataList = SortingService.SortByBlockName(blockDataList);
                var countDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (BlockData block in blockDataList)
                {
                    var keyParts = new List<string> { block.BlockName };

                    foreach (string tag in selTags)
                    {
                        string value;
                        if (block.Attributes.TryGetValue(tag, out value))
                            keyParts.Add(StringHelper.UnformatMText(value));
                        else
                            keyParts.Add("");
                    }

                    foreach (string param in selParams)
                    {
                        string value;
                        if (block.DynamicProperties.TryGetValue(param, out value))
                            keyParts.Add(value);
                        else
                            keyParts.Add("");
                    }

                    string key = string.Join("\t", keyParts);

                    if (countDict.ContainsKey(key))
                        countDict[key]++;
                    else
                        countDict[key] = 1;
                }

                // Build table
                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                tableData.Add(new List<string> { "BẢNG THỐNG KÊ SỐ LƯỢNG ATTRIBUTE" });

                var header = new List<string> { "STT", "Tên Block" };
                header.AddRange(selTags);
                header.AddRange(selParams);
                header.Add("Số lượng");
                tableData.Add(header);

                var sortedKeys = countDict.Keys.ToList();
                sortedKeys.Sort(new AlphanumComparer());

                int stt = 0;
                foreach (string key in sortedKeys)
                {
                    stt++;
                    string[] parts = key.Split('\t');
                    var row = new List<string> { stt.ToString() };
                    row.AddRange(parts);
                    row.Add(countDict[key].ToString());
                    tableData.Add(row);
                }

                if (settings.ShowSumRow)
                {
                    tableData.Add(CountLogic.BuildSumRow(tableData, header.Count));
                }

                using (document.LockDocument())
                {
                    TableGeneratorService.CreateTableAutoWidth(
                        database, tableData,
                        settings.TextHeight,
                        settings.TextHeight * 3.0,
                        null,
                        pointResult.Value,
                        QuickStatCommands.GetTableStyleName(database, settings),
                        QuickStatCommands.GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng đếm: {sortedKeys.Count} mục.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh ATDYC: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  BLC - Đếm số lượng Block
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh BLC - Đếm số lượng Block (có thể đếm theo Visibility State).
        /// Tương đương C:blc trong LISP.
        /// </summary>
        [CommandMethod("BLC")]
        public void ExecuteBLC()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();

                List<BlockData> blockDataList = BlockQueryService.SelectAndExtractBlocks(editor, database);

                if (blockDataList == null || blockDataList.Count == 0)
                    return;

                // Kiểm tra có block nào có Visibility State không
                bool hasVisibility = blockDataList.Any(b => !string.IsNullOrEmpty(b.VisibilityState));

                string countByVisibility = "No";
                if (hasVisibility)
                {
                    // Hỏi người dùng
                    PromptKeywordOptions keyOptions = new PromptKeywordOptions(
                        "\nĐếm theo trạng thái Block (Visibility)? [Yes/No]", "Yes No");
                    keyOptions.Keywords.Default = "Yes";

                    PromptResult keyResult = editor.GetKeywords(keyOptions);
                    if (keyResult.Status != PromptStatus.OK) return;

                    countByVisibility = keyResult.StringResult;
                }

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                if (countByVisibility == "Yes")
                {
                    // Đếm theo Block Name + Visibility State
                    tableData.Add(new List<string> { "Bảng thống kê" });
                    tableData.Add(new List<string> { "STT", "Minh họa", "Tên Block", "Trạng thái", "Số lượng" });

                    var countDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    foreach (BlockData block in blockDataList)
                    {
                        string key = block.BlockName + "\t" + (block.VisibilityState ?? "");
                        if (countDict.ContainsKey(key))
                            countDict[key]++;
                        else
                            countDict[key] = 1;
                    }

                    var sortedKeys = countDict.Keys.ToList();
                    sortedKeys.Sort(new AlphanumComparer());

                    int stt = 0;
                    foreach (string key in sortedKeys)
                    {
                        stt++;
                        string[] parts = key.Split('\t');
                        string blockName = parts[0];
                        string visState = parts.Length > 1 ? parts[1] : "";

                        tableData.Add(new List<string>
                        {
                            stt.ToString(),
                            "%%BLK" + blockName,
                            blockName,
                            visState,
                            countDict[key].ToString()
                        });
                    }
                }
                else
                {
                    // Đếm theo Block Name
                    tableData.Add(new List<string> { "BẢNG THỐNG KÊ SỐ LƯỢNG BLOCK" });
                    tableData.Add(new List<string> { "STT", "Minh họa", "Tên Block", "Số lượng" });

                    var countDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    foreach (BlockData block in blockDataList)
                    {
                        if (countDict.ContainsKey(block.BlockName))
                            countDict[block.BlockName]++;
                        else
                            countDict[block.BlockName] = 1;
                    }

                    var sortedKeys = countDict.Keys.ToList();
                    sortedKeys.Sort(new AlphanumComparer());

                    int stt = 0;
                    foreach (string key in sortedKeys)
                    {
                        stt++;
                        tableData.Add(new List<string>
                        {
                            stt.ToString(),
                            "%%BLK" + key,
                            key,
                            countDict[key].ToString()
                        });
                    }
                }

                using (document.LockDocument())
                {
                    TableGeneratorService.CreateTableAutoWidth(
                        database, tableData,
                        settings.TextHeight,
                        settings.TextHeight * 3.0,
                        null,
                        pointResult.Value,
                        QuickStatCommands.GetTableStyleName(database, settings),
                        QuickStatCommands.GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage("\nĐã tạo bảng đếm Block.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh BLC: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  NDC - Đếm nội dung Att, Text
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh NDC - Đếm nội dung Attribute và Text.
        /// Tương đương C:ndc trong LISP.
        /// </summary>
        [CommandMethod("NDC")]
        public void ExecuteNDC()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();

                var result = BlockQueryService.SelectBlocksAndTexts(editor, database);

                if (result.blocks == null && result.texts == null)
                    return;

                // Thu thập tất cả nội dung (attribute values + text values)
                var contentCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                if (result.blocks != null)
                {
                    foreach (BlockData block in result.blocks)
                    {
                        foreach (var att in block.Attributes)
                        {
                            string value = StringHelper.UnformatMText(att.Value);
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (contentCount.ContainsKey(value))
                                    contentCount[value]++;
                                else
                                    contentCount[value] = 1;
                            }
                        }
                    }
                }

                if (result.texts != null)
                {
                    foreach (string text in result.texts)
                    {
                        string value = StringHelper.UnformatMText(text);
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (contentCount.ContainsKey(value))
                                contentCount[value]++;
                            else
                                contentCount[value] = 1;
                        }
                    }
                }

                if (contentCount.Count == 0)
                {
                    editor.WriteMessage("\nKhông có nội dung nào.");
                    return;
                }

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Build table
                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                tableData.Add(new List<string> { "BẢNG THỐNG KÊ" });
                tableData.Add(new List<string> { "STT", "NỘI DUNG", "SL" });

                var sortedKeys = contentCount.Keys.ToList();
                sortedKeys.Sort(new AlphanumComparer());

                int stt = 0;
                foreach (string key in sortedKeys)
                {
                    stt++;
                    tableData.Add(new List<string>
                    {
                        stt.ToString(),
                        key,
                        contentCount[key].ToString()
                    });
                }

                using (document.LockDocument())
                {
                    TableGeneratorService.CreateTableAutoWidth(
                        database, tableData,
                        settings.TextHeight,
                        settings.TextHeight * 3.0,
                        null,
                        pointResult.Value,
                        QuickStatCommands.GetTableStyleName(database, settings),
                        QuickStatCommands.GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng đếm nội dung: {sortedKeys.Count} mục.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh NDC: {ex.Message}");
            }
        }

    }
}
