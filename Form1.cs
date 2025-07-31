using MaterialSkin;
using MaterialSkin.Controls;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GhostDock
{
    public partial class Form1 : MaterialForm
    {
        private MMDeviceEnumerator enumerator;
        private AudioSessionManager sessionManager;
        private string currentDeviceId;
        private MaterialLabel[] appLabels;
        private Dictionary<string, string> processIconKeys;
        private SettingsManager settingsManager;
        private ToolTip toolTip;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Timer animationTimer;
        private static Dictionary<string, Image> iconCache = new Dictionary<string, Image>();
        private PrivateFontCollection fontCollection;
        private double opacityIncrement = 0.05;
        private bool isFadingIn;
        private MaterialSkinManager skinManager;
        private DateTime lastRefresh = DateTime.MinValue;
        private readonly TimeSpan refreshDebounce = TimeSpan.FromSeconds(2);

        public Form1()
        {
            InitializeComponent();
            // Enable double buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            appLabels = new MaterialLabel[] { labelApp1, labelApp2, labelApp3 };
            processIconKeys = new Dictionary<string, string>();
            settingsManager = new SettingsManager();
            toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 500, ReshowDelay = 500 };

            // Initialize custom font
            fontCollection = new PrivateFontCollection();
            try
            {
                string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Orbitron-Bold.ttf");
                fontCollection.AddFontFile(fontPath);
                labelHeader.Font = new Font(fontCollection.Families[0], 20, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                LogError($"Failed to load custom font: {ex.Message}");
                labelHeader.Font = new Font("Roboto", 20, FontStyle.Bold);
            }

            // Initialize MaterialSkinManager
            skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.DARK;

            // Load saved color scheme or default to Navy Blue/Gold
            string colorScheme = settingsManager.LoadColorScheme();
            ApplyColorScheme(colorScheme);

            enumerator = new MMDeviceEnumerator();
            InitializeAudioDevice();
            SetupTrayIcon();
            this.Opacity = 0;
            this.Visible = false;
            this.ShowInTaskbar = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        private void ApplyColorScheme(string schemeName)
        {
            switch (schemeName)
            {
                case "Dark Gray/Silver":
                    skinManager.ColorScheme = new ColorScheme(
                        Primary.Grey900,
                        Primary.Grey800,
                        Primary.Grey500,
                        Accent.LightBlue200,
                        TextShade.WHITE);
                    break;
                case "Blue/White":
                    skinManager.ColorScheme = new ColorScheme(
                        Primary.Blue900,
                        Primary.Blue800,
                        Primary.Blue500,
                        Accent.Blue100,
                        TextShade.WHITE);
                    break;
                case "Navy Blue/Gold":
                default:
                    skinManager.ColorScheme = new ColorScheme(
                        Primary.BlueGrey900,
                        Primary.BlueGrey800,
                        Primary.BlueGrey500,
                        Accent.Amber400,
                        TextShade.WHITE);
                    break;
            }
            this.BackColor = skinManager.ColorScheme.PrimaryColor;
            panelAudioControls.BackColor = skinManager.ColorScheme.PrimaryColor;
            listBoxApps.BackColor = skinManager.ColorScheme.PrimaryColor;
            listBoxApps.ForeColor = skinManager.ColorScheme.TextColor;
            labelHeader.ForeColor = skinManager.ColorScheme.AccentColor;
            borderPanel.BackColor = skinManager.ColorScheme.AccentColor;
            foreach (var label in appLabels)
                label.ForeColor = skinManager.ColorScheme.TextColor;
        }

        private void InitializeAudioDevice()
        {
            try
            {
                var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                sessionManager = device.AudioSessionManager;
                currentDeviceId = device.ID;
                sessionManager.OnSessionCreated += (s, e) => LoadAudioSessions();
                LogError($"Initialized audio device: {device.FriendlyName} (ID: {currentDeviceId})");
            }
            catch (Exception ex)
            {
                LogError($"Error initializing audio device: {ex.Message}");
            }

            refreshTimer = new System.Windows.Forms.Timer { Interval = 10000 }; // 10s interval
            refreshTimer.Tick += (s, e) => RefreshAudioState();
            refreshTimer.Start();

            animationTimer = new System.Windows.Forms.Timer { Interval = 20 };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void SetupTrayIcon()
        {
            try
            {
                NotifyIcon.Icon = new Icon("Resources\\GhostDock.ico");
            }
            catch (Exception ex)
            {
                LogError($"Failed to load GhostDock icon: {ex.Message}");
                NotifyIcon.Icon = System.Drawing.SystemIcons.Application;
            }

            trayMenu.Items.Clear();
            trayMenu.Items.Add("Restore", null, (s, e) => RestoreForm());
            trayMenu.Items.Add("-");
            UpdateTrayMenuApps();
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Settings", null, Settings_Click);
            trayMenu.Items.Add("Exit", null, Exit_Click);

            NotifyIcon.DoubleClick += TrayIcon_DoubleClick;
            NotifyIcon.Visible = true;
        }

        private void UpdateTrayMenuApps()
        {
            for (int i = trayMenu.Items.Count - 1; i >= 0; i--)
            {
                if (trayMenu.Items[i].Tag?.ToString() == "AppControl")
                {
                    trayMenu.Items.RemoveAt(i);
                }
            }

            int insertIndex = 2;
            for (int i = 0; i < listBoxApps.Items.Count; i++)
            {
                string appName = listBoxApps.Items[i].ToString();
                var sliders = panelAudioControls.Controls.Find($"slider_{i}", true);
                if (sliders.Length == 0 || !(sliders[0].Tag is AudioSessionControl session)) continue;
                bool isMuted = session.SimpleAudioVolume.Mute;
                int processId = (int)session.GetProcessID;
                ToolStripMenuItem muteItem = new ToolStripMenuItem($"{(isMuted ? "Unmute" : "Mute")} {appName} (PID: {processId})")
                {
                    Tag = "AppControl",
                    Name = $"muteItem_{appName}"
                };
                if (processIconKeys.ContainsKey(appName) && imageList1.Images.ContainsKey(processIconKeys[appName]))
                {
                    try
                    {
                        muteItem.Image = imageList1.Images[processIconKeys[appName]];
                    }
                    catch (Exception ex)
                    {
                        LogError($"Failed to set menu icon for {appName}: {ex.Message}");
                    }
                }
                muteItem.Click += (s, e) => MuteAppFromTray(appName, session);
                trayMenu.Items.Insert(insertIndex++, muteItem);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (sessionManager == null)
                {
                    InitializeAudioDevice();
                }
                RestoreForm();
                LoadAudioSessions();
                NotifyIcon.ShowBalloonTip(3000, "GhostDock", "Running in system tray", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                LogError($"Error initializing audio sessions: {ex.Message}");
                MessageBox.Show($"Error initializing audio sessions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshAudioState()
        {
            if ((DateTime.Now - lastRefresh) < refreshDebounce)
                return;

            lastRefresh = DateTime.Now;
            try
            {
                var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                if (device.ID != currentDeviceId)
                {
                    sessionManager = device.AudioSessionManager;
                    currentDeviceId = device.ID;
                    LogError($"Default audio device changed to {device.FriendlyName} (ID: {currentDeviceId})");
                    LoadAudioSessions();
                }
                else
                {
                    LoadAudioSessionsIfChanged();
                }
            }
            catch (Exception ex)
            {
                LogError($"Error checking audio device: {ex.Message}");
                MessageBox.Show($"Audio device error. Please restart GhostDock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadAudioSessionsIfChanged()
        {
            var sessions = sessionManager?.Sessions;
            if (sessions == null) return;

            var currentPids = new HashSet<int>();
            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session != null && !session.IsSystemSoundsSession && session.State != AudioSessionState.AudioSessionStateExpired)
                    currentPids.Add((int)session.GetProcessID);
            }

            var displayedPids = new HashSet<int>();
            foreach (var control in panelAudioControls.Controls)
            {
                if (control is TrackBar slider && slider.Tag is AudioSessionControl session)
                    displayedPids.Add((int)session.GetProcessID);
            }

            if (!currentPids.SetEquals(displayedPids))
                LoadAudioSessions();
        }

        private void LoadAudioSessions()
        {
            panelAudioControls.SuspendLayout();
            listBoxApps.SuspendLayout();
            try
            {
                var sessions = sessionManager?.Sessions;
                if (sessions == null || sessions.Count == 0)
                {
                    if (panelAudioControls.Controls.Count == 0)
                    {
                        var noAppsLabel = new MaterialLabel
                        {
                            Text = "No audio apps detected",
                            AutoSize = true,
                            Location = new Point(16, 16),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.Body1,
                            ForeColor = skinManager.ColorScheme.TextColor
                        };
                        panelAudioControls.Controls.Add(noAppsLabel);
                    }
                    listBoxApps.Items.Clear();
                    imageList1.Images.Clear();
                    processIconKeys.Clear();
                    UpdateTrayMenuApps();
                    LogError("No audio sessions available.");
                    return;
                }

                LogError($"Detected {sessions.Count} audio sessions.");
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    if (session == null) continue;
                    int pid = (int)session.GetProcessID;
                    string name = session.DisplayName ?? "Unknown";
                    try
                    {
                        using (var proc = Process.GetProcessById(pid))
                        {
                            name = proc.ProcessName;
                        }
                    }
                    catch { }
                    LogError($"Session {i}: PID={pid}, Name={name}, State={session.State}, IsSystem={session.IsSystemSoundsSession}");
                }

                var sessionsByPid = new Dictionary<int, AudioSessionControl>();
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    if (session == null || session.IsSystemSoundsSession || session.State == AudioSessionState.AudioSessionStateExpired)
                        continue;
                    int pid = (int)session.GetProcessID;
                    if (!sessionsByPid.ContainsKey(pid))
                        sessionsByPid[pid] = session;
                }

                var assignments = settingsManager.LoadAppAssignments();
                var assignedSessions = new Dictionary<int, AudioSessionControl>();
                var unassignedPids = new List<int>(sessionsByPid.Keys);

                foreach (var assignment in assignments)
                {
                    string processName = assignment.Key;
                    int sliderIndex = assignment.Value;
                    if (sliderIndex < 0 || sliderIndex >= appLabels.Length)
                        continue;

                    var session = unassignedPids.Select(pid => sessionsByPid[pid]).FirstOrDefault(s =>
                    {
                        try
                        {
                            using (var proc = Process.GetProcessById((int)s.GetProcessID))
                            {
                                return proc.ProcessName == processName;
                            }
                        }
                        catch
                        {
                            return s.DisplayName == processName;
                        }
                    });

                    if (session != null && !assignedSessions.ContainsKey(sliderIndex))
                    {
                        assignedSessions[sliderIndex] = session;
                        unassignedPids.Remove((int)session.GetProcessID);
                    }
                }

                int nextSliderIndex = 0;
                foreach (int pid in unassignedPids)
                {
                    while (assignedSessions.ContainsKey(nextSliderIndex) && nextSliderIndex < appLabels.Length)
                        nextSliderIndex++;
                    if (nextSliderIndex < appLabels.Length)
                    {
                        assignedSessions[nextSliderIndex] = sessionsByPid[pid];
                        nextSliderIndex++;
                    }
                }

                int y = 16;
                for (int index = 0; index < appLabels.Length; index++)
                {
                    var existingSlider = panelAudioControls.Controls.Find($"slider_{index}", true).FirstOrDefault() as TrackBar;
                    var existingMuteButton = panelAudioControls.Controls.Find($"muteButton_{index}", true).FirstOrDefault() as MaterialButton;
                    var existingSeparator = index > 0 ? panelAudioControls.Controls.Find($"separator_{index}", true).FirstOrDefault() as Panel : null;

                    if (!assignedSessions.ContainsKey(index))
                    {
                        if (existingSlider != null) panelAudioControls.Controls.Remove(existingSlider);
                        if (existingMuteButton != null) panelAudioControls.Controls.Remove(existingMuteButton);
                        if (existingSeparator != null) panelAudioControls.Controls.Remove(existingSeparator);
                        appLabels[index].Text = string.Empty;
                        if (listBoxApps.Items.Count > index) listBoxApps.Items[index] = string.Empty;
                        continue;
                    }

                    var session = assignedSessions[index];
                    string processName = "Unknown";
                    Icon processIcon = null;
                    int processId = (int)session.GetProcessID;
                    try
                    {
                        using (var proc = Process.GetProcessById(processId))
                        {
                            processName = proc.ProcessName;
                            string exePath = proc.MainModule.FileName;
                            processIcon = Icon.ExtractAssociatedIcon(exePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        processName = session.DisplayName ?? $"Session {index + 1}";
                        LogError($"Failed to get process info for session {index} (PID: {processId}): {ex.Message}");
                    }

                    string iconKey = $"icon_{processName}_{processId}";
                    if (processIcon != null && !iconCache.ContainsKey(iconKey))
                    {
                        try
                        {
                            var bitmap = processIcon.ToBitmap();
                            imageList1.Images.Add(iconKey, bitmap);
                            iconCache[iconKey] = bitmap;
                            processIconKeys[processName] = iconKey;
                        }
                        catch (Exception ex)
                        {
                            LogError($"Failed to add icon for {processName}: {ex.Message}");
                            string defaultIconKey = "default_icon";
                            if (!imageList1.Images.ContainsKey(defaultIconKey))
                            {
                                var defaultIcon = System.Drawing.SystemIcons.Application.ToBitmap();
                                imageList1.Images.Add(defaultIconKey, defaultIcon);
                                iconCache[defaultIconKey] = defaultIcon;
                            }
                            processIconKeys[processName] = defaultIconKey;
                        }
                        processIcon?.Dispose();
                    }
                    else if (processIcon != null && !imageList1.Images.ContainsKey(iconKey))
                    {
                        imageList1.Images.Add(iconKey, iconCache[iconKey]);
                        processIconKeys[processName] = iconKey;
                    }

                    if (listBoxApps.Items.Count <= index)
                        listBoxApps.Items.Add(processName);
                    else
                        listBoxApps.Items[index] = processName;

                    appLabels[index].Text = processName;
                    appLabels[index].Location = new Point(16, y);
                    appLabels[index].FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1;
                    appLabels[index].ForeColor = skinManager.ColorScheme.TextColor;

                    if (existingSlider == null)
                    {
                        existingSlider = new TrackBar
                        {
                            Minimum = 0,
                            Maximum = 100,
                            TickStyle = TickStyle.None,
                            Size = new Size(180, 30),
                            Location = new Point(16, y + 24),
                            Name = $"slider_{index}",
                            BackColor = skinManager.ColorScheme.PrimaryColor,
                            ForeColor = skinManager.ColorScheme.AccentColor
                        };
                        existingSlider.Scroll += Slider_Scroll;
                        existingSlider.MouseEnter += (s, e) => existingSlider.BackColor = skinManager.ColorScheme.DarkPrimaryColor;
                        existingSlider.MouseLeave += (s, e) => existingSlider.BackColor = skinManager.ColorScheme.PrimaryColor;
                        panelAudioControls.Controls.Add(existingSlider);
                    }
                    existingSlider.Value = (int)(session.SimpleAudioVolume.Volume * 100);
                    existingSlider.Tag = session;
                    toolTip.SetToolTip(existingSlider, $"Adjust volume for {processName}");

                    if (existingMuteButton == null)
                    {
                        existingMuteButton = new MaterialButton
                        {
                            Text = session.SimpleAudioVolume.Mute ? "Unmute" : "Mute",
                            Size = new Size(80, 36),
                            Location = new Point(210, y + 24),
                            Name = $"muteButton_{index}",
                            Type = MaterialButton.MaterialButtonType.Contained,
                            UseAccentColor = true
                        };
                        existingMuteButton.Click += MuteButton_Click;
                        panelAudioControls.Controls.Add(existingMuteButton);
                    }
                    existingMuteButton.Text = session.SimpleAudioVolume.Mute ? "Unmute" : "Mute";
                    existingMuteButton.Tag = session;
                    toolTip.SetToolTip(existingMuteButton, session.SimpleAudioVolume.Mute ? $"Unmute {processName}" : $"Mute {processName}");

                    if (index > 0 && existingSeparator == null)
                    {
                        existingSeparator = new Panel
                        {
                            Size = new Size(300, 1),
                            Location = new Point(16, y - 5),
                            BackColor = skinManager.ColorScheme.TextColor,
                            Name = $"separator_{index}"
                        };
                        panelAudioControls.Controls.Add(existingSeparator);
                    }

                    y += 70;
                }

                // Clear excess controls and list items
                for (int i = appLabels.Length; i < panelAudioControls.Controls.Count; i++)
                {
                    var control = panelAudioControls.Controls.Find($"slider_{i}", true).FirstOrDefault();
                    if (control != null) panelAudioControls.Controls.Remove(control);
                    control = panelAudioControls.Controls.Find($"muteButton_{i}", true).FirstOrDefault();
                    if (control != null) panelAudioControls.Controls.Remove(control);
                    control = panelAudioControls.Controls.Find($"separator_{i}", true).FirstOrDefault();
                    if (control != null) panelAudioControls.Controls.Remove(control);
                }
                while (listBoxApps.Items.Count > appLabels.Length)
                {
                    listBoxApps.Items.RemoveAt(listBoxApps.Items.Count - 1);
                }

                UpdateTrayMenuApps();
            }
            finally
            {
                panelAudioControls.ResumeLayout(true);
                listBoxApps.ResumeLayout(true);
            }
        }

        private void listBoxApps_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= listBoxApps.Items.Count) return;

            string appName = listBoxApps.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Brush bgBrush = isSelected ? new SolidBrush(Color.FromArgb(100, skinManager.ColorScheme.AccentColor)) : new SolidBrush(skinManager.ColorScheme.PrimaryColor);
            Brush textBrush = isSelected ? Brushes.White : new SolidBrush(skinManager.ColorScheme.TextColor);

            e.Graphics.FillRectangle(bgBrush, e.Bounds);

            int iconX = e.Bounds.Left + 8;
            int textX = e.Bounds.Left + 32;
            if (processIconKeys.ContainsKey(appName) && imageList1.Images.ContainsKey(processIconKeys[appName]))
            {
                e.Graphics.DrawImage(imageList1.Images[processIconKeys[appName]], iconX, e.Bounds.Top + 4, 16, 16);
            }

            var sliders = panelAudioControls.Controls.Find($"slider_{e.Index}", true);
            if (sliders.Length > 0 && sliders[0]?.Tag is AudioSessionControl session)
            {
                int processId = (int)session.GetProcessID;
                e.Graphics.DrawString($"{appName} (PID: {processId})", new Font("Roboto", 10), textBrush, textX, e.Bounds.Top + 4);
            }
            else
            {
                e.Graphics.DrawString($"{appName}", new Font("Roboto", 10), textBrush, textX, e.Bounds.Top + 4);
            }

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }
        }

        private void Slider_Scroll(object sender, EventArgs e)
        {
            if (sender is TrackBar slider && slider.Tag is AudioSessionControl session)
            {
                session.SimpleAudioVolume.Volume = slider.Value / 100f;
            }
        }

        private void MuteButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is AudioSessionControl session)
            {
                bool isMuted = session.SimpleAudioVolume.Mute;
                session.SimpleAudioVolume.Mute = !isMuted;
                button.Text = isMuted ? "Mute" : "Unmute";
                toolTip.SetToolTip(button, isMuted ? $"Mute {button.Text}" : $"Unmute {button.Text}");
                UpdateTrayMenuApps();
            }
        }

        private void MuteAppFromTray(string appName, AudioSessionControl session)
        {
            bool isMuted = session.SimpleAudioVolume.Mute;
            session.SimpleAudioVolume.Mute = !isMuted;
            foreach (Control control in panelAudioControls.Controls)
            {
                if (control is Button button && button.Tag == session)
                {
                    button.Text = isMuted ? "Mute" : "Unmute";
                    toolTip.SetToolTip(button, isMuted ? $"Mute {appName}" : $"Unmute {appName}");
                    break;
                }
            }
            UpdateTrayMenuApps();
        }

        private void ListBoxApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxApps.SelectedIndex;
            if (index >= 0 && index < appLabels.Length)
            {
                var sliders = panelAudioControls.Controls.Find($"slider_{index}", true);
                if (sliders.Length > 0)
                {
                    sliders[0].Focus();
                }
                UpdateTrayMenuApps();
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            RestoreForm();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isFadingIn)
            {
                if (this.Opacity < 1.0)
                {
                    this.Opacity += opacityIncrement;
                }
                else
                {
                    this.Opacity = 1.0;
                    animationTimer.Stop();
                }
            }
            else
            {
                if (this.Opacity > 0.0)
                {
                    this.Opacity -= opacityIncrement;
                }
                else
                {
                    this.Opacity = 0.0;
                    this.Visible = false;
                    this.ShowInTaskbar = false;
                    animationTimer.Stop();
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                isFadingIn = false;
                animationTimer.Start();
                NotifyIcon.Visible = true;
                NotifyIcon.ShowBalloonTip(3000, "GhostDock", "Minimized to system tray", ToolTipIcon.Info);
            }
        }

        private void RestoreForm()
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            int x = workingArea.Right - this.Width - 10;
            int y = workingArea.Bottom - this.Height - 10;
            this.Location = new Point(x, y);

            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            isFadingIn = true;
            this.Opacity = 0;
            animationTimer.Start();
            this.BringToFront();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            try
            {
                using (var settingsForm = new SettingsForm(settingsManager, sessionManager, skinManager))
                {
                    settingsForm.ShowDialog();
                    if (settingsForm.DialogResult == DialogResult.OK)
                    {
                        string newColorScheme = settingsManager.LoadColorScheme();
                        ApplyColorScheme(newColorScheme);
                        LoadAudioSessions();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to open SettingsForm: {ex.Message}");
                MessageBox.Show($"Error opening settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeAudioDevice();
                LoadAudioSessions();
                LogError("Manual refresh triggered.");
            }
            catch (Exception ex)
            {
                LogError($"Error during manual refresh: {ex.Message}");
                MessageBox.Show($"Error refreshing audio sessions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
            animationTimer?.Stop();
            animationTimer?.Dispose();
            refreshTimer?.Stop();
            refreshTimer?.Dispose();
            imageList1.Images.Clear();
            fontCollection?.Dispose();
            enumerator?.Dispose();
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                isFadingIn = false;
                animationTimer.Start();
                NotifyIcon.Visible = true;
                NotifyIcon.ShowBalloonTip(3000, "GhostDock", "Minimized to system tray", ToolTipIcon.Info);
            }
            else
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
                animationTimer?.Stop();
                animationTimer?.Dispose();
                refreshTimer?.Stop();
                refreshTimer?.Dispose();
                imageList1.Images.Clear();
                fontCollection?.Dispose();
                enumerator?.Dispose();
            }
        }

        private void LogError(string message)
        {
            Debug.WriteLine(message);
            try
            {
                File.AppendAllText("GhostDock.log", $"{DateTime.Now}: {message}\n");
            }
            catch { }
        }
    }
}