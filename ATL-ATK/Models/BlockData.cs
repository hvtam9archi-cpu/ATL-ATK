using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace ATL_ATK.Models
{
    /// <summary>
    /// Dữ liệu một Block Insert đã trích xuất.
    /// Chứa entity name, tên block, vị trí, attributes và dynamic properties.
    /// </summary>
    public class BlockData
    {
        /// <summary>ObjectId của BlockReference</summary>
        public ObjectId ObjectId { get; set; }

        /// <summary>Tên Block (effective name cho dynamic block)</summary>
        public string BlockName { get; set; }

        /// <summary>Điểm chèn (Insertion Point)</summary>
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>Danh sách Attribute: Tag -> TextString</summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>Danh sách Dynamic Properties (visible): PropertyName -> Value (string)</summary>
        public Dictionary<string, string> DynamicProperties { get; set; } = new Dictionary<string, string>();

        /// <summary>Trạng thái Visibility hiện tại (nếu có)</summary>
        public string VisibilityState { get; set; }
    }

    /// <summary>
    /// Kết quả đếm: giá trị + số lượng
    /// </summary>
    public class CountItem
    {
        /// <summary>Giá trị các cột</summary>
        public List<string> Values { get; set; } = new List<string>();

        /// <summary>Số lượng</summary>
        public int Count { get; set; }
    }
}
