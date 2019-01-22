namespace Konterbont_Words_Connector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnLoadAndGo = new System.Windows.Forms.Button();
            this.lbValues = new System.Windows.Forms.ListBox();
            this.lbOccurences = new System.Windows.Forms.ListBox();
            this.lblValues = new System.Windows.Forms.Label();
            this.lblOccurences = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoumaachenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.astellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.magazineAuswielenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbNotFound = new System.Windows.Forms.ListBox();
            this.btnCopyToMag = new System.Windows.Forms.Button();
            this.lblArticle = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadAndGo
            // 
            this.btnLoadAndGo.Location = new System.Drawing.Point(12, 38);
            this.btnLoadAndGo.Name = "btnLoadAndGo";
            this.btnLoadAndGo.Size = new System.Drawing.Size(107, 23);
            this.btnLoadAndGo.TabIndex = 0;
            this.btnLoadAndGo.Text = "Open WordList";
            this.btnLoadAndGo.UseVisualStyleBackColor = true;
            this.btnLoadAndGo.Click += new System.EventHandler(this.Button1_Click);
            // 
            // lbValues
            // 
            this.lbValues.FormattingEnabled = true;
            this.lbValues.Location = new System.Drawing.Point(320, 38);
            this.lbValues.Name = "lbValues";
            this.lbValues.Size = new System.Drawing.Size(161, 355);
            this.lbValues.TabIndex = 1;
            // 
            // lbOccurences
            // 
            this.lbOccurences.FormattingEnabled = true;
            this.lbOccurences.Location = new System.Drawing.Point(487, 38);
            this.lbOccurences.Name = "lbOccurences";
            this.lbOccurences.Size = new System.Drawing.Size(257, 355);
            this.lbOccurences.TabIndex = 2;
            // 
            // lblValues
            // 
            this.lblValues.AutoSize = true;
            this.lblValues.Location = new System.Drawing.Point(317, 396);
            this.lblValues.Name = "lblValues";
            this.lblValues.Size = new System.Drawing.Size(13, 13);
            this.lblValues.TabIndex = 3;
            this.lblValues.Text = "0";
            // 
            // lblOccurences
            // 
            this.lblOccurences.AutoSize = true;
            this.lblOccurences.Location = new System.Drawing.Point(488, 395);
            this.lblOccurences.Name = "lblOccurences";
            this.lblOccurences.Size = new System.Drawing.Size(13, 13);
            this.lblOccurences.TabIndex = 4;
            this.lblOccurences.Text = "0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.astellungenToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(756, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoumaachenToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            this.menuToolStripMenuItem.Click += new System.EventHandler(this.menuToolStripMenuItem_Click);
            // 
            // zoumaachenToolStripMenuItem
            // 
            this.zoumaachenToolStripMenuItem.Name = "zoumaachenToolStripMenuItem";
            this.zoumaachenToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.zoumaachenToolStripMenuItem.Text = "Zoumaachen";
            this.zoumaachenToolStripMenuItem.Click += new System.EventHandler(this.ZoumaachenToolStripMenuItem_Click);
            // 
            // astellungenToolStripMenuItem
            // 
            this.astellungenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.magazineAuswielenToolStripMenuItem});
            this.astellungenToolStripMenuItem.Name = "astellungenToolStripMenuItem";
            this.astellungenToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.astellungenToolStripMenuItem.Text = "Astellungen";
            // 
            // magazineAuswielenToolStripMenuItem
            // 
            this.magazineAuswielenToolStripMenuItem.Name = "magazineAuswielenToolStripMenuItem";
            this.magazineAuswielenToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.magazineAuswielenToolStripMenuItem.Text = "Magazine Auswielen...";
            this.magazineAuswielenToolStripMenuItem.Click += new System.EventHandler(this.MagazineAuswielenToolStripMenuItem_Click);
            // 
            // lbNotFound
            // 
            this.lbNotFound.BackColor = System.Drawing.SystemColors.Window;
            this.lbNotFound.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNotFound.FormattingEnabled = true;
            this.lbNotFound.ItemHeight = 17;
            this.lbNotFound.Location = new System.Drawing.Point(12, 90);
            this.lbNotFound.Name = "lbNotFound";
            this.lbNotFound.Size = new System.Drawing.Size(293, 310);
            this.lbNotFound.TabIndex = 6;
            // 
            // btnCopyToMag
            // 
            this.btnCopyToMag.Location = new System.Drawing.Point(125, 38);
            this.btnCopyToMag.Name = "btnCopyToMag";
            this.btnCopyToMag.Size = new System.Drawing.Size(121, 23);
            this.btnCopyToMag.TabIndex = 7;
            this.btnCopyToMag.Text = "An de Magazine kopéieren";
            this.btnCopyToMag.UseVisualStyleBackColor = true;
            this.btnCopyToMag.Click += new System.EventHandler(this.BtnCopyToMag_Click);
            // 
            // lblArticle
            // 
            this.lblArticle.AutoSize = true;
            this.lblArticle.Location = new System.Drawing.Point(13, 68);
            this.lblArticle.Name = "lblArticle";
            this.lblArticle.Size = new System.Drawing.Size(63, 13);
            this.lblArticle.TabIndex = 8;
            this.lblArticle.Text = "Placeholder";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 435);
            this.Controls.Add(this.lblArticle);
            this.Controls.Add(this.btnCopyToMag);
            this.Controls.Add(this.lbNotFound);
            this.Controls.Add(this.lblOccurences);
            this.Controls.Add(this.lblValues);
            this.Controls.Add(this.lbOccurences);
            this.Controls.Add(this.lbValues);
            this.Controls.Add(this.btnLoadAndGo);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Konterbont Wieder Selector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnLoadAndGo;
        private System.Windows.Forms.ListBox lbValues;
        private System.Windows.Forms.ListBox lbOccurences;
        private System.Windows.Forms.Label lblValues;
        private System.Windows.Forms.Label lblOccurences;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem astellungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem magazineAuswielenToolStripMenuItem;
        private System.Windows.Forms.ListBox lbNotFound;
        private System.Windows.Forms.Button btnCopyToMag;
        private System.Windows.Forms.Label lblArticle;
        private System.Windows.Forms.ToolStripMenuItem zoumaachenToolStripMenuItem;
    }
}

