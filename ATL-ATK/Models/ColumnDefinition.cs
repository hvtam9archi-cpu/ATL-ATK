namespace ATL_ATK.Models
{
    /// <summary>
    /// Định nghĩa một cột trong bảng thống kê.
    /// Mỗi cột gồm: biểu thức Tag, tên cột hiển thị, và độ rộng cột.
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>Biểu thức Tag (ví dụ: "%%STT", "TENBV", "MADUAN \"-\" MABV")</summary>
        public string TagExpression { get; set; } = "";

        /// <summary>Tên cột hiển thị trên tiêu đề bảng</summary>
        public string Title { get; set; } = "";

        /// <summary>Độ rộng cột (đơn vị drawing)</summary>
        public double Width { get; set; } = 1000;

        public ColumnDefinition() { }

        public ColumnDefinition(string tagExpression, string title, double width)
        {
            TagExpression = tagExpression;
            Title = title;
            Width = width;
        }
    }
}
