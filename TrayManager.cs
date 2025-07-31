using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostDock
{
    // 3. Class Declaration
    internal class TrayManager
    {
        // 4. Constants (if any)
        private const string AppName = "GhostDock";

        // 5. Fields (private variables)
        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayMenu;
        private Form _mainForm;

        // 6. Properties (getters/setters)
        public bool IsTrayVisible => _trayIcon?.Visible ?? false;

        // 7. Constructors
        public TrayManager(Form mainForm)
        {
            _mainForm = mainForm;
            InitializeTray();
        }

        // 8. Public Methods
        public void ShowTrayIcon()
        {
            _trayIcon.Visible = true;
        }

        public void HideTrayIcon()
        {
            _trayIcon.Visible = false;
        }

        // 9. Private Methods
        private void InitializeTray()
        {
            _trayMenu = new ContextMenuStrip();
            _trayMenu.Items.Add("Show", null, OnShowClick);
            _trayMenu.Items.Add("Exit", null, OnExitClick);

            _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = AppName,
                ContextMenuStrip = _trayMenu,
                Visible = true
            };

            _trayIcon.DoubleClick += OnShowClick;
        }

        private void OnShowClick(object? sender, EventArgs e)
        {
            _mainForm.Show();
            _mainForm.WindowState = FormWindowState.Normal;
            _mainForm.ShowInTaskbar = true;
        }

        private void OnExitClick(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}