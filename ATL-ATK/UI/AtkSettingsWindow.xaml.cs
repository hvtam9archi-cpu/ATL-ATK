using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ATL_ATK.Models;
using ATL_ATK.Services;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ATL_ATK.UI
{
    /// <summary>
    /// ViewModel cho mỗi dòng cột trong ItemsControl.
    /// </summary>
    public class ColumnItemViewModel : INotifyPropertyChanged
    {
        private string _tagExpression;
        private string _title;
        private string _widthText;
        public int Index { get; set; }
        public string Header => $"Cột {Index + 1}: Tag";

        public string TagExpression
        {
            get => _tagExpression;
            set { _tagExpression = value; OnPropertyChanged(nameof(TagExpression)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string WidthText
        {
            get => _widthText;
            set { _widthText = value; OnPropertyChanged(nameof(WidthText)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Code-behind cho AtkSettingsWindow.
    /// Tương đương toàn bộ logic DCL trong C:atl, ND:ATK_settile, ND:ATK_savevar, v.v.
    /// </summary>
    public partial class AtkSettingsWindow : Window
    {
        public AtkSettings Settings { get; private set; }
        public bool IsConfirmed { get; private set; }

        private ObservableCollection<ColumnItemViewModel> _columnViewModels;

        public AtkSettingsWindow(AtkSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            _columnViewModels = new ObservableCollection<ColumnItemViewModel>();

            LoadStyleLists();
            LoadFromSettings();
        }

        // -------------------------------------------------------
        //  LOAD DATA
        // -------------------------------------------------------

        private void LoadStyleLists()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;

            // Table Styles
            List<string> tableStyles = BlockQueryService.GetTableStyleNames(database);
            CmbTableStyle.ItemsSource = tableStyles;

            if (tableStyles.Contains(Settings.TableStyleName))
                CmbTableStyle.SelectedItem = Settings.TableStyleName;
            else if (tableStyles.Count > 0)
                CmbTableStyle.SelectedIndex = 0;

            // Text Styles
            List<string> textStyles = BlockQueryService.GetTextStyleNames(database);
            CmbTextStyle.ItemsSource = textStyles;

            if (textStyles.Contains(Settings.TextStyleName))
                CmbTextStyle.SelectedItem = Settings.TextStyleName;
            else if (textStyles.Count > 0)
                CmbTextStyle.SelectedIndex = 0;
        }

        private void LoadFromSettings()
        {
            // Block name
            TxtBlockName.Text = string.IsNullOrEmpty(Settings.BlockName)
                ? "Nothing selected"
                : Settings.BlockName;

            // Tag list
            LstTags.ItemsSource = Settings.TagList;

            // Thiết lập bảng
            TxtTitle.Text = Settings.Title;
            TxtTextHeight.Text = Settings.TextHeight.ToString();
            TxtRowHeight.Text = Settings.RowHeight.ToString();
            ChkAutoColumn.IsChecked = Settings.AutoColumnWidth;
            ChkShowSum.IsChecked = Settings.ShowSumRow;

            // Nội dung cột
            RefreshColumnItems();
        }

        private void RefreshColumnItems()
        {
            _columnViewModels.Clear();

            for (int i = 0; i < Settings.TotalColumns; i++)
            {
                ColumnDefinition col = Settings.Columns[i];
                _columnViewModels.Add(new ColumnItemViewModel
                {
                    Index = i,
                    TagExpression = col.TagExpression,
                    Title = col.Title,
                    WidthText = col.Width.ToString()
                });
            }

            ColumnItems.ItemsSource = _columnViewModels;
        }

        // -------------------------------------------------------
        //  SAVE TO SETTINGS
        // -------------------------------------------------------

        private void SaveToSettings()
        {
            Settings.TableStyleName = CmbTableStyle.SelectedItem?.ToString() ?? "";
            Settings.TextStyleName = CmbTextStyle.SelectedItem?.ToString() ?? "";
            Settings.Title = TxtTitle.Text;

            if (double.TryParse(TxtTextHeight.Text, out double textHeight) && textHeight > 0)
                Settings.TextHeight = textHeight;

            if (double.TryParse(TxtRowHeight.Text, out double rowHeight) && rowHeight > 0)
                Settings.RowHeight = rowHeight;

            Settings.AutoColumnWidth = ChkAutoColumn.IsChecked == true;
            Settings.ShowSumRow = ChkShowSum.IsChecked == true;

            // Lưu cột
            for (int i = 0; i < _columnViewModels.Count; i++)
            {
                ColumnItemViewModel vm = _columnViewModels[i];
                Settings.Columns[i].TagExpression = vm.TagExpression ?? "";
                Settings.Columns[i].Title = vm.Title ?? "";

                if (double.TryParse(vm.WidthText, out double width) && width > 0)
                    Settings.Columns[i].Width = width;
            }
        }

        // -------------------------------------------------------
        //  EVENT HANDLERS
        // -------------------------------------------------------

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            SaveToSettings();
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

        private void BtnSelectBlock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                Editor editor = document.Editor;
                Database database = document.Database;

                using (EditorUserInteraction interaction = editor.StartUserInteraction(this))
                {

                PromptEntityOptions entityOptions = new PromptEntityOptions("\nSelect Block Att: ");
                entityOptions.SetRejectMessage("\nKhông phải Block INSERT.");
                entityOptions.AddAllowedClass(typeof(BlockReference), false);

                PromptEntityResult entityResult = editor.GetEntity(entityOptions);

                if (entityResult.Status == PromptStatus.OK)
                {
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        BlockReference blockRef = (BlockReference)transaction.GetObject(
                            entityResult.ObjectId, OpenMode.ForRead);

                        string blockName = BlockQueryService.GetBlockName(blockRef);
                        Settings.BlockName = blockName;
                        TxtBlockName.Text = blockName;

                        // Lấy danh sách Tag
                        Settings.TagList = BlockQueryService.GetAttributeTagsFromDefinition(database, blockName);
                        LstTags.ItemsSource = Settings.TagList;

                        // Tự động điền Tag vào các cột (tương đương ND:ATK_selectblock)
                        Settings.Columns[0].TagExpression = "%%STT";
                        for (int i = 0; i < Math.Min(Settings.TagList.Count, 8); i++)
                        {
                            Settings.Columns[i + 1].TagExpression = Settings.TagList[i];
                        }

                        RefreshColumnItems();
                        transaction.Commit();
                    }
                }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi chọn Block: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCopyTag_Click(object sender, RoutedEventArgs e)
        {
            if (LstTags.SelectedItem != null)
            {
                try
                {
                    System.Windows.Clipboard.SetText(LstTags.SelectedItem.ToString());
                }
                catch
                {
                    // Bỏ qua nếu clipboard bận
                }
            }
        }

        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveToSettings();

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Lưu cài đặt",
                Filter = "Text files (*.txt)|*.txt",
                FileName = "Thong ke Block Att, Dynamic - Settings.txt"
            };

            if (!string.IsNullOrEmpty(Settings.SettingsFilePath))
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(Settings.SettingsFilePath);

            if (dialog.ShowDialog() == true)
            {
                Settings.SettingsFilePath = dialog.FileName;
                SettingsIOService.SaveSettings(Settings, dialog.FileName);
            }
        }

        private void BtnLoadSettings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Đọc cài đặt",
                Filter = "Text files (*.txt)|*.txt"
            };

            if (!string.IsNullOrEmpty(Settings.SettingsFilePath))
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(Settings.SettingsFilePath);

            if (dialog.ShowDialog() == true)
            {
                Settings = SettingsIOService.LoadSettings(dialog.FileName);
                Settings.SettingsFilePath = dialog.FileName;

                // Validate
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;

                if (!string.IsNullOrEmpty(Settings.BlockName))
                {
                    var tags = BlockQueryService.GetAttributeTagsFromDefinition(database, Settings.BlockName);
                    if (tags.Count == 0)
                    {
                        MessageBox.Show($"Không tồn tại Block: {Settings.BlockName}",
                            "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        Settings.TagList = tags;
                    }
                }

                LoadStyleLists();
                LoadFromSettings();
            }
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            Settings.ResetToDefault();
            LoadFromSettings();
        }

        private void BtnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.TotalColumns < 9)
            {
                SaveToSettings();
                Settings.TotalColumns++;
                RefreshColumnItems();
            }
        }

        private void BtnRemoveColumn_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.TotalColumns > 1)
            {
                SaveToSettings();
                Settings.TotalColumns--;
                RefreshColumnItems();
            }
        }

        private void BtnTips_Click(object sender, RoutedEventArgs e)
        {
            string tips =
                "Hướng dẫn cách nhập Tag:\n" +
                "\n   - Nhập tên Tag hoặc Copy từ danh sách Tag\n" +
                "\n   - Nhập %%STT để định nghĩa số thứ tự tăng dần\n" +
                "\n   - Nhập %%NAME để định nghĩa tên Block\n" +
                "   - Nhập %%BLK để tạo hình ảnh Block\n" +
                "\n   - Nhập %%X, %%Y, %%Z để định nghĩa tọa độ X, Y, Z của Block\n" +
                "\n   - Nhập %%+, %%-, %%*, %%/ để thực hiện phép tính từ trái qua phải\n" +
                "   (không phân biệt, không ưu tiên * / trước + - sau)\n" +
                "   - Nhập %%sothuc để định nghĩa số thực\n" +
                "   Ví dụ phép tính với cả Tag (biến) và số thực (hằng số):\n" +
                "   TAG  %%+  %%2  %%*  %%3.5, tương đương: (TAG+2)*3.5\n" +
                "\n   - Nhập \"noidung\" để định nghĩa các nội dung cố định\n" +
                "   - Để kết hợp nhiều Tag trong 1 cột, đặt dấu cách giữa các tên Tag\n" +
                "   Ví dụ: TAG1 TAG2 hoặc MADUAN \" - \" MABANVE";

            MessageBox.Show(tips, "Hướng dẫn nhập Tag", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            string info =
                "LISP Thống kê Block Att, Dynamic (AutoCAD Table) v1.06\n" +
                "Tác giả: 3Duy\n" +
                "Tên lệnh:\n" +
                "     ATL - Thiết lập\n" +
                "     ATK - Thống kê chi tiết Block Att\n" +
                "     AT1 - Thống kê nhanh Block Att\n" +
                "     DY1 - Thống kê nhanh Block Dynamic\n" +
                "     ATDY1 - Thống kê nhanh Block Att-Dynamic\n" +
                "     ATC - Đếm số lượng Att (Block Att)\n" +
                "     DYC - Đếm số lượng Parameter (Block Dynamic)\n" +
                "     ATDYC - Đếm số lượng Att và Parameter (Block Att-Dynamic)\n" +
                "     BLC - Đếm số lượng Block\n" +
                "     NDC - Đếm nội dung Att, Text\n" +
                "     ?? - Bảng tên lệnh";

            MessageBox.Show(info, "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
