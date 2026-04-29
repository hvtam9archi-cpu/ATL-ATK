using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ATL_ATK.Helpers;
using ATL_ATK.Models;

namespace ATL_ATK.Services
{
    /// <summary>
    /// Service truy vấn và trích xuất dữ liệu Block từ bản vẽ.
    /// Tương đương các hàm ND:att_get, ND:dyn_get, ND:get_blkname, v.v. trong LISP.
    /// </summary>
    public static class BlockQueryService
    {
        // -------------------------------------------------------
        //  LẤY TÊN BLOCK
        // -------------------------------------------------------

        /// <summary>
        /// Lấy tên Block (hỗ trợ Dynamic Block → EffectiveName).
        /// Tương đương ND:get_blkname trong LISP.
        /// </summary>
        public static string GetBlockName(BlockReference blockRef)
        {
            if (blockRef == null)
                return null;

            if (blockRef.IsDynamicBlock)
            {
                using (BlockTableRecord dynamicBtr =
                    (BlockTableRecord)blockRef.DynamicBlockTableRecord.GetObject(OpenMode.ForRead))
                {
                    return dynamicBtr.Name;
                }
            }

            return blockRef.Name;
        }

        // -------------------------------------------------------
        //  ATTRIBUTE
        // -------------------------------------------------------

        /// <summary>
        /// Lấy danh sách Attribute Tag từ Block Definition (BlockTableRecord).
        /// Tương đương ND:att_get-blk trong LISP.
        /// </summary>
        public static List<string> GetAttributeTagsFromDefinition(Database database, string blockName)
        {
            var tags = new List<string>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

                if (!blockTable.Has(blockName))
                {
                    transaction.Commit();
                    return tags;
                }

                BlockTableRecord blockRecord = (BlockTableRecord)transaction.GetObject(blockTable[blockName], OpenMode.ForRead);

                foreach (ObjectId entId in blockRecord)
                {
                    AttributeDefinition attributeDefinition =
                        transaction.GetObject(entId, OpenMode.ForRead) as AttributeDefinition;

                    if (attributeDefinition != null)
                    {
                        tags.Add(attributeDefinition.Tag);
                    }
                }

                transaction.Commit();
            }

            return tags;
        }

        /// <summary>
        /// Lấy Attribute Tag->Value từ BlockReference.
        /// Tương đương ND:att_get trong LISP.
        /// </summary>
        public static Dictionary<string, string> GetAttributes(BlockReference blockRef, Transaction transaction)
        {
            var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (blockRef.AttributeCollection == null)
                return attributes;

            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                AttributeReference attRef = transaction.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                if (attRef != null && !attributes.ContainsKey(attRef.Tag))
                {
                    attributes[attRef.Tag] = attRef.TextString;
                }
            }

