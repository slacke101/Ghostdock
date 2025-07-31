namespace GhostDock
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ListBox listBoxApps;
        private MaterialSkin.Controls.MaterialCard panelAudioControls;
        private System.Windows.Forms.Panel borderPanel;
        private MaterialSkin.Controls.MaterialButton btnSettings;
        private MaterialSkin.Controls.MaterialButton btnRefresh;
        private MaterialSkin.Controls.MaterialLabel labelApp1;
        private MaterialSkin.Controls.MaterialLabel labelApp2;
        private MaterialSkin.Controls.MaterialLabel labelApp3;
        private MaterialSkin.Controls.MaterialLabel labelHeader;
        private System.Windows.Forms.ImageList imageList1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            imageList1 = new ImageList(components);
            listBoxApps = new ListBox();
            panelAudioControls = new MaterialSkin.Controls.MaterialCard();
            labelApp1 = new MaterialSkin.Controls.MaterialLabel();
            labelApp2 = new MaterialSkin.Controls.MaterialLabel();
            labelApp3 = new MaterialSkin.Controls.MaterialLabel();
            borderPanel = new Panel();
            trayMenu = new ContextMenuStrip(components);
            NotifyIcon = new NotifyIcon(components);
            btnSettings = new MaterialSkin.Controls.MaterialButton();
            btnRefresh = new MaterialSkin.Controls.MaterialButton();
            labelHeader = new MaterialSkin.Controls.MaterialLabel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panelAudioControls.SuspendLayout();
            borderPanel.SuspendLayout();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // listBoxApps
            // 
            listBoxApps.BackColor = Color.FromArgb(28, 37, 38);
            listBoxApps.BorderStyle = BorderStyle.None;
            listBoxApps.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxApps.ForeColor = Color.FromArgb(224, 224, 224);
            listBoxApps.ItemHeight = 24;
            listBoxApps.Location = new Point(16, 80);
            listBoxApps.Name = "listBoxApps";
            listBoxApps.Size = new Size(348, 96);
            listBoxApps.TabIndex = 0;
            listBoxApps.DrawItem += listBoxApps_DrawItem;
            listBoxApps.SelectedIndexChanged += ListBoxApps_SelectedIndexChanged;
            // 
            // panelAudioControls
            // 
            panelAudioControls.BackColor = Color.FromArgb(255, 255, 255);
            panelAudioControls.Controls.Add(labelApp1);
            panelAudioControls.Controls.Add(labelApp2);
            panelAudioControls.Controls.Add(labelApp3);
            panelAudioControls.Depth = 0;
            panelAudioControls.ForeColor = Color.FromArgb(222, 0, 0, 0);
            panelAudioControls.Location = new Point(2, 2);
            panelAudioControls.Margin = new Padding(14);
            panelAudioControls.MouseState = MaterialSkin.MouseState.HOVER;
            panelAudioControls.Name = "panelAudioControls";
            panelAudioControls.Padding = new Padding(16);
            panelAudioControls.Size = new Size(348, 240);
            panelAudioControls.TabIndex = 2;
            // 
            // labelApp1
            // 
            labelApp1.AutoSize = true;
            labelApp1.Depth = 0;
            labelApp1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            labelApp1.ForeColor = Color.FromArgb(224, 224, 224);
            labelApp1.Location = new Point(16, 10);
            labelApp1.MouseState = MaterialSkin.MouseState.HOVER;
            labelApp1.Name = "labelApp1";
            labelApp1.Size = new Size(42, 19);
            labelApp1.TabIndex = 3;
            labelApp1.Text = "App 1";
            // 
            // labelApp2
            // 
            labelApp2.AutoSize = true;
            labelApp2.Depth = 0;
            labelApp2.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            labelApp2.ForeColor = Color.FromArgb(224, 224, 224);
            labelApp2.Location = new Point(16, 80);
            labelApp2.MouseState = MaterialSkin.MouseState.HOVER;
            labelApp2.Name = "labelApp2";
            labelApp2.Size = new Size(42, 19);
            labelApp2.TabIndex = 4;
            labelApp2.Text = "App 2";
            // 
            // labelApp3
            // 
            labelApp3.AutoSize = true;
            labelApp3.Depth = 0;
            labelApp3.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            labelApp3.ForeColor = Color.FromArgb(224, 224, 224);
            labelApp3.Location = new Point(16, 150);
            labelApp3.MouseState = MaterialSkin.MouseState.HOVER;
            labelApp3.Name = "labelApp3";
            labelApp3.Size = new Size(42, 19);
            labelApp3.TabIndex = 5;
            labelApp3.Text = "App 3";
            // 
            // borderPanel
            // 
            borderPanel.BackColor = Color.FromArgb(212, 160, 23);
            borderPanel.Controls.Add(panelAudioControls);
            borderPanel.Location = new Point(14, 188);
            borderPanel.Name = "borderPanel";
            borderPanel.Size = new Size(352, 244);
            borderPanel.TabIndex = 1;
            // 
            // trayMenu
            // 
            trayMenu.Name = "trayMenu";
            trayMenu.Size = new Size(61, 4);
            // 
            // NotifyIcon
            // 
            NotifyIcon.Icon = (Icon)resources.GetObject("NotifyIcon.Icon");
            NotifyIcon.Text = "GhostDock";
            NotifyIcon.Visible = true;
            // 
            // btnSettings
            // 
            btnSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSettings.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnSettings.Depth = 0;
            btnSettings.HighEmphasis = true;
            btnSettings.Icon = null;
            btnSettings.Location = new Point(284, 440);
            btnSettings.Margin = new Padding(4, 6, 4, 6);
            btnSettings.MouseState = MaterialSkin.MouseState.HOVER;
            btnSettings.Name = "btnSettings";
            btnSettings.NoAccentTextColor = Color.Empty;
            btnSettings.Size = new Size(90, 36);
            btnSettings.TabIndex = 6;
            btnSettings.Text = "Settings";
            btnSettings.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnSettings.UseAccentColor = true;
            btnSettings.Click += Settings_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnRefresh.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnRefresh.Depth = 0;
            btnRefresh.HighEmphasis = true;
            btnRefresh.Icon = null;
            btnRefresh.Location = new Point(194, 440);
            btnRefresh.Margin = new Padding(4, 6, 4, 6);
            btnRefresh.MouseState = MaterialSkin.MouseState.HOVER;
            btnRefresh.Name = "btnRefresh";
            btnRefresh.NoAccentTextColor = Color.Empty;
            btnRefresh.Size = new Size(84, 36);
            btnRefresh.TabIndex = 7;
            btnRefresh.Text = "Refresh";
            btnRefresh.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnRefresh.UseAccentColor = true;
            btnRefresh.Click += Refresh_Click;
            // 
            // labelHeader
            // 
            labelHeader.AutoSize = true;
            labelHeader.Depth = 0;
            labelHeader.Font = new Font("Roboto", 34F, FontStyle.Bold, GraphicsUnit.Pixel);
            labelHeader.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            labelHeader.ForeColor = Color.FromArgb(212, 160, 23);
            labelHeader.Location = new Point(16, 21);
            labelHeader.MouseState = MaterialSkin.MouseState.HOVER;
            labelHeader.Name = "labelHeader";
            labelHeader.Size = new Size(167, 41);
            labelHeader.TabIndex = 8;
            labelHeader.Text = "GhostDock";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 482);
            label1.Name = "label1";
            label1.Size = new Size(517, 15);
            label1.TabIndex = 9;
            label1.Text = "------------------------------------------------------------------------------------------------------";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Palatino Linotype", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(368, 517);
            label2.Name = "label2";
            label2.Size = new Size(42, 17);
            label2.TabIndex = 10;
            label2.Text = "v.1.0.0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Palatino Linotype", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(14, 517);
            label3.Name = "label3";
            label3.Size = new Size(90, 17);
            label3.TabIndex = 11;
            label3.Text = "© Castron 2024";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 37, 38);
            ClientSize = new Size(451, 541);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnRefresh);
            Controls.Add(btnSettings);
            Controls.Add(borderPanel);
            Controls.Add(labelHeader);
            Controls.Add(listBoxApps);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            ShowInTaskbar = false;
            Text = "GhostDock";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            Resize += Form1_Resize;
            panelAudioControls.ResumeLayout(false);
            panelAudioControls.PerformLayout();
            borderPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
        private Label label2;
        private Label label3;
        private NotifyIcon NotifyIcon;
    }
}