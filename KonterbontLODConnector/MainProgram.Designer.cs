﻿namespace KonterbontLODConnector
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Béier", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Béier", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Béier",
            "Bier [Getränk]",
            "bière [boisson]"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Béier",
            "Eber",
            "verrat"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Béier",
            "(Glas) Bier",
            "(verre de) bière"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "(Fläsch) Béier",
            "(Flasche) Bier",
            "(bouteille de) bière"}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainProgram));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnMenuFolder = new System.Windows.Forms.Button();
            this.btnArtikelOpman = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.vistaOpenFileDialog = new Ookii.Dialogs.WinForms.VistaOpenFileDialog();
            this.label_WordsTab_Header = new System.Windows.Forms.Label();
            this.listView_Words = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label_MeaningTab_Header = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gbBasic = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.edtBasisWuert = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbTranslation = new System.Windows.Forms.GroupBox();
            this.gbExamples = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.edtWordform = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelVerb = new System.Windows.Forms.Panel();
            this.edtHelperVerb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.edtParticipePasse = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panelPlural = new System.Windows.Forms.Panel();
            this.edtPlural = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.linkLod = new System.Windows.Forms.LinkLabel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.edtMp3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.richTextWPF1 = new KonterbontLODConnector.RichTextWPF();
            this.panelMenu.SuspendLayout();
            this.panel3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbBasic.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panelVerb.SuspendLayout();
            this.panelPlural.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(1110, 704);
            this.splitContainer1.SplitterDistance = 788;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(318, 704);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Controls.Add(this.label_MeaningTab_Header);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(310, 678);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Meaning";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            // label_WordsTab_Header
            // 
            this.label_WordsTab_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_WordsTab_Header.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WordsTab_Header.Location = new System.Drawing.Point(3, 3);
            this.label_WordsTab_Header.Name = "label_WordsTab_Header";
            this.label_WordsTab_Header.Size = new System.Drawing.Size(304, 51);
            this.label_WordsTab_Header.TabIndex = 0;
            // 
            // listView_Words
            // 
            this.listView_Words.CheckBoxes = true;
            this.listView_Words.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_Words.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup1.Header = "Béier";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "Béier";
            listViewGroup2.Name = "listViewGroup2";
            this.listView_Words.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listView_Words.HideSelection = false;
            listViewItem1.Checked = true;
            listViewItem1.StateImageIndex = 1;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            this.listView_Words.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listView_Words.Location = new System.Drawing.Point(3, 54);
            this.listView_Words.Name = "listView_Words";
            this.listView_Words.Size = new System.Drawing.Size(304, 621);
            this.listView_Words.TabIndex = 1;
            this.listView_Words.UseCompatibleStateImageBehavior = false;
            this.listView_Words.View = System.Windows.Forms.View.Details;
            this.listView_Words.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_Words_ItemCheck);
            this.listView_Words.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_Words_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "LU";
            this.columnHeader1.Width = 107;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "DE";
            this.columnHeader2.Width = 93;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "FR";
            this.columnHeader3.Width = 107;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView_Words);
            this.tabPage1.Controls.Add(this.label_WordsTab_Header);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(310, 678);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Words";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label_MeaningTab_Header
            // 
            this.label_MeaningTab_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_MeaningTab_Header.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_MeaningTab_Header.Location = new System.Drawing.Point(0, 0);
            this.label_MeaningTab_Header.Name = "label_MeaningTab_Header";
            this.label_MeaningTab_Header.Size = new System.Drawing.Size(310, 31);
            this.label_MeaningTab_Header.TabIndex = 1;
            this.label_MeaningTab_Header.Text = "WUEEEEERTTTT";
            this.label_MeaningTab_Header.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gbExamples, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.gbBasic, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbTranslation, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 31);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.76271F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.33898F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.8983F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(310, 647);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // gbBasic
            // 
            this.gbBasic.AutoSize = true;
            this.gbBasic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbBasic.Controls.Add(this.panel6);
            this.gbBasic.Controls.Add(this.panel5);
            this.gbBasic.Controls.Add(this.panelPlural);
            this.gbBasic.Controls.Add(this.panelVerb);
            this.gbBasic.Controls.Add(this.panel4);
            this.gbBasic.Controls.Add(this.panel1);
            this.gbBasic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbBasic.Location = new System.Drawing.Point(3, 3);
            this.gbBasic.Name = "gbBasic";
            this.gbBasic.Size = new System.Drawing.Size(304, 290);
            this.gbBasic.TabIndex = 3;
            this.gbBasic.TabStop = false;
            this.gbBasic.Text = "BasisInfo";
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.edtBasisWuert);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(298, 39);
            this.panel1.TabIndex = 2;
            // 
            // edtBasisWuert
            // 
            this.edtBasisWuert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtBasisWuert.Location = new System.Drawing.Point(89, 9);
            this.edtBasisWuert.Name = "edtBasisWuert";
            this.edtBasisWuert.Size = new System.Drawing.Size(206, 20);
            this.edtBasisWuert.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Basis Wuert";
            // 
            // gbTranslation
            // 
            this.gbTranslation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTranslation.Location = new System.Drawing.Point(3, 299);
            this.gbTranslation.Name = "gbTranslation";
            this.gbTranslation.Size = new System.Drawing.Size(304, 125);
            this.gbTranslation.TabIndex = 4;
            this.gbTranslation.TabStop = false;
            this.gbTranslation.Text = "Iwwersetzungen";
            // 
            // gbExamples
            // 
            this.gbExamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbExamples.Location = new System.Drawing.Point(3, 430);
            this.gbExamples.Name = "gbExamples";
            this.gbExamples.Size = new System.Drawing.Size(304, 214);
            this.gbExamples.TabIndex = 6;
            this.gbExamples.TabStop = false;
            this.gbExamples.Text = "Beispiller";
            // 
            // panel4
            // 
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.Controls.Add(this.edtWordform);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(3, 55);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(298, 39);
            this.panel4.TabIndex = 3;
            // 
            // edtWordform
            // 
            this.edtWordform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtWordform.Location = new System.Drawing.Point(89, 9);
            this.edtWordform.Name = "edtWordform";
            this.edtWordform.Size = new System.Drawing.Size(206, 20);
            this.edtWordform.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "WuertForm";
            // 
            // panelVerb
            // 
            this.panelVerb.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelVerb.Controls.Add(this.edtParticipePasse);
            this.panelVerb.Controls.Add(this.label4);
            this.panelVerb.Controls.Add(this.edtHelperVerb);
            this.panelVerb.Controls.Add(this.label3);
            this.panelVerb.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelVerb.Location = new System.Drawing.Point(3, 94);
            this.panelVerb.Name = "panelVerb";
            this.panelVerb.Size = new System.Drawing.Size(298, 67);
            this.panelVerb.TabIndex = 4;
            // 
            // edtHelperVerb
            // 
            this.edtHelperVerb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtHelperVerb.Location = new System.Drawing.Point(89, 9);
            this.edtHelperVerb.Name = "edtHelperVerb";
            this.edtHelperVerb.Size = new System.Drawing.Size(206, 20);
            this.edtHelperVerb.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Hëllefsverb";
            // 
            // edtParticipePasse
            // 
            this.edtParticipePasse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtParticipePasse.Location = new System.Drawing.Point(89, 39);
            this.edtParticipePasse.Name = "edtParticipePasse";
            this.edtParticipePasse.Size = new System.Drawing.Size(206, 20);
            this.edtParticipePasse.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Participe passé";
            // 
            // panelPlural
            // 
            this.panelPlural.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelPlural.Controls.Add(this.edtPlural);
            this.panelPlural.Controls.Add(this.label6);
            this.panelPlural.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPlural.Location = new System.Drawing.Point(3, 161);
            this.panelPlural.Name = "panelPlural";
            this.panelPlural.Size = new System.Drawing.Size(298, 39);
            this.panelPlural.TabIndex = 7;
            // 
            // edtPlural
            // 
            this.edtPlural.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtPlural.Location = new System.Drawing.Point(89, 9);
            this.edtPlural.Name = "edtPlural";
            this.edtPlural.Size = new System.Drawing.Size(206, 20);
            this.edtPlural.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Pluriel";
            // 
            // panel6
            // 
            this.panel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel6.Controls.Add(this.linkLod);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(3, 239);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(298, 24);
            this.panel6.TabIndex = 9;
            // 
            // linkLod
            // 
            this.linkLod.AutoSize = true;
            this.linkLod.Location = new System.Drawing.Point(3, 3);
            this.linkLod.Name = "linkLod";
            this.linkLod.Size = new System.Drawing.Size(54, 13);
            this.linkLod.TabIndex = 0;
            this.linkLod.TabStop = true;
            this.linkLod.Text = "Lod-Link: ";
            this.linkLod.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLod_LinkClicked);
            // 
            // panel5
            // 
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.Controls.Add(this.edtMp3);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(3, 200);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(298, 39);
            this.panel5.TabIndex = 8;
            // 
            // edtMp3
            // 
            this.edtMp3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtMp3.Location = new System.Drawing.Point(89, 9);
            this.edtMp3.Name = "edtMp3";
            this.edtMp3.Size = new System.Drawing.Size(206, 20);
            this.edtMp3.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "MP3";
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(788, 704);
            this.elementHost1.TabIndex = 2;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.richTextWPF1;
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
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbBasic.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panelVerb.ResumeLayout(false);
            this.panelVerb.PerformLayout();
            this.panelPlural.ResumeLayout(false);
            this.panelPlural.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
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
        private System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView listView_Words;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label_WordsTab_Header;
        private System.Windows.Forms.Label label_MeaningTab_Header;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbExamples;
        private System.Windows.Forms.GroupBox gbBasic;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbTranslation;
        public System.Windows.Forms.TextBox edtBasisWuert;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.TextBox edtWordform;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelVerb;
        public System.Windows.Forms.TextBox edtHelperVerb;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox edtParticipePasse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.LinkLabel linkLod;
        private System.Windows.Forms.Panel panel5;
        public System.Windows.Forms.TextBox edtMp3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelPlural;
        public System.Windows.Forms.TextBox edtPlural;
        private System.Windows.Forms.Label label6;
    }
}