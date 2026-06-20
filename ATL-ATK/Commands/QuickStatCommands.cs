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
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ATL_ATK.Commands
{
    /// <summary>
    /// Các lệnh thống kê nhanh: AT1, DY1, ATDY1.
    /// Tương đương C:at1, C:dy1, C:atdy1 trong LISP.
    /// Không cần DCL, chọn block → tạo bảng ngay.
    /// </summary>
    public class QuickStatCommands
    {
        // -------------------------------------------------------
        //  AT1 - Thống kê nhanh Block Att
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh AT1 - Thống kê nhanh Block Attribute.
        /// Tương đương C:at1 trong LISP.
        /// Chọn Block INSERT → Tạo bảng: STT | Tên Block | [Tất cả Attributes]
        /// </summary>
        [CommandMethod("AT1")]
        public void ExecuteAT1()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return;

            Editor editor = document.Editor;
            Database database = document.Database;

            try
            {
                AtlCommand.EnsureInitialized();

                // Chọn Block có Attribute
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

                // Sắp xếp theo tên Block
                blockDataList = SortingService.SortByBlockName(blockDataList);

                // Chọn điểm chèn
                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Lấy danh sách unique Tags
                List<string> uniqueBlockNames = blockDataList.Select(b => b.BlockName).Distinct().ToList();
                List<string> allTags = new List<string>();
                foreach (string blockName in uniqueBlockNames)
                {
                    List<string> tags = BlockQueryService.GetAttributeTagsFromDefinition(database, blockName);
                    foreach (string tag in tags)
                    {
                        if (!allTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                            allTags.Add(tag);
                    }
                }

                // Build table data
                AtkSettings settings = AtlCommand.GlobalSettings;
                int precision = Convert.ToInt32(Application.GetSystemVariable("LUPREC"));
                var tableData = new List<List<string>>();

                // Title
                tableData.Add(new List<string> { "BẢNG THỐNG KE BLOCK ATTRIBUTE" });

                // Header
                var header = new List<string> { "STT", "Tên Block" };
                header.AddRange(allTags);
                tableData.Add(header);

                // Data rows
                int stt = 0;
                foreach (BlockData block in blockDataList)
                {
                    stt++;
                    var row = new List<string> { stt.ToString(), block.BlockName };

                    foreach (string tag in allTags)
                    {
                        string value;
                        if (block.Attributes.TryGetValue(tag, out value))
                            row.Add(StringHelper.UnformatMText(value));
                        else
                            row.Add("");
                    }

                    tableData.Add(row);
                }

                // Sum row
                if (settings.ShowSumRow)
                {
                    tableData.Add(CountLogic.BuildSumRow(tableData, header.Count));
                }

                // Create table
                using (document.LockDocument())
                {
                    TableGeneratorService.CreateTableAutoWidth(
                        database, tableData,
                        settings.TextHeight,
                        settings.TextHeight * 3.0,
                        null,
                        pointResult.Value,
                        GetTableStyleName(database, settings),
                        GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng: {blockDataList.Count} block, {allTags.Count} attribute.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh AT1: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  DY1 - Thống kê nhanh Block Dynamic
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh DY1 - Thống kê nhanh Block Dynamic.
        /// Tương đương C:dy1 trong LISP.
        /// Chọn Block INSERT → Tạo bảng: STT | Tên Block | [Dynamic Properties]
        /// </summary>
        [CommandMethod("DY1")]
        public void ExecuteDY1()
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

                // Chỉ giữ block có Dynamic Properties
                blockDataList = blockDataList.Where(b => b.DynamicProperties.Count > 0).ToList();
                if (blockDataList.Count == 0)
                {
                    editor.WriteMessage("\nKhông có Block Dynamic nào.");
                    return;
                }

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Lấy unique dynamic property names (bỏ Origin)
                var allDynProps = blockDataList
                    .SelectMany(b => b.DynamicProperties.Keys)
                    .Where(k => !string.Equals(k, "Origin", StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(k => k)
                    .ToList();

                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                // Title
                tableData.Add(new List<string> { "BẢNG THỐNG KE BLOCK DYNAMIC" });

                // Header
                var header = new List<string> { "STT", "Tên Block" };
                header.AddRange(allDynProps);
                tableData.Add(header);

                // Data rows
                int stt = 0;
                foreach (BlockData block in blockDataList)
                {
                    stt++;
                    var row = new List<string> { stt.ToString(), block.BlockName };

                    foreach (string prop in allDynProps)
                    {
                        string value;
                        if (block.DynamicProperties.TryGetValue(prop, out value))
                            row.Add(value);
                        else
                            row.Add("");
                    }

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
                        GetTableStyleName(database, settings),
                        GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng: {blockDataList.Count} block, {allDynProps.Count} property.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh DY1: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  ATDY1 - Thống kê nhanh Block Att-Dynamic
        // -------------------------------------------------------

        /// <summary>
        /// Lệnh ATDY1 - Thống kê nhanh Block Attribute và Dynamic.
        /// Tương đương C:atdy1 trong LISP.
        /// Chọn Block INSERT → Tạo bảng: STT | Tên Block | [Attributes] | [Dynamic Props]
        /// </summary>
        [CommandMethod("ATDY1")]
        public void ExecuteATDY1()
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

                blockDataList = SortingService.SortByBlockName(blockDataList);

                PromptPointResult pointResult = editor.GetPoint("\nChọn điểm xuất bảng thống kê: ");
                if (pointResult.Status != PromptStatus.OK) return;

                // Unique attribute tags
                List<string> uniqueBlockNames = blockDataList.Select(b => b.BlockName).Distinct().ToList();
                var allTags = new List<string>();
                foreach (string blockName in uniqueBlockNames)
                {
                    foreach (string tag in BlockQueryService.GetAttributeTagsFromDefinition(database, blockName))
                    {
                        if (!allTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                            allTags.Add(tag);
                    }
                }

                // Unique dynamic properties
                var allDynProps = blockDataList
                    .SelectMany(b => b.DynamicProperties.Keys)
                    .Where(k => !string.Equals(k, "Origin", StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(k => k)
                    .ToList();

                AtkSettings settings = AtlCommand.GlobalSettings;
                var tableData = new List<List<string>>();

                // Title
                tableData.Add(new List<string> { "BẢNG THỐNG KE BLOCK ATTRIBUTE-DYNAMIC" });

                // Header
                var header = new List<string> { "STT", "Tên Block" };
                header.AddRange(allTags);
                header.AddRange(allDynProps);
                tableData.Add(header);

                // Data rows
                int stt = 0;
                foreach (BlockData block in blockDataList)
                {
                    stt++;
                    var row = new List<string> { stt.ToString(), block.BlockName };

                    foreach (string tag in allTags)
                    {
                        string value;
                        if (block.Attributes.TryGetValue(tag, out value))
                            row.Add(StringHelper.UnformatMText(value));
                        else
                            row.Add("");
                    }

                    foreach (string prop in allDynProps)
                    {
                        string value;
                        if (block.DynamicProperties.TryGetValue(prop, out value))
                            row.Add(value);
                        else
                            row.Add("");
                    }

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
                        GetTableStyleName(database, settings),
                        GetTextStyleName(database, settings),
                        Application.GetSystemVariable("CLAYER").ToString());
                }

                editor.WriteMessage($"\nĐã tạo bảng: {blockDataList.Count} block.");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage($"\nLỗi lệnh ATDY1: {ex.Message}");
            }
        }

        // -------------------------------------------------------
        //  SHARED HELPERS
        // -------------------------------------------------------

        internal static string GetTableStyleName(Database database, AtkSettings settings)
        {
            List<string> styles = BlockQueryService.GetTableStyleNames(database);
            if (!string.IsNullOrEmpty(settings.TableStyleName) && styles.Contains(settings.TableStyleName))
                return settings.TableStyleName;
            return Application.GetSystemVariable("CTABLESTYLE")?.ToString() ?? "Standard";
        }

        internal static string GetTextStyleName(Database database, AtkSettings settings)
        {
            List<string> styles = BlockQueryService.GetTextStyleNames(database);
            if (!string.IsNullOrEmpty(settings.TextStyleName) && styles.Contains(settings.TextStyleName))
                return settings.TextStyleName;
            return Application.GetSystemVariable("TEXTSTYLE")?.ToString() ?? "Standard";
        }
    }
}
