using System.Collections.Generic;

namespace ATL_ATK.Models
{
    /// <summary>
    /// Thiết lập toàn cục cho plugin ATL-ATK.
    /// Tương đương các biến 3DUY-ATK-* trong LISP.
    /// </summary>
    public class AtkSettings
    {
        // ---- Thiết lập bảng ----
        /// <summary>Tên Table Style</summary>
        public string TableStyleName { get; set; } = "";

        /// <summary>Tên Text Style</summary>
        public string TextStyleName { get; set; } = "";

        /// <summary>Tiêu đề bảng</summary>
        public string Title { get; set; } = "DANH MỤC BẢN VẼ";

        /// <summary>Cao chữ (text height)</summary>
        public double TextHeight { get; set; } = 100;

        /// <summary>Cao hàng (row height)</summary>
        public double RowHeight { get; set; } = 360;

        /// <summary>Tự động điều chỉnh hàng cột</summary>
        public bool AutoColumnWidth { get; set; } = true;

        /// <summary>Thêm hàng tính tổng cuối bảng</summary>
        public bool ShowSumRow { get; set; } = true;

        // ---- Khai báo Block ----
        /// <summary>Tên Block Att đã chọn</summary>
        public string BlockName { get; set; }

        /// <summary>Danh sách Tag của Block đã chọn</summary>
        public List<string> TagList { get; set; } = new List<string>();

        // ---- Danh sách cột ----
        /// <summary>Số cột hiện tại (1-9)</summary>
        public int TotalColumns { get; set; } = 4;

        /// <summary>Định nghĩa 9 cột (max)</summary>
        public ColumnDefinition[] Columns { get; set; } = new ColumnDefinition[9];

        // ---- Thiết lập chọn / sắp xếp ----
        /// <summary>Chọn thủ công (Manual)</summary>
        public bool SelectManual { get; set; } = true;

        /// <summary>Chọn theo Layout</summary>
        public bool SelectByLayout { get; set; } = false;

        /// <summary>Chọn tất cả (All)</summary>
        public bool SelectAll { get; set; } = false;

        /// <summary>Chỉ chọn 1 loại Block</summary>
        public bool SelectOneBlock { get; set; } = true;

        /// <summary>Chọn nhiều loại Block</summary>
        public bool SelectMultiBlock { get; set; } = false;

        /// <summary>Sắp xếp theo vị trí tọa độ</summary>
        public bool SortByPosition { get; set; } = false;

        /// <summary>Sắp xếp theo cột</summary>
        public bool SortByColumn { get; set; } = false;

        /// <summary>Hướng sắp xếp theo vị trí: 0=Trái->Phải, 1=Phải->Trái, 2=Trên->Dưới, 3=Dưới->Trên</summary>
        public int SortDirection1 { get; set; } = 0;

        /// <summary>Hướng sắp xếp phụ</summary>
        public int SortDirection2 { get; set; } = 0;

        /// <summary>Dung sai sắp xếp (fuzz)</summary>
        public double SortFuzz { get; set; } = 100;

        /// <summary>Chỉ số cột dùng để sắp xếp</summary>
        public int SortColumnIndex { get; set; } = 2;

        /// <summary>Đường dẫn file settings đã lưu/load</summary>
        public string SettingsFilePath { get; set; }

        public AtkSettings()
        {
            InitDefaultColumns();
        }

        /// <summary>Khởi tạo cột mặc định tương đương LISP</summary>
        private void InitDefaultColumns()
        {
            Columns[0] = new ColumnDefinition("%%STT", "STT", 500);
            Columns[1] = new ColumnDefinition("TENBV", "Tên bản vẽ", 2500);
            Columns[2] = new ColumnDefinition("MADUAN \"-\" MABV", "Ma bản vẽ", 1200);
            Columns[3] = new ColumnDefinition("\"A3\"", "Khổ giấy", 800);
            for (int i = 4; i < 9; i++)
            {
                Columns[i] = new ColumnDefinition("", "", 1000);
            }
        }

        /// <summary>Khôi phục thiết lập mặc định</summary>
        public void ResetToDefault()
        {
            Title = "DANH MỤC BẢN VẼ";
            TextHeight = 100;
            RowHeight = 360;
            AutoColumnWidth = true;
            ShowSumRow = true;
            TotalColumns = 4;
            InitDefaultColumns();
        }
    }
}