            return attributes;
        }

        // -------------------------------------------------------
        //  DYNAMIC PROPERTIES
        // -------------------------------------------------------

        /// <summary>
        /// Lấy tất cả Dynamic Properties từ BlockReference.
        /// Tương đương ND:dyn_get trong LISP.
        /// </summary>
        public static Dictionary<string, string> GetDynamicProperties(BlockReference blockRef)
        {
            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!blockRef.IsDynamicBlock)
                return properties;

            DynamicBlockReferencePropertyCollection dynProps = blockRef.DynamicBlockReferencePropertyCollection;

            foreach (DynamicBlockReferenceProperty dynProp in dynProps)
            {
                if (dynProp.PropertyName == "Origin")
                    continue;

                string valueString = ConvertDynPropValue(dynProp);
                properties[dynProp.PropertyName] = valueString;
            }

            return properties;
        }

        /// <summary>
        /// Lấy Dynamic Properties "visible" (chỉ các property có thể nhìn thấy).
        /// Tương đương ND:dyn_get-visible trong LISP.
        /// </summary>
        public static Dictionary<string, string> GetVisibleDynamicProperties(BlockReference blockRef)
        {
            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!blockRef.IsDynamicBlock)
                return properties;

            DynamicBlockReferencePropertyCollection dynProps = blockRef.DynamicBlockReferencePropertyCollection;

            foreach (DynamicBlockReferenceProperty dynProp in dynProps)
            {
                if (dynProp.PropertyName == "Origin")
                    continue;

                if (!dynProp.Show)
                    continue;

                string valueString = ConvertDynPropValue(dynProp);
                properties[dynProp.PropertyName] = valueString;
            }

            return properties;
        }

        /// <summary>
        /// Lấy trạng thái Visibility hiện tại của Block.
        /// Tương đương ND:dyn_visparam trong LISP.
        /// </summary>
        public static string GetVisibilityState(BlockReference blockRef)
        {
            if (!blockRef.IsDynamicBlock)
                return null;

            DynamicBlockReferencePropertyCollection dynProps = blockRef.DynamicBlockReferencePropertyCollection;

            foreach (DynamicBlockReferenceProperty dynProp in dynProps)
            {
                // Visibility parameter thường có tên chứa "Visibility" 
                // hoặc có UnitsType = NoUnits và AllowedValues > 0
                if (dynProp.PropertyName.IndexOf("Visibility", StringComparison.OrdinalIgnoreCase) >= 0
                    || IsVisibilityParameter(dynProp))
                {
                    return dynProp.Value?.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Lấy tên tham số Visibility của Block.
        /// Tương đương ND:getvisibilityparametername trong LISP.
        /// </summary>
        public static string GetVisibilityParameterName(BlockReference blockRef)
        {
            if (!blockRef.IsDynamicBlock)
                return null;

            DynamicBlockReferencePropertyCollection dynProps = blockRef.DynamicBlockReferencePropertyCollection;

            foreach (DynamicBlockReferenceProperty dynProp in dynProps)
            {
                if (dynProp.PropertyName.IndexOf("Visibility", StringComparison.OrdinalIgnoreCase) >= 0
                    || IsVisibilityParameter(dynProp))
                {
                    return dynProp.PropertyName;
                }
            }

            return null;
        }

        // -------------------------------------------------------
        //  TRÍCH XUẤT BLOCK DATA
        // -------------------------------------------------------

        /// <summary>
        /// Trích xuất toàn bộ dữ liệu từ một BlockReference.
        /// </summary>
        public static BlockData ExtractBlockData(BlockReference blockRef, Transaction transaction)
        {
            var data = new BlockData
            {
                ObjectId = blockRef.ObjectId,
                BlockName = GetBlockName(blockRef),
                X = blockRef.Position.X,
                Y = blockRef.Position.Y,
                Z = blockRef.Position.Z,
                Attributes = GetAttributes(blockRef, transaction),
                DynamicProperties = GetVisibleDynamicProperties(blockRef),
                VisibilityState = GetVisibilityState(blockRef)
            };

            return data;
        }

        /// <summary>
        /// Chọn và trích xuất danh sách Block INSERT từ người dùng.
        /// Trả về null nếu người dùng hủy chọn.
        /// </summary>
        public static List<BlockData> SelectAndExtractBlocks(
            Editor editor, Database database,
            string filterBlockName = null,
            bool insertOnly = true)
        {
            // Tạo filter
            var filterValues = new List<TypedValue>();
            filterValues.Add(new TypedValue((int)DxfCode.Start, "INSERT"));

            if (!string.IsNullOrEmpty(filterBlockName))
            {
                filterValues.Add(new TypedValue((int)DxfCode.BlockName, filterBlockName));
            }

            SelectionFilter selFilter = new SelectionFilter(filterValues.ToArray());
            PromptSelectionResult selResult = editor.GetSelection(selFilter);

            if (selResult.Status != PromptStatus.OK)
                return null;

            SelectionSet selectionSet = selResult.Value;
            var blockDataList = new List<BlockData>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (SelectedObject selObj in selectionSet)
                {
                    BlockReference blockRef = transaction.GetObject(selObj.ObjectId, OpenMode.ForRead) as BlockReference;

                    if (blockRef != null)
                    {
                        blockDataList.Add(ExtractBlockData(blockRef, transaction));
                    }
                }

                transaction.Commit();
            }

            return blockDataList;
        }

        /// <summary>
        /// Chọn và trích xuất Block INSERT + Text từ người dùng.
        /// Dùng cho lệnh NDC (đếm nội dung Att + Text).
        /// </summary>
        public static (List<BlockData> blocks, List<string> texts) SelectBlocksAndTexts(
            Editor editor, Database database)
        {
            var filterValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT,TEXT,MTEXT")
            };

            SelectionFilter selFilter = new SelectionFilter(filterValues);
            PromptSelectionResult selResult = editor.GetSelection(selFilter);

            if (selResult.Status != PromptStatus.OK)
                return (null, null);

            var blocks = new List<BlockData>();
            var texts = new List<string>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (SelectedObject selObj in selResult.Value)
                {
                    Entity entity = transaction.GetObject(selObj.ObjectId, OpenMode.ForRead) as Entity;

                    if (entity is BlockReference blockRef)
                    {
                        blocks.Add(ExtractBlockData(blockRef, transaction));
                    }
                    else if (entity is DBText dbText)
                    {
                        texts.Add(dbText.TextString);
                    }
                    else if (entity is MText mText)
                    {
                        texts.Add(StringHelper.UnformatMText(mText.Contents));
                    }
                }

                transaction.Commit();
            }

            return (blocks, texts);
        }

        /// <summary>
        /// Lấy tất cả Block INSERT trong layout chỉ định (dùng cho Select By Layout / All).
        /// </summary>
        public static List<BlockData> GetBlocksInLayout(Database database, string layoutName)
        {
            var blockDataList = new List<BlockData>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                // Tìm LayoutManager
                BlockTable blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

                // Tìm layout tương ứng
                DBDictionary layoutDict = (DBDictionary)transaction.GetObject(
                    database.LayoutDictionaryId, OpenMode.ForRead);

                foreach (DBDictionaryEntry entry in layoutDict)
                {
                    Layout layout = transaction.GetObject(entry.Value, OpenMode.ForRead) as Layout;

                    if (layout != null && layout.LayoutName == layoutName)
                    {
                        BlockTableRecord layoutBtr = (BlockTableRecord)transaction.GetObject(
                            layout.BlockTableRecordId, OpenMode.ForRead);

                        foreach (ObjectId objId in layoutBtr)
                        {
                            BlockReference blockRef = transaction.GetObject(objId, OpenMode.ForRead) as BlockReference;

                            if (blockRef != null)
                            {
                                blockDataList.Add(ExtractBlockData(blockRef, transaction));
                            }
                        }

                        break;
                    }
                }

                transaction.Commit();
            }

            return blockDataList;
        }

        /// <summary>
        /// Lấy danh sách tên Layout (trừ Model).
        /// Tương đương ND:layoutlist trong LISP.
        /// </summary>
        public static List<string> GetLayoutNames(Database database)
        {
            var names = new List<string>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                DBDictionary layoutDict = (DBDictionary)transaction.GetObject(
                    database.LayoutDictionaryId, OpenMode.ForRead);

                var layoutInfos = new List<Tuple<int, string>>();

                foreach (DBDictionaryEntry entry in layoutDict)
                {
                    Layout layout = transaction.GetObject(entry.Value, OpenMode.ForRead) as Layout;
                    if (layout != null && layout.LayoutName != "Model")
                    {
                        layoutInfos.Add(Tuple.Create(layout.TabOrder, layout.LayoutName));
                    }
                }

                names = layoutInfos.OrderBy(x => x.Item1).Select(x => x.Item2).ToList();
                transaction.Commit();
            }

            return names;
        }

        /// <summary>
        /// Lấy danh sách tên Table Style.
        /// Tương đương ND:tablestyle_lst trong LISP.
        /// </summary>
        public static List<string> GetTableStyleNames(Database database)
        {
            var names = new List<string>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                DBDictionary tableStyleDict = (DBDictionary)transaction.GetObject(
                    database.TableStyleDictionaryId, OpenMode.ForRead);

                foreach (DBDictionaryEntry entry in tableStyleDict)
                {
                    names.Add(entry.Key);
                }

                transaction.Commit();
            }

            return names.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Lấy danh sách tên Text Style.
        /// Tương đương ND:table_lst("STYLE") trong LISP.
        /// </summary>
        public static List<string> GetTextStyleNames(Database database)
        {
            var names = new List<string>();

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                TextStyleTable textStyleTable = (TextStyleTable)transaction.GetObject(
                    database.TextStyleTableId, OpenMode.ForRead);

                foreach (ObjectId styleId in textStyleTable)
                {
                    TextStyleTableRecord style = (TextStyleTableRecord)transaction.GetObject(styleId, OpenMode.ForRead);

                    // Bỏ qua style rỗng hoặc chứa ký tự đặc biệt (tương tự LISP filter)
                    if (!string.IsNullOrEmpty(style.Name)
                        && !style.Name.Contains("*")
                        && !style.Name.Contains("|"))
                    {
                        names.Add(style.Name);
                    }
                }

                transaction.Commit();
            }

            return names.OrderBy(x => x).ToList();
        }

        // -------------------------------------------------------
        //  PRIVATE HELPERS
        // -------------------------------------------------------

        /// <summary>
        /// Chuyển đổi giá trị DynamicBlockReferenceProperty sang string.
        /// </summary>
        private static string ConvertDynPropValue(DynamicBlockReferenceProperty dynProp)
        {
            object value = dynProp.Value;

            if (value == null)
                return "";

            if (value is double doubleVal)
            {
                // Làm tròn để tránh floating point noise (tương đương LISP)
                double rounded = Math.Round(doubleVal, 4);
                return StringHelper.FormatNumber(rounded);
            }

            if (value is short shortVal)
                return shortVal.ToString();

            return value.ToString();
        }

        /// <summary>
        /// Kiểm tra xem DynamicBlockReferenceProperty có phải là Visibility Parameter không.
        /// </summary>
        private static bool IsVisibilityParameter(DynamicBlockReferenceProperty dynProp)
        {
            try
            {
                // Visibility parameter thường có AllowedValues chứa các string
                object[] allowedValues = dynProp.GetAllowedValues();
                if (allowedValues != null && allowedValues.Length > 0 && allowedValues[0] is string)
                    return true;
            }
            catch
            {
                // Bỏ qua nếu không lấy được AllowedValues
            }

            return false;
        }
    }
}
