namespace KonterbontLODConnector
{
    partial class frmMainProgram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainProgram));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnMenuFolder = new System.Windows.Forms.Button();
            this.btnArtikelOpman = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.richTextWPF1 = new KonterbontLODConnector.RichTextWPF();
            this.panel2 = new System.Windows.Forms.Panel();
            this.vistaOpenFileDialog = new Ookii.Dialogs.WinForms.VistaOpenFileDialog();
            this.panelMenu.SuspendLayout();
            this.panel3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelMenu.Controls.Add(this.btnMenuFolder);
            this.panelMenu.Controls.Add(this.btnArtikelOpman);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(240, 748);
            this.panelMenu.TabIndex = 1;
            // 
            // btnMenuFolder
            // 
            this.btnMenuFolder.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMenuFolder.FlatAppearance.BorderSize = 0;
            this.btnMenuFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMenuFolder.ForeColor = System.Drawing.Color.White;
            this.btnMenuFolder.Location = new System.Drawing.Point(0, 0);
            this.btnMenuFolder.Name = "btnMenuFolder";
            this.btnMenuFolder.Size = new System.Drawing.Size(240, 30);
            this.btnMenuFolder.TabIndex = 1;
            this.btnMenuFolder.Text = "<<";
            this.btnMenuFolder.UseVisualStyleBackColor = true;
            this.btnMenuFolder.Click += new System.EventHandler(this.btnMenuFolder_Click);
            // 
            // btnArtikelOpman
            // 
            this.btnArtikelOpman.FlatAppearance.BorderSize = 0;
            this.btnArtikelOpman.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnArtikelOpman.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArtikelOpman.ForeColor = System.Drawing.Color.White;
            this.btnArtikelOpman.Location = new System.Drawing.Point(0, 29);
            this.btnArtikelOpman.Name = "btnArtikelOpman";
            this.btnArtikelOpman.Size = new System.Drawing.Size(240, 50);
            this.btnArtikelOpman.TabIndex = 0;
            this.btnArtikelOpman.Text = "Artikel opmaachen";
            this.btnArtikelOpman.UseVisualStyleBackColor = true;
            this.btnArtikelOpman.Click += new System.EventHandler(this.btnArtikelOpman_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panel3.Controls.Add(this.statusStrip);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(240, 719);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1110, 29);
            this.panel3.TabIndex = 3;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 3);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1110, 26);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(200, 20);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(92, 21);
            this.toolStripStatusLabel.Text = "-----------------";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(240, 15);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.elementHost1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(1110, 704);
            this.splitContainer1.SplitterDistance = 788;
            this.splitContainer1.TabIndex = 4;
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(1110, 704);
            this.elementHost1.TabIndex = 2;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.richTextWPF1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(240, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1110, 15);
            this.panel2.TabIndex = 2;
            // 
            // vistaOpenFileDialog
            // 
            this.vistaOpenFileDialog.FileName = "vistaOpenFileDialog";
            this.vistaOpenFileDialog.Filter = null;
            // 
            // frmMainProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 748);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMainProgram";
            this.Text = "MainProgram";
            this.panelMenu.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnArtikelOpman;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnMenuFolder;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private Ookii.Dialogs.WinForms.VistaOpenFileDialog vistaOpenFileDialog;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private RichTextWPF richTextWPF1;
    }
}