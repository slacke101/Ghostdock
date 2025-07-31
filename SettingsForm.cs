using MaterialSkin;
using MaterialSkin.Controls;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GhostDock
{
    public partial class SettingsForm : MaterialForm
    {
        private readonly SettingsManager settingsManager;
        private readonly AudioSessionManager sessionManager;
        private readonly MaterialSkinManager skinManager;
        private readonly MaterialComboBox[] comboBoxes;
        private Dictionary<int, string> newAssignments;

        public SettingsForm(SettingsManager settingsManager, AudioSessionManager sessionManager, MaterialSkinManager skinManager)
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Enable double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            this.settingsManager = settingsManager;
            this.sessionManager = sessionManager;
            this.skinManager = skinManager;
            this.comboBoxes = new MaterialComboBox[] { comboBox1, comboBox2, comboBox3 };
            newAssignments = new Dictionary<int, string>();

            // Apply MaterialSkin theme
            skinManager.AddFormToManage(this);
            ApplyTheme();

            // Initialize color scheme selector
            comboBoxColorScheme.Items.AddRange(new string[] { "Navy Blue/Gold", "Dark Gray/Silver", "Blue/White" });
            string currentScheme = settingsManager.LoadColorScheme();
            comboBoxColorScheme.SelectedItem = currentScheme ?? "Navy Blue/Gold";

            // Load current assignments
            var assignments = settingsManager.LoadAppAssignments();
            for (int i = 0; i < comboBoxes.Length; i++)
            {
                string assignedApp = assignments.FirstOrDefault(x => x.Value == i).Key;
                if (!string.IsNullOrEmpty(assignedApp))
                {
                    comboBoxes[i].SelectedItem = assignedApp;
                    newAssignments[i] = assignedApp;
                }
            }
            PopulateComboBoxes();
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

        private void ApplyTheme()
        {
            this.BackColor = skinManager.ColorScheme.PrimaryColor;
            comboBoxColorScheme.BackColor = skinManager.ColorScheme.PrimaryColor;
            comboBoxColorScheme.ForeColor = skinManager.ColorScheme.TextColor;
            comboBoxColorScheme.Font = new Font("Roboto", 10);
            btnSave.UseAccentColor = true;
            btnSave.BackColor = skinManager.ColorScheme.PrimaryColor;
            btnSave.ForeColor = skinManager.ColorScheme.TextColor;
            btnCancel.UseAccentColor = true;
            btnCancel.BackColor = skinManager.ColorScheme.PrimaryColor;
            btnCancel.ForeColor = skinManager.ColorScheme.TextColor;
            label1.ForeColor = skinManager.ColorScheme.TextColor;
            label2.ForeColor = skinManager.ColorScheme.TextColor;
            label3.ForeColor = skinManager.ColorScheme.TextColor;
            labelColorScheme.ForeColor = skinManager.ColorScheme.TextColor;

            // Style ComboBoxes
            foreach (var comboBox in comboBoxes)
            {
                comboBox.BackColor = skinManager.ColorScheme.PrimaryColor;
                comboBox.ForeColor = skinManager.ColorScheme.TextColor;
                comboBox.Font = new Font("Roboto", 10);
            }
        }

        private void PopulateComboBoxes()
        {
            var sessions = sessionManager?.Sessions;
            if (sessions == null) return;

            var appNames = new HashSet<string>();
            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session == null || session.IsSystemSoundsSession || session.State == AudioSessionState.AudioSessionStateExpired)
                    continue;

                string processName = session.DisplayName ?? "Unknown";
                try
                {
                    using (var proc = System.Diagnostics.Process.GetProcessById((int)session.GetProcessID))
                    {
                        processName = proc.ProcessName;
                    }
                }
                catch { }
                appNames.Add(processName);
            }

            foreach (var comboBox in comboBoxes)
            {
                comboBox.BeginUpdate();
                try
                {
                    var selected = comboBox.SelectedItem?.ToString();
                    comboBox.Items.Clear();
                    comboBox.Items.Add("");
                    foreach (var app in appNames.OrderBy(x => x))
                    {
                        comboBox.Items.Add(app);
                    }
                    if (!string.IsNullOrEmpty(selected) && appNames.Contains(selected))
                    {
                        comboBox.SelectedItem = selected;
                    }
                }
                finally
                {
                    comboBox.EndUpdate();
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAssignment(0, comboBox1.SelectedItem?.ToString());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAssignment(1, comboBox2.SelectedItem?.ToString());
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAssignment(2, comboBox3.SelectedItem?.ToString());
        }

        private void UpdateAssignment(int index, string appName)
        {
            if (string.IsNullOrEmpty(appName))
            {
                newAssignments.Remove(index);
            }
            else
            {
                newAssignments[index] = appName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            settingsManager.SaveAppAssignments(newAssignments);
            settingsManager.SaveColorScheme(comboBoxColorScheme.SelectedItem?.ToString() ?? "Navy Blue/Gold");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}