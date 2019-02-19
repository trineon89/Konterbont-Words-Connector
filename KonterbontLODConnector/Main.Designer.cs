namespace KonterbontLODConnector
{
    partial class frmMain
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
            this.mmMain = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenArticle = new System.Windows.Forms.ToolStripMenuItem();
            this.astellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.magazineSelectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lbWords = new System.Windows.Forms.ListBox();
            this.lbSelectWord = new System.Windows.Forms.ListBox();
            this.lbSelectMeaning = new System.Windows.Forms.ListBox();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCreatePopups = new System.Windows.Forms.Button();
            this.btnCopyToMag = new System.Windows.Forms.Button();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.rtbDetails = new System.Windows.Forms.RichTextBox();
            this.mmMain.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.magazineOpmaachenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // mmMain
            // 
            this.mmMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.astellungenToolStripMenuItem});
            this.mmMain.Location = new System.Drawing.Point(0, 0);
            this.mmMain.Name = "mmMain";
            this.mmMain.Size = new System.Drawing.Size(735, 24);
            this.mmMain.TabIndex = 7;
            this.mmMain.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiOpenArticle,
            this.tsmiSave,
            this.tsmiExit});
            this.magazineOpmaachenToolStripMenuItem});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.dateiToolStripMenuItem.Text = "Datei";
            // 
            // tsmiNew
            // 
            this.tsmiNew.Name = "tsmiNew";
            this.tsmiNew.Size = new System.Drawing.Size(183, 22);
            this.tsmiNew.Text = "Nei";
            // 
            // tsmiOpenArticle
            // 
            this.tsmiOpenArticle.Name = "tsmiOpenArticle";
            this.tsmiOpenArticle.Size = new System.Drawing.Size(183, 22);
            this.tsmiOpenArticle.Text = "Artikel opmaachen...";
            this.tsmiOpenArticle.Click += new System.EventHandler(this.ArtikelOpmaachenToolStripMenuItem_Click);
            // 
            // astellungenToolStripMenuItem
            // 
            this.astellungenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.magazineSelectorToolStripMenuItem});
            this.astellungenToolStripMenuItem.Name = "astellungenToolStripMenuItem";
            this.astellungenToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.astellungenToolStripMenuItem.Text = "Astellungen";
            // 
            // magazineSelectorToolStripMenuItem
            // 
            this.magazineSelectorToolStripMenuItem.Name = "magazineSelectorToolStripMenuItem";
            this.magazineSelectorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.magazineSelectorToolStripMenuItem.Text = "Magazine Selector...";
            this.magazineSelectorToolStripMenuItem.Click += new System.EventHandler(this.MagazineSelectorToolStripMenuItem_Click);
            // 
            // tlpMain
            // 
            this.tlpMain.AutoSize = true;
            this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMain.ColumnCount = 4;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.88742F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.88742F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.11258F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.11258F));
            this.tlpMain.Controls.Add(this.btnCopyToMag, 1, 0);
            this.tlpMain.Controls.Add(this.btnCreatePopups, 0, 0);
            this.tlpMain.Controls.Add(this.lbWords, 0, 1);
            this.tlpMain.Controls.Add(this.lbSelectWord, 2, 1);
            this.tlpMain.Controls.Add(this.lbSelectMeaning, 3, 1);
            this.tlpMain.Controls.Add(this.pnlDetails, 2, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlpMain.Location = new System.Drawing.Point(0, 24);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.36842F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.63158F));
            this.tlpMain.Size = new System.Drawing.Size(735, 439);
            this.tlpMain.TabIndex = 15;
            // 
            // lbWords
            // 
            this.tlpMain.SetColumnSpan(this.lbWords, 2);
            this.lbWords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbWords.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWords.FormattingEnabled = true;
            this.lbWords.Location = new System.Drawing.Point(5, 35);
            this.lbWords.Margin = new System.Windows.Forms.Padding(5);
            this.lbWords.Name = "lbWords";
            this.tlpMain.SetRowSpan(this.lbWords, 2);
            this.lbWords.Size = new System.Drawing.Size(238, 399);
            this.lbWords.TabIndex = 8;
            this.lbWords.SelectedIndexChanged += new System.EventHandler(this.LbWords_SelectedIndexChanged);
            // 
            // lbSelectWord
            // 
            this.lbSelectWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelectWord.FormattingEnabled = true;
            this.lbSelectWord.Location = new System.Drawing.Point(253, 35);
            this.lbSelectWord.Margin = new System.Windows.Forms.Padding(5);
            this.lbSelectWord.Name = "lbSelectWord";
            this.lbSelectWord.Size = new System.Drawing.Size(233, 183);
            this.lbSelectWord.TabIndex = 9;
            this.lbSelectWord.SelectedIndexChanged += new System.EventHandler(this.LbSelectWord_SelectedIndexChanged);
            this.lbSelectWord.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectWord_MouseDoubleClick);
            // 
            // lbSelectMeaning
            // 
            this.lbSelectMeaning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelectMeaning.FormattingEnabled = true;
            this.lbSelectMeaning.Location = new System.Drawing.Point(496, 35);
            this.lbSelectMeaning.Margin = new System.Windows.Forms.Padding(5);
            this.lbSelectMeaning.Name = "lbSelectMeaning";
            this.lbSelectMeaning.Size = new System.Drawing.Size(234, 183);
            this.lbSelectMeaning.TabIndex = 10;
            this.lbSelectMeaning.SelectedIndexChanged += new System.EventHandler(this.LbSelectMeaning_SelectedIndexChanged);
            this.lbSelectMeaning.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectMeaning_MouseDoubleClick);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(183, 22);
            this.tsmiExit.Text = "Zoumaachen";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Enabled = false;
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(183, 22);
            this.tsmiSave.Text = "Späicheren";
            this.tsmiSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCreatePopups
            // 
            this.btnCreatePopups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreatePopups.Enabled = false;
            this.btnCreatePopups.Location = new System.Drawing.Point(3, 3);
            this.btnCreatePopups.Name = "btnCreatePopups";
            this.btnCreatePopups.Size = new System.Drawing.Size(118, 24);
            this.btnCreatePopups.TabIndex = 14;
            this.btnCreatePopups.Text = "Popups erstellen";
            this.btnCreatePopups.UseVisualStyleBackColor = true;
            this.btnCreatePopups.Click += new System.EventHandler(this.BtnCreatePopups_Click);
            // 
            // btnCopyToMag
            // 
            this.btnCopyToMag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCopyToMag.Enabled = false;
            this.btnCopyToMag.Location = new System.Drawing.Point(127, 3);
            this.btnCopyToMag.Name = "btnCopyToMag";
            this.btnCopyToMag.Size = new System.Drawing.Size(118, 24);
            this.btnCopyToMag.TabIndex = 15;
            this.btnCopyToMag.Text = "An de Magazin";
            this.btnCopyToMag.UseVisualStyleBackColor = true;
            this.btnCopyToMag.Click += new System.EventHandler(this.btnCopyMag_Click);
            // 
            // pnlDetails
            // 
            this.pnlDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpMain.SetColumnSpan(this.pnlDetails, 2);
            this.pnlDetails.Controls.Add(this.rtbDetails);
            this.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetails.Location = new System.Drawing.Point(253, 228);
            this.pnlDetails.Margin = new System.Windows.Forms.Padding(5);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Padding = new System.Windows.Forms.Padding(5);
            this.pnlDetails.Size = new System.Drawing.Size(477, 206);
            this.pnlDetails.TabIndex = 17;
            // 
            // rtbDetails
            // 
            this.rtbDetails.BackColor = System.Drawing.SystemColors.Control;
            this.rtbDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDetails.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbDetails.Location = new System.Drawing.Point(5, 5);
            this.rtbDetails.Margin = new System.Windows.Forms.Padding(5);
            this.rtbDetails.Name = "rtbDetails";
            this.rtbDetails.ReadOnly = true;
            this.rtbDetails.Size = new System.Drawing.Size(465, 194);
            this.rtbDetails.TabIndex = 17;
            this.rtbDetails.Text = "";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // magazineOpmaachenToolStripMenuItem
            // 
            this.magazineOpmaachenToolStripMenuItem.Name = "magazineOpmaachenToolStripMenuItem";
            this.magazineOpmaachenToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.magazineOpmaachenToolStripMenuItem.Text = "Magazine opmaachen...";
            this.magazineOpmaachenToolStripMenuItem.Click += new System.EventHandler(this.MagazineOpmaachenToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 463);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.mmMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.mmMain;
            this.Name = "frmMain";
            this.Text = "Konterbont LOD Connector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.mmMain.ResumeLayout(false);
            this.mmMain.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.pnlDetails.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mmMain;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiNew;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenArticle;
        private System.Windows.Forms.ToolStripMenuItem astellungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem magazineSelectorToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.ListBox lbWords;
        private System.Windows.Forms.ListBox lbSelectWord;
        private System.Windows.Forms.ListBox lbSelectMeaning;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.Button btnCopyToMag;
        private System.Windows.Forms.Button btnCreatePopups;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.RichTextBox rtbDetails;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem magazineOpmaachenToolStripMenuItem;
    }
}

