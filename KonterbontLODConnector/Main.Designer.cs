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
            this.btnFetch = new System.Windows.Forms.Button();
            this.edtWord = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.artikelOpmaachenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbWords = new System.Windows.Forms.ListBox();
            this.lbSelectWord = new System.Windows.Forms.ListBox();
            this.lbSelectMeaning = new System.Windows.Forms.ListBox();
            this.rtbDetails = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(639, 27);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(75, 23);
            this.btnFetch.TabIndex = 2;
            this.btnFetch.Text = "&Fetch";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.BtnFetch_ClickAsync);
            // 
            // edtWord
            // 
            this.edtWord.Location = new System.Drawing.Point(12, 30);
            this.edtWord.Name = "edtWord";
            this.edtWord.Size = new System.Drawing.Size(621, 22);
            this.edtWord.TabIndex = 6;
            this.edtWord.Text = "beier";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem});
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
            // lbWords
            // 
            this.lbWords.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWords.FormattingEnabled = true;
            this.lbWords.Location = new System.Drawing.Point(12, 57);
            this.lbWords.Name = "lbWords";
            this.lbWords.Size = new System.Drawing.Size(225, 459);
            this.lbWords.TabIndex = 8;
            this.lbWords.SelectedIndexChanged += new System.EventHandler(this.lbWords_SelectedIndexChanged);
            // 
            // lbSelectWord
            // 
            this.lbSelectWord.FormattingEnabled = true;
            this.lbSelectWord.Location = new System.Drawing.Point(243, 57);
            this.lbSelectWord.Name = "lbSelectWord";
            this.lbSelectWord.Size = new System.Drawing.Size(234, 199);
            this.lbSelectWord.TabIndex = 9;
            this.lbSelectWord.SelectedIndexChanged += new System.EventHandler(this.lbSelectWord_SelectedIndexChanged);
            // 
            // lbSelectMeaning
            // 
            this.lbSelectMeaning.FormattingEnabled = true;
            this.lbSelectMeaning.Location = new System.Drawing.Point(483, 57);
            this.lbSelectMeaning.Name = "lbSelectMeaning";
            this.lbSelectMeaning.Size = new System.Drawing.Size(231, 199);
            this.lbSelectMeaning.TabIndex = 10;
            this.lbSelectMeaning.SelectedIndexChanged += new System.EventHandler(this.lbSelectMeaning_SelectedIndexChanged);
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
            // frmMain
            // 
            this.AcceptButton = this.btnFetch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 524);
            this.Controls.Add(this.rtbDetails);
            this.Controls.Add(this.lbSelectMeaning);
            this.Controls.Add(this.lbSelectWord);
            this.Controls.Add(this.lbWords);
            this.Controls.Add(this.edtWord);
            this.Controls.Add(this.btnFetch);
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
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.TextBox edtWord;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem artikelOpmaachenToolStripMenuItem;
        private System.Windows.Forms.ListBox lbWords;
        private System.Windows.Forms.ListBox lbSelectWord;
        private System.Windows.Forms.ListBox lbSelectMeaning;
        private System.Windows.Forms.RichTextBox rtbDetails;
    }
}

