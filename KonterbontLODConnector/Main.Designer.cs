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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.artikelOpmaachenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.astellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.magazineSelectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbWords = new System.Windows.Forms.ListBox();
            this.lbSelectWord = new System.Windows.Forms.ListBox();
            this.lbSelectMeaning = new System.Windows.Forms.ListBox();
            this.rtbDetails = new System.Windows.Forms.RichTextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCreatePopups = new System.Windows.Forms.Button();
            this.btnCopyToMag = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.astellungenToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(726, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neiToolStripMenuItem,
            this.artikelOpmaachenToolStripMenuItem});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.dateiToolStripMenuItem.Text = "Datei";
            // 
            // neiToolStripMenuItem
            // 
            this.neiToolStripMenuItem.Name = "neiToolStripMenuItem";
            this.neiToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.neiToolStripMenuItem.Text = "Nei";
            // 
            // artikelOpmaachenToolStripMenuItem
            // 
            this.artikelOpmaachenToolStripMenuItem.Name = "artikelOpmaachenToolStripMenuItem";
            this.artikelOpmaachenToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.artikelOpmaachenToolStripMenuItem.Text = "Artikel opmaachen...";
            this.artikelOpmaachenToolStripMenuItem.Click += new System.EventHandler(this.ArtikelOpmaachenToolStripMenuItem_Click);
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
            this.magazineSelectorToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.magazineSelectorToolStripMenuItem.Text = "Magazine Selector...";
            this.magazineSelectorToolStripMenuItem.Click += new System.EventHandler(this.MagazineSelectorToolStripMenuItem_Click);
            // 
            // lbWords
            // 
            this.lbWords.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWords.FormattingEnabled = true;
            this.lbWords.Location = new System.Drawing.Point(12, 57);
            this.lbWords.Name = "lbWords";
            this.lbWords.Size = new System.Drawing.Size(225, 459);
            this.lbWords.TabIndex = 8;
            this.lbWords.SelectedIndexChanged += new System.EventHandler(this.LbWords_SelectedIndexChanged);
            // 
            // lbSelectWord
            // 
            this.lbSelectWord.FormattingEnabled = true;
            this.lbSelectWord.Location = new System.Drawing.Point(243, 57);
            this.lbSelectWord.Name = "lbSelectWord";
            this.lbSelectWord.Size = new System.Drawing.Size(234, 199);
            this.lbSelectWord.TabIndex = 9;
            this.lbSelectWord.SelectedIndexChanged += new System.EventHandler(this.LbSelectWord_SelectedIndexChanged);
            this.lbSelectWord.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectWord_MouseDoubleClick);
            // 
            // lbSelectMeaning
            // 
            this.lbSelectMeaning.FormattingEnabled = true;
            this.lbSelectMeaning.Location = new System.Drawing.Point(483, 57);
            this.lbSelectMeaning.Name = "lbSelectMeaning";
            this.lbSelectMeaning.Size = new System.Drawing.Size(231, 199);
            this.lbSelectMeaning.TabIndex = 10;
            this.lbSelectMeaning.SelectedIndexChanged += new System.EventHandler(this.LbSelectMeaning_SelectedIndexChanged);
            this.lbSelectMeaning.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectMeaning_MouseDoubleClick);
            // 
            // rtbDetails
            // 
            this.rtbDetails.BackColor = System.Drawing.SystemColors.Control;
            this.rtbDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbDetails.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbDetails.Location = new System.Drawing.Point(244, 263);
            this.rtbDetails.Name = "rtbDetails";
            this.rtbDetails.ReadOnly = true;
            this.rtbDetails.Size = new System.Drawing.Size(470, 253);
            this.rtbDetails.TabIndex = 11;
            this.rtbDetails.Text = "";
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(12, 28);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Späicheren";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCreatePopups
            // 
            this.btnCreatePopups.Enabled = false;
            this.btnCreatePopups.Location = new System.Drawing.Point(121, 28);
            this.btnCreatePopups.Name = "btnCreatePopups";
            this.btnCreatePopups.Size = new System.Drawing.Size(116, 23);
            this.btnCreatePopups.TabIndex = 13;
            this.btnCreatePopups.Text = "Popups erstellen";
            this.btnCreatePopups.UseVisualStyleBackColor = true;
            this.btnCreatePopups.Click += new System.EventHandler(this.BtnCreatePopups_Click);
            // 
            // btnCopyToMag
            // 
            this.btnCopyToMag.Location = new System.Drawing.Point(243, 28);
            this.btnCopyToMag.Name = "btnCopyToMag";
            this.btnCopyToMag.Size = new System.Drawing.Size(116, 23);
            this.btnCopyToMag.TabIndex = 14;
            this.btnCopyToMag.Text = "An de Magazin";
            this.btnCopyToMag.UseVisualStyleBackColor = true;
            this.btnCopyToMag.Click += new System.EventHandler(this.btnCopyMag_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 524);
            this.Controls.Add(this.btnCopyToMag);
            this.Controls.Add(this.btnCreatePopups);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.rtbDetails);
            this.Controls.Add(this.lbSelectMeaning);
            this.Controls.Add(this.lbSelectWord);
            this.Controls.Add(this.lbWords);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Konterbont LOD Connector";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem artikelOpmaachenToolStripMenuItem;
        private System.Windows.Forms.ListBox lbWords;
        private System.Windows.Forms.ListBox lbSelectWord;
        private System.Windows.Forms.ListBox lbSelectMeaning;
        private System.Windows.Forms.RichTextBox rtbDetails;
        private System.Windows.Forms.ToolStripMenuItem astellungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem magazineSelectorToolStripMenuItem;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCreatePopups;
        private System.Windows.Forms.Button btnCopyToMag;
    }
}

