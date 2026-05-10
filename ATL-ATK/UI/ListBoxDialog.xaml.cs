using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ATL_ATK.UI
{
    /// <summary>
    /// Hộp thoại chọn từ danh sách.
    /// Tương đương ND:listbox trong LISP.
    /// Hỗ trợ chọn đơn hoặc chọn nhiều.
    /// </summary>
    public partial class ListBoxDialog : Window
    {
        /// <summary>Danh sách mục đã chọn</summary>
        public List<string> SelectedItems { get; private set; } = new List<string>();

        /// <summary>Đã xác nhận (nhấn OK)</summary>
        public bool IsConfirmed { get; private set; }

        /// <summary>
        /// Khởi tạo dialog.
        /// </summary>
        /// <param name="items">Danh sách mục</param>
        /// <param name="title">Tiêu đề dialog</param>
        /// <param name="multiSelect">Cho phép chọn nhiều</param>
        public ListBoxDialog(List<string> items, string title, bool multiSelect = true)
        {
            InitializeComponent();

            Title = title;
            LstItems.SelectionMode = multiSelect
                ? System.Windows.Controls.SelectionMode.Extended
                : System.Windows.Controls.SelectionMode.Single;

            LstItems.ItemsSource = items;

            if (items.Count > 0)
                LstItems.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedItems = LstItems.SelectedItems.Cast<string>().ToList();
            IsConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            BtnCancel_Click(sender, e);
        }
    }
}
