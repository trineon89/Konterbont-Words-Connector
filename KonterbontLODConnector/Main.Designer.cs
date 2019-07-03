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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mmMain = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenArticle = new System.Windows.Forms.ToolStripMenuItem();
            this.magazineOpmaachenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.astellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.magazineSelectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiText = new System.Windows.Forms.ToolStripMenuItem();
            this.tssExperimental = new System.Windows.Forms.ToolStripMenuItem();
            this.iNDesignConnectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.btnCopyToMag = new System.Windows.Forms.Button();
            this.btnCreatePopups = new System.Windows.Forms.Button();
            this.lbWords = new System.Windows.Forms.ListBox();
            this.lbSelectWord = new System.Windows.Forms.ListBox();
            this.lbSelectMeaning = new System.Windows.Forms.ListBox();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.rtbDetails = new System.Windows.Forms.RichTextBox();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.tssNeedSave = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssMagazine = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssArticle = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssInDesign = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnCustomAudio = new System.Windows.Forms.Button();
            this.btnPlayAudio = new System.Windows.Forms.Button();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.btnFetch = new System.Windows.Forms.Button();
            this.mmMain.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.ssStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // mmMain
            // 
            this.mmMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.astellungenToolStripMenuItem,
            this.tssExperimental});
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
            this.tsmiSave,
            this.toolStripSeparator1,
            this.tsmiOpenArticle,
            this.magazineOpmaachenToolStripMenuItem,
            this.toolStripSeparator2,
            this.tsmiExit});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.dateiToolStripMenuItem.Text = "Datei";
            // 
            // tsmiNew
            // 
            this.tsmiNew.Image = global::KonterbontLODConnector.Properties.Resources.NewFile_16x;
            this.tsmiNew.Name = "tsmiNew";
            this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiNew.Size = new System.Drawing.Size(245, 22);
            this.tsmiNew.Text = "Nei";
            this.tsmiNew.Click += new System.EventHandler(this.TsmiNew_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Enabled = false;
            this.tsmiSave.Image = global::KonterbontLODConnector.Properties.Resources.save_16xMD;
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiSave.Size = new System.Drawing.Size(245, 22);
            this.tsmiSave.Text = "Späicheren";
            this.tsmiSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(242, 6);
            // 
            // tsmiOpenArticle
            // 
            this.tsmiOpenArticle.Image = global::KonterbontLODConnector.Properties.Resources.OpenFile_16x;
            this.tsmiOpenArticle.Name = "tsmiOpenArticle";
            this.tsmiOpenArticle.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.tsmiOpenArticle.Size = new System.Drawing.Size(245, 22);
            this.tsmiOpenArticle.Text = "Artikel opmaachen...";
            this.tsmiOpenArticle.Click += new System.EventHandler(this.ArtikelOpmaachenToolStripMenuItem_Click);
            // 
            // magazineOpmaachenToolStripMenuItem
            // 
            this.magazineOpmaachenToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Strikeout))));
            this.magazineOpmaachenToolStripMenuItem.Image = global::KonterbontLODConnector.Properties.Resources.OpenFileFromProject_16x;
            this.magazineOpmaachenToolStripMenuItem.Name = "magazineOpmaachenToolStripMenuItem";
            this.magazineOpmaachenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.magazineOpmaachenToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.magazineOpmaachenToolStripMenuItem.Text = "Magasinn opmaachen...";
            this.magazineOpmaachenToolStripMenuItem.Click += new System.EventHandler(this.MagazineOpmaachenToolStripMenuItem1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(242, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Image = global::KonterbontLODConnector.Properties.Resources.Exit_16x;
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.tsmiExit.Size = new System.Drawing.Size(245, 22);
            this.tsmiExit.Text = "Zoumaachen";
            this.tsmiExit.Click += new System.EventHandler(this.TsmiExit_Click);
            // 
            // astellungenToolStripMenuItem
            // 
            this.astellungenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.magazineSelectorToolStripMenuItem,
            this.tsmiText});
            this.astellungenToolStripMenuItem.Name = "astellungenToolStripMenuItem";
            this.astellungenToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.astellungenToolStripMenuItem.Text = "Astellungen";
            // 
            // magazineSelectorToolStripMenuItem
            // 
            this.magazineSelectorToolStripMenuItem.Image = global::KonterbontLODConnector.Properties.Resources.SelectFileGroup_16x;
            this.magazineSelectorToolStripMenuItem.Name = "magazineSelectorToolStripMenuItem";
            this.magazineSelectorToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.magazineSelectorToolStripMenuItem.Text = "Magasinn auswielen...";
            this.magazineSelectorToolStripMenuItem.Click += new System.EventHandler(this.MagazineSelectorToolStripMenuItem_Click);
            // 
            // tsmiText
            // 
            this.tsmiText.Enabled = false;
            this.tsmiText.Name = "tsmiText";
            this.tsmiText.Size = new System.Drawing.Size(190, 22);
            this.tsmiText.Text = "Text uweisen...";
            this.tsmiText.Click += new System.EventHandler(this.textUweisenToolStripMenuItem_Click);
            // 
            // tssExperimental
            // 
            this.tssExperimental.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iNDesignConnectorToolStripMenuItem,
            this.bookContentToolStripMenuItem});
            this.tssExperimental.Name = "tssExperimental";
            this.tssExperimental.Size = new System.Drawing.Size(93, 20);
            this.tssExperimental.Text = "*experimental";
            this.tssExperimental.Click += new System.EventHandler(this.tssExperimental_Click);
            // 
            // iNDesignConnectorToolStripMenuItem
            // 
            this.iNDesignConnectorToolStripMenuItem.Name = "iNDesignConnectorToolStripMenuItem";
            this.iNDesignConnectorToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.iNDesignConnectorToolStripMenuItem.Text = "INDesign Connector";
            this.iNDesignConnectorToolStripMenuItem.Click += new System.EventHandler(this.iNDesignConnectorToolStripMenuItem_Click);
            // 
            // bookContentToolStripMenuItem
            // 
            this.bookContentToolStripMenuItem.Name = "bookContentToolStripMenuItem";
            this.bookContentToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.bookContentToolStripMenuItem.Text = "Book Content";
            this.bookContentToolStripMenuItem.Click += new System.EventHandler(this.bookContentToolStripMenuItem_Click);
            // 
            // tlpMain
            // 
            this.tlpMain.AutoSize = true;
            this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMain.ColumnCount = 6;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.35311F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.37916F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.76829F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.18411F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.29148F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tlpMain.Controls.Add(this.btnCopyToMag, 2, 0);
            this.tlpMain.Controls.Add(this.btnCreatePopups, 1, 0);
            this.tlpMain.Controls.Add(this.lbWords, 0, 1);
            this.tlpMain.Controls.Add(this.lbSelectWord, 2, 1);
            this.tlpMain.Controls.Add(this.lbSelectMeaning, 4, 1);
            this.tlpMain.Controls.Add(this.pnlDetails, 2, 2);
            this.tlpMain.Controls.Add(this.ssStatus, 0, 3);
            this.tlpMain.Controls.Add(this.btnCustomAudio, 5, 0);
            this.tlpMain.Controls.Add(this.btnPlayAudio, 4, 0);
            this.tlpMain.Controls.Add(this.axWindowsMediaPlayer1, 3, 0);
            this.tlpMain.Controls.Add(this.btnFetch, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlpMain.Location = new System.Drawing.Point(0, 24);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.36842F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.63158F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(735, 483);
            this.tlpMain.TabIndex = 15;
            // 
            // btnCopyToMag
            // 
            this.btnCopyToMag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCopyToMag.Enabled = false;
            this.btnCopyToMag.Location = new System.Drawing.Point(243, 3);
            this.btnCopyToMag.Name = "btnCopyToMag";
            this.btnCopyToMag.Size = new System.Drawing.Size(124, 24);
            this.btnCopyToMag.TabIndex = 15;
            this.btnCopyToMag.Text = "An de Magasinn";
            this.btnCopyToMag.UseVisualStyleBackColor = true;
            this.btnCopyToMag.Click += new System.EventHandler(this.btnCopyMag_Click);
            // 
            // btnCreatePopups
            // 
            this.btnCreatePopups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreatePopups.Enabled = false;
            this.btnCreatePopups.Location = new System.Drawing.Point(116, 3);
            this.btnCreatePopups.Name = "btnCreatePopups";
            this.btnCreatePopups.Size = new System.Drawing.Size(121, 24);
            this.btnCreatePopups.TabIndex = 14;
            this.btnCreatePopups.Text = "Popups erstellen";
            this.btnCreatePopups.UseVisualStyleBackColor = true;
            this.btnCreatePopups.Click += new System.EventHandler(this.BtnCreatePopups_Click);
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
            this.lbWords.Size = new System.Drawing.Size(230, 422);
            this.lbWords.TabIndex = 8;
            this.lbWords.SelectedIndexChanged += new System.EventHandler(this.LbWords_SelectedIndexChanged);
            // 
            // lbSelectWord
            // 
            this.tlpMain.SetColumnSpan(this.lbSelectWord, 2);
            this.lbSelectWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelectWord.FormattingEnabled = true;
            this.lbSelectWord.Location = new System.Drawing.Point(245, 35);
            this.lbSelectWord.Margin = new System.Windows.Forms.Padding(5);
            this.lbSelectWord.Name = "lbSelectWord";
            this.lbSelectWord.Size = new System.Drawing.Size(218, 195);
            this.lbSelectWord.TabIndex = 9;
            this.lbSelectWord.SelectedIndexChanged += new System.EventHandler(this.LbSelectWord_SelectedIndexChanged);
            this.lbSelectWord.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectWord_MouseDoubleClick);
            // 
            // lbSelectMeaning
            // 
            this.tlpMain.SetColumnSpan(this.lbSelectMeaning, 2);
            this.lbSelectMeaning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelectMeaning.FormattingEnabled = true;
            this.lbSelectMeaning.Location = new System.Drawing.Point(473, 35);
            this.lbSelectMeaning.Margin = new System.Windows.Forms.Padding(5);
            this.lbSelectMeaning.Name = "lbSelectMeaning";
            this.lbSelectMeaning.Size = new System.Drawing.Size(257, 195);
            this.lbSelectMeaning.TabIndex = 10;
            this.lbSelectMeaning.SelectedIndexChanged += new System.EventHandler(this.LbSelectMeaning_SelectedIndexChanged);
            this.lbSelectMeaning.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectMeaning_MouseDoubleClick);
            // 
            // pnlDetails
            // 
            this.pnlDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpMain.SetColumnSpan(this.pnlDetails, 4);
            this.pnlDetails.Controls.Add(this.rtbDetails);
            this.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetails.Location = new System.Drawing.Point(245, 240);
            this.pnlDetails.Margin = new System.Windows.Forms.Padding(5);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Padding = new System.Windows.Forms.Padding(5);
            this.pnlDetails.Size = new System.Drawing.Size(485, 217);
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
            this.rtbDetails.Size = new System.Drawing.Size(473, 205);
            this.rtbDetails.TabIndex = 17;
            this.rtbDetails.Text = "";
            // 
            // ssStatus
            // 
            this.tlpMain.SetColumnSpan(this.ssStatus, 5);
            this.ssStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssNeedSave,
            this.tssMagazine,
            this.tssArticle,
            this.tssInDesign});
            this.ssStatus.Location = new System.Drawing.Point(0, 462);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.Size = new System.Drawing.Size(691, 21);
            this.ssStatus.TabIndex = 18;
            // 
            // tssNeedSave
            // 
            this.tssNeedSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tssNeedSave.Name = "tssNeedSave";
            this.tssNeedSave.Size = new System.Drawing.Size(0, 16);
            // 
            // tssMagazine
            // 
            this.tssMagazine.Name = "tssMagazine";
            this.tssMagazine.Size = new System.Drawing.Size(44, 16);
            this.tssMagazine.Text = "_MAG_";
            // 
            // tssArticle
            // 
            this.tssArticle.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tssArticle.Name = "tssArticle";
            this.tssArticle.Size = new System.Drawing.Size(41, 16);
            this.tssArticle.Text = "_ART_";
            // 
            // tssInDesign
            // 
            this.tssInDesign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.tssInDesign.Name = "tssInDesign";
            this.tssInDesign.Size = new System.Drawing.Size(143, 16);
            this.tssInDesign.Text = "not connected to InDesign";
            // 
            // btnCustomAudio
            // 
            this.btnCustomAudio.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCustomAudio.Enabled = false;
            this.btnCustomAudio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCustomAudio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomAudio.Location = new System.Drawing.Point(707, 3);
            this.btnCustomAudio.Name = "btnCustomAudio";
            this.btnCustomAudio.Size = new System.Drawing.Size(25, 24);
            this.btnCustomAudio.TabIndex = 19;
            this.btnCustomAudio.Text = "♬";
            this.btnCustomAudio.UseVisualStyleBackColor = true;
            this.btnCustomAudio.Click += new System.EventHandler(this.btnCustomAudio_Click);
            // 
            // btnPlayAudio
            // 
            this.btnPlayAudio.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnPlayAudio.Enabled = false;
            this.btnPlayAudio.Location = new System.Drawing.Point(664, 3);
            this.btnPlayAudio.Name = "btnPlayAudio";
            this.btnPlayAudio.Size = new System.Drawing.Size(24, 24);
            this.btnPlayAudio.TabIndex = 20;
            this.btnPlayAudio.Text = "▶";
            this.btnPlayAudio.UseVisualStyleBackColor = true;
            this.btnPlayAudio.Click += new System.EventHandler(this.btnPlayAudio_Click);
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(373, 3);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(88, 23);
            this.axWindowsMediaPlayer1.TabIndex = 21;
            this.axWindowsMediaPlayer1.Visible = false;
            // 
            // btnFetch
            // 
            this.btnFetch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFetch.Location = new System.Drawing.Point(3, 3);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(107, 24);
            this.btnFetch.TabIndex = 22;
            this.btnFetch.Text = "Sync LOD";
            this.btnFetch.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 507);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.mmMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mmMain;
            this.Name = "frmMain";
            this.Text = "Konterbont LOD Connector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.mmMain.ResumeLayout(false);
            this.mmMain.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem magazineOpmaachenToolStripMenuItem;
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel tssMagazine;
        private System.Windows.Forms.ToolStripStatusLabel tssArticle;
        private System.Windows.Forms.ToolStripStatusLabel tssNeedSave;
        private System.Windows.Forms.Button btnCustomAudio;
        private System.Windows.Forms.Button btnPlayAudio;
        private System.Windows.Forms.ToolStripMenuItem tsmiText;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.ToolStripMenuItem tssExperimental;
        private System.Windows.Forms.ToolStripMenuItem iNDesignConnectorToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tssInDesign;
        private System.Windows.Forms.ToolStripMenuItem bookContentToolStripMenuItem;
        private System.Windows.Forms.Button btnFetch;
    }
}

