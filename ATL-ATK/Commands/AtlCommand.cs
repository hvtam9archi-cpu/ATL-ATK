using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using ATL_ATK.Models;
using ATL_ATK.Services;
using ATL_ATK.UI;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ATL_ATK.Commands
{
    /// <summary>
    /// Lệnh ATL - Mở cửa sổ thiết lập.
    /// Tương đương C:atl trong LISP.
    /// </summary>
    public class AtlCommand
    {
        /// <summary>Thiết lập toàn cục (singleton, giữ giữa các lần gọi)</summary>
        internal static AtkSettings GlobalSettings = new AtkSettings();

        /// <summary>Đánh dấu đã khởi tạo style mặc định chưa</summary>
        private static bool _initialized = false;

        /// <summary>
        /// Khởi tạo giá trị mặc định từ bản vẽ hiện hành.
        /// Gọi lazy khi lệnh đầu tiên được thực thi (không dùng static constructor).
        /// </summary>
        internal static void EnsureInitialized()
        {
            if (_initialized)
                return;

            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                if (document != null)
                {
                    var tableStyles = BlockQueryService.GetTableStyleNames(document.Database);
                    if (tableStyles.Count > 0)
                        GlobalSettings.TableStyleName = tableStyles[0];

                    var textStyles = BlockQueryService.GetTextStyleNames(document.Database);
                    if (textStyles.Count > 0)
                        GlobalSettings.TextStyleName = textStyles[0];
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ATL-ATK] EnsureInitialized: {ex.Message}");
            }

            _initialized = true;
        }

        [CommandMethod("ATL")]
        public void Execute()
        {
            try
            {
                EnsureInitialized();
                Document document = Application.DocumentManager.MdiActiveDocument;
                if (document == null) return;

                // Hiển thị cửa sổ thiết lập
                var window = new AtkSettingsWindow(GlobalSettings);
                Application.ShowModalWindow(window);

                if (window.IsConfirmed)
                {
                    GlobalSettings = window.Settings;
                    document.Editor.WriteMessage("\nĐã lưu thiết lập.");
                }
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?.Editor
                    .WriteMessage($"\nLỗi lệnh ATL: {ex.Message}");
            }
        }

        /// <summary>
        /// Lệnh ATKHELP - Hiển thị bảng tên lệnh.
        /// </summary>
        [CommandMethod("ATKHELP")]
        public void ShowHelp()
        {
            try
            {
                string info =
                    "\nPlugin Thống kê Block Att, Dynamic (AutoCAD Table) v1.06 - C# Edition" +
                    "\nTác giả: 3Duy" +
                    "\nTên lệnh:" +
                    "\n     ATL      - Thiết lập" +
                    "\n     ATK      - Thống kê chi tiết Block Att" +
                    "\n     AT1      - Thống kê nhanh Block Att" +
                    "\n     DY1      - Thống kê nhanh Block Dynamic" +
                    "\n     ATDY1    - Thống kê nhanh Block Att-Dynamic" +
                    "\n     ATC      - Đếm số lượng Att (Block Att)" +
                    "\n     DYC      - Đếm số lượng Parameter (Block Dynamic)" +
                    "\n     ATDYC    - Đếm số lượng Att và Parameter (Block Att-Dynamic)" +
                    "\n     BLC      - Đếm số lượng Block" +
                    "\n     NDC      - Đếm nội dung Att, Text" +
                    "\n     ATKHELP  - Bảng tên lệnh";

                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage(info);
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?.Editor
                    .WriteMessage($"\nLỗi: {ex.Message}");
            }
        }
    }
}
