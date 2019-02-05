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
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.btnFetch = new System.Windows.Forms.Button();
            this.rtbTest = new System.Windows.Forms.RichTextBox();
            this.edtWord = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.artikelOpmaachenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbResult
            // 
            this.rtbResult.BackColor = System.Drawing.SystemColors.Window;
            this.rtbResult.Location = new System.Drawing.Point(30, 68);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(780, 238);
            this.rtbResult.TabIndex = 1;
            this.rtbResult.Text = "";
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(735, 29);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(75, 23);
            this.btnFetch.TabIndex = 2;
            this.btnFetch.Text = "&Fetch";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.BtnFetch_ClickAsync);
            // 
            // rtbTest
            // 
            this.rtbTest.Location = new System.Drawing.Point(30, 322);
            this.rtbTest.Name = "rtbTest";
            this.rtbTest.Size = new System.Drawing.Size(533, 178);
            this.rtbTest.TabIndex = 5;
            this.rtbTest.Text = "";
            // 
            // edtWord
            // 
            this.edtWord.Location = new System.Drawing.Point(30, 31);
            this.edtWord.Name = "edtWord";
            this.edtWord.Size = new System.Drawing.Size(684, 20);
            this.edtWord.TabIndex = 6;
            this.edtWord.Text = "beier";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(897, 24);
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
            // frmMain
            // 
            this.AcceptButton = this.btnFetch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 524);
            this.Controls.Add(this.edtWord);
            this.Controls.Add(this.rtbTest);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Konterbont LOD Connector";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.RichTextBox rtbTest;
        private System.Windows.Forms.TextBox edtWord;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem artikelOpmaachenToolStripMenuItem;
    }
}

