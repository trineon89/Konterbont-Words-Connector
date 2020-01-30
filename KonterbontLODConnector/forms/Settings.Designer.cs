namespace KonterbontLODConnector
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.General = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.grpLanguage = new System.Windows.Forms.GroupBox();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnImportOldContent = new System.Windows.Forms.Button();
            this.btnCleanupArticle = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.General.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.grpLanguage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(618, 575);
            this.tabControl1.TabIndex = 0;
            // 
            // General
            // 
            this.General.Controls.Add(this.flowLayoutPanel1);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Padding = new System.Windows.Forms.Padding(3);
            this.General.Size = new System.Drawing.Size(610, 549);
            this.General.TabIndex = 0;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.grpLanguage);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(604, 543);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // grpLanguage
            // 
            this.grpLanguage.Controls.Add(this.cbLanguage);
            this.grpLanguage.Location = new System.Drawing.Point(3, 3);
            this.grpLanguage.Name = "grpLanguage";
            this.grpLanguage.Size = new System.Drawing.Size(135, 46);
            this.grpLanguage.TabIndex = 0;
            this.grpLanguage.TabStop = false;
            this.grpLanguage.Text = "Language";
            // 
            // cbLanguage
            // 
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Location = new System.Drawing.Point(6, 19);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(121, 21);
            this.cbLanguage.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(610, 549);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Coming soon...";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCleanupArticle);
            this.groupBox1.Controls.Add(this.btnImportOldContent);
            this.groupBox1.Location = new System.Drawing.Point(144, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 95);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced Actions";
            // 
            // btnImportOldContent
            // 
            this.btnImportOldContent.Location = new System.Drawing.Point(6, 17);
            this.btnImportOldContent.Name = "btnImportOldContent";
            this.btnImportOldContent.Size = new System.Drawing.Size(175, 23);
            this.btnImportOldContent.TabIndex = 0;
            this.btnImportOldContent.Text = "Import old Selections";
            this.btnImportOldContent.UseVisualStyleBackColor = true;
            this.btnImportOldContent.Click += new System.EventHandler(this.btnImportOldContent_Click);
            // 
            // btnCleanupArticle
            // 
            this.btnCleanupArticle.Location = new System.Drawing.Point(6, 46);
            this.btnCleanupArticle.Name = "btnCleanupArticle";
            this.btnCleanupArticle.Size = new System.Drawing.Size(175, 23);
            this.btnCleanupArticle.TabIndex = 1;
            this.btnCleanupArticle.Text = "btnStartCleanup";
            this.btnCleanupArticle.UseVisualStyleBackColor = true;
            this.btnCleanupArticle.Click += new System.EventHandler(this.btnCleanupArticle_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 575);
            this.Controls.Add(this.tabControl1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.tabControl1.ResumeLayout(false);
            this.General.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.grpLanguage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox grpLanguage;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCleanupArticle;
        private System.Windows.Forms.Button btnImportOldContent;
    }
}