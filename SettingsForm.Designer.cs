namespace GhostDock
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private MaterialSkin.Controls.MaterialComboBox comboBox1;
        private MaterialSkin.Controls.MaterialComboBox comboBox2;
        private MaterialSkin.Controls.MaterialComboBox comboBox3;
        private MaterialSkin.Controls.MaterialLabel label1;
        private MaterialSkin.Controls.MaterialLabel label2;
        private MaterialSkin.Controls.MaterialLabel label3;
        private MaterialSkin.Controls.MaterialButton btnSave;
        private MaterialSkin.Controls.MaterialButton btnCancel;
        private MaterialSkin.Controls.MaterialComboBox comboBoxColorScheme;
        private MaterialSkin.Controls.MaterialLabel labelColorScheme;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.comboBox1 = new MaterialSkin.Controls.MaterialComboBox();
            this.comboBox2 = new MaterialSkin.Controls.MaterialComboBox();
            this.comboBox3 = new MaterialSkin.Controls.MaterialComboBox();
            this.label1 = new MaterialSkin.Controls.MaterialLabel();
            this.label2 = new MaterialSkin.Controls.MaterialLabel();
            this.label3 = new MaterialSkin.Controls.MaterialLabel();
            this.btnSave = new MaterialSkin.Controls.MaterialButton();
            this.btnCancel = new MaterialSkin.Controls.MaterialButton();
            this.comboBoxColorScheme = new MaterialSkin.Controls.MaterialComboBox();
            this.labelColorScheme = new MaterialSkin.Controls.MaterialLabel();

            // comboBox1
            this.comboBox1.Location = new System.Drawing.Point(120, 120);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(220, 36);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);

            // comboBox2
            this.comboBox2.Location = new System.Drawing.Point(120, 170);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(220, 36);
            this.comboBox2.TabIndex = 2;
            this.comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);

            // comboBox3
            this.comboBox3.Location = new System.Drawing.Point(120, 220);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(220, 36);
            this.comboBox3.TabIndex = 3;
            this.comboBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 120);
            this.label1.Name = "label1";
            this.label1.Text = "Slider 1:";
            this.label1.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1;
            this.label1.Font = new System.Drawing.Font("Roboto", 10);

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 170);
            this.label2.Name = "label2";
            this.label2.Text = "Slider 2:";
            this.label2.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1;
            this.label2.Font = new System.Drawing.Font("Roboto", 10);

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 220);
            this.label3.Name = "label3";
            this.label3.Text = "Slider 3:";
            this.label3.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1;
            this.label3.Font = new System.Drawing.Font("Roboto", 10);

            // labelColorScheme
            this.labelColorScheme.AutoSize = true;
            this.labelColorScheme.Location = new System.Drawing.Point(20, 70);
            this.labelColorScheme.Name = "labelColorScheme";
            this.labelColorScheme.Text = "Color Scheme:";
            this.labelColorScheme.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1;
            this.labelColorScheme.Font = new System.Drawing.Font("Roboto", 10);

            // comboBoxColorScheme
            this.comboBoxColorScheme.Location = new System.Drawing.Point(120, 70);
            this.comboBoxColorScheme.Name = "comboBoxColorScheme";
            this.comboBoxColorScheme.Size = new System.Drawing.Size(220, 36);
            this.comboBoxColorScheme.TabIndex = 0;
            this.comboBoxColorScheme.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(250, 280);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 36);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(150, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 36);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // SettingsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 360); // Increased size for better spacing
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.comboBoxColorScheme);
            this.Controls.Add(this.labelColorScheme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "GhostDock Settings";
            this.StartPosition = FormStartPosition.CenterParent; // Center on parent form
        }
    }
}