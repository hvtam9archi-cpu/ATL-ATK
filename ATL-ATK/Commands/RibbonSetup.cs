using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: ExtensionApplication(typeof(ATL_ATK.Commands.RibbonSetup))]

namespace ATL_ATK.Commands
{
    public class RibbonSetup : IExtensionApplication
    {
        public void Initialize()
        {
            // Initialize Plugin
            try
            {
                // Verify license on load
                if (!Services.LicenseManager.Instance.IsValid)
                {
                    Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("\n[ATL-ATK] License is invalid.");
                    return;
                }

                Application.SystemVariableChanged += OnSystemVariableChanged;
                Autodesk.AutoCAD.ApplicationServices.Application.Idle += OnIdle;
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("\n[ATL-ATK] Plugin initialized successfully.");
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage($"\n[ATL-ATK] Init Error: {ex.Message}");
            }
        }

        private void OnIdle(object sender, EventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.Application.Idle -= OnIdle;
            CreateRibbon();
        }

        private void OnSystemVariableChanged(object sender, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs e)
        {
            if (e.Name.Equals("COLORTHEME", StringComparison.OrdinalIgnoreCase))
            {
                // Theme changed logic if necessary globally
            }
        }

        public void Terminate()
        {
            Application.SystemVariableChanged -= OnSystemVariableChanged;
        }

        private void CreateRibbon()
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon == null) return;

            string tabId = "ATL_ATK_TAB";
            RibbonTab rtab = ribbon.FindTab(tabId);
            if (rtab != null) return;

            rtab = new RibbonTab();
            rtab.Title = "ATL-ATK";
            rtab.Id = tabId;
            ribbon.Tabs.Add(rtab);

            AddPanel(rtab);

            rtab.IsActive = true;
        }

        private void AddPanel(RibbonTab tab)
        {
            RibbonPanelSource panelSrc = new RibbonPanelSource();
            panelSrc.Title = "Thống Kê";

            RibbonPanel panel = new RibbonPanel();
            panel.Source = panelSrc;
            tab.Panels.Add(panel);

            // Create buttons
            RibbonButton btnAtk = new RibbonButton();
            btnAtk.Text = "ATK";
            btnAtk.ShowText = true;
            btnAtk.CommandParameter = "ATK ";
            btnAtk.CommandHandler = new RibbonCommandHandler();

            RibbonButton btnAtl = new RibbonButton();
            btnAtl.Text = "Thiết Lập";
            btnAtl.ShowText = true;
            btnAtl.CommandParameter = "ATL ";
            btnAtl.CommandHandler = new RibbonCommandHandler();

            panelSrc.Items.Add(btnAtk);
            panelSrc.Items.Add(new RibbonRowBreak());
            panelSrc.Items.Add(btnAtl);
        }
    }

    public class RibbonCommandHandler : System.Windows.Input.ICommand
    {
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc != null && parameter is string cmd)
            {
                doc.SendStringToExecute(cmd, true, false, false);
            }
        }
    }
}
