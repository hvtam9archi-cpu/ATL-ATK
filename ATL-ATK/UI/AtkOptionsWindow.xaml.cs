using System.Collections.Generic;
using System.Windows;
using ATL_ATK.Models;
using ATL_ATK.Services;

namespace ATL_ATK.UI
{
    /// <summary>
    /// Code-behind cho AtkOptionsWindow.
    /// Tương đương logic DCL trong ND:ATK_options, ND:ATK2_dcl, ND:ATK2_settile, v.v.
    /// </summary>
    public partial class AtkOptionsWindow : Window
    {
        public AtkSettings Settings { get; private set; }
        public bool IsConfirmed { get; private set; }

        public AtkOptionsWindow(AtkSettings settings)
        {
            InitializeComponent();
            Settings = settings;

            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            // Cách chọn
            RdManual.IsChecked = Settings.SelectManual;
            RdByLayout.IsChecked = Settings.SelectByLayout;
            RdAll.IsChecked = Settings.SelectAll;
            RdOneBlock.IsChecked = Settings.SelectOneBlock;
            RdMultiBlock.IsChecked = Settings.SelectMultiBlock;

            // Sắp xếp
            ChkSortPos.IsChecked = Settings.SortByPosition;
            ChkSortCol.IsChecked = Settings.SortByColumn;

            // Hướng sắp xếp
            CmbSortDir1.ItemsSource = SortingService.SortDirectionLabels;
            CmbSortDir2.ItemsSource = SortingService.SortDirectionLabels;
            CmbSortDir1.SelectedIndex = Settings.SortDirection1;
            CmbSortDir2.SelectedIndex = Settings.SortDirection2;

            TxtSortFuzz.Text = Settings.SortFuzz.ToString();

            // Cột sắp xếp
            var colIndexes = new List<string>();
            for (int i = 1; i <= Settings.TotalColumns; i++)
                colIndexes.Add(i.ToString());
            CmbSortColIdx.ItemsSource = colIndexes;
            if (Settings.SortColumnIndex >= 0 && Settings.SortColumnIndex < Settings.TotalColumns)
                CmbSortColIdx.SelectedIndex = Settings.SortColumnIndex;
            else if (colIndexes.Count > 0)
                CmbSortColIdx.SelectedIndex = 0;

            UpdateSortPanelState();
        }

        private void SaveToSettings()
        {
            Settings.SelectManual = RdManual.IsChecked == true;
            Settings.SelectByLayout = RdByLayout.IsChecked == true;
            Settings.SelectAll = RdAll.IsChecked == true;
            Settings.SelectOneBlock = RdOneBlock.IsChecked == true;
            Settings.SelectMultiBlock = RdMultiBlock.IsChecked == true;

            Settings.SortByPosition = ChkSortPos.IsChecked == true;
            Settings.SortByColumn = ChkSortCol.IsChecked == true;

            Settings.SortDirection1 = CmbSortDir1.SelectedIndex >= 0 ? CmbSortDir1.SelectedIndex : 0;
            Settings.SortDirection2 = CmbSortDir2.SelectedIndex >= 0 ? CmbSortDir2.SelectedIndex : 0;

            if (double.TryParse(TxtSortFuzz.Text, out double fuzz))
                Settings.SortFuzz = fuzz;

            Settings.SortColumnIndex = CmbSortColIdx.SelectedIndex >= 0 ? CmbSortColIdx.SelectedIndex : 0;
        }

        private void UpdateSortPanelState()
        {
            PnlSortPos.IsEnabled = ChkSortPos.IsChecked == true;
            PnlSortCol.IsEnabled = ChkSortCol.IsChecked == true;
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

        private void ChkSortPos_Changed(object sender, RoutedEventArgs e)
        {
            UpdateSortPanelState();
        }

        private void ChkSortCol_Changed(object sender, RoutedEventArgs e)
        {
            UpdateSortPanelState();
        }
    }
}
