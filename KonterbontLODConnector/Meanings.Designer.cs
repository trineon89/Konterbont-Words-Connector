using System;

namespace KonterbontLODConnector
{
    partial class frmSelectMeaning
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
            this.components = new System.ComponentModel.Container();
            this.gbMeanings = new System.Windows.Forms.GroupBox();
            this.btnAuswielen = new System.Windows.Forms.Button();
            this.tpInfo = new System.Windows.Forms.ToolTip(this.components);
            this.tcLang = new System.Windows.Forms.TabControl();
            this.tpDE = new System.Windows.Forms.TabPage();
            this.tpFR = new System.Windows.Forms.TabPage();
            this.tpEN = new System.Windows.Forms.TabPage();
            this.tpPT = new System.Windows.Forms.TabPage();
            this.rtbPT = new System.Windows.Forms.RichTextBox();
            this.rtbEN = new System.Windows.Forms.RichTextBox();
            this.rtbFR = new System.Windows.Forms.RichTextBox();
            this.rtbDE = new System.Windows.Forms.RichTextBox();
            this.gbMeanings.SuspendLayout();
            this.tcLang.SuspendLayout();
            this.tpDE.SuspendLayout();
            this.tpFR.SuspendLayout();
            this.tpEN.SuspendLayout();
            this.tpPT.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMeanings
            // 
            this.gbMeanings.Controls.Add(this.btnAuswielen);
            this.gbMeanings.Dock = System.Windows.Forms.DockStyle.Left;
            this.gbMeanings.Location = new System.Drawing.Point(0, 0);
            this.gbMeanings.Name = "gbMeanings";
            this.gbMeanings.Size = new System.Drawing.Size(141, 398);
            this.gbMeanings.TabIndex = 0;
            this.gbMeanings.TabStop = false;
            this.gbMeanings.Text = "Bedeitung auswielen:";
            this.gbMeanings.Click += new System.EventHandler(this.gbMeanings_Click);
            // 
            // btnAuswielen
            // 
            this.btnAuswielen.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAuswielen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAuswielen.Location = new System.Drawing.Point(26, 363);
            this.btnAuswielen.Name = "btnAuswielen";
            this.btnAuswielen.Size = new System.Drawing.Size(75, 23);
            this.btnAuswielen.TabIndex = 1;
            this.btnAuswielen.Text = "&Auswielen";
            this.btnAuswielen.UseVisualStyleBackColor = true;
            // 
            // tpInfo
            // 
            this.tpInfo.Popup += new System.Windows.Forms.PopupEventHandler(this.tpInfo_Popup);
            // 
            // tcLang
            // 
            this.tcLang.Controls.Add(this.tpDE);
            this.tcLang.Controls.Add(this.tpFR);
            this.tcLang.Controls.Add(this.tpEN);
            this.tcLang.Controls.Add(this.tpPT);
            this.tcLang.Dock = System.Windows.Forms.DockStyle.Right;
            this.tcLang.Location = new System.Drawing.Point(144, 0);
            this.tcLang.Name = "tcLang";
            this.tcLang.SelectedIndex = 0;
            this.tcLang.Size = new System.Drawing.Size(396, 398);
            this.tcLang.TabIndex = 2;
            // 
            // tpDE
            // 
            this.tpDE.Controls.Add(this.rtbDE);
            this.tpDE.Location = new System.Drawing.Point(4, 22);
            this.tpDE.Name = "tpDE";
            this.tpDE.Padding = new System.Windows.Forms.Padding(3);
            this.tpDE.Size = new System.Drawing.Size(388, 372);
            this.tpDE.TabIndex = 0;
            this.tpDE.Text = "DE";
            this.tpDE.UseVisualStyleBackColor = true;
            // 
            // tpFR
            // 
            this.tpFR.Controls.Add(this.rtbFR);
            this.tpFR.Location = new System.Drawing.Point(4, 22);
            this.tpFR.Name = "tpFR";
            this.tpFR.Padding = new System.Windows.Forms.Padding(3);
            this.tpFR.Size = new System.Drawing.Size(388, 372);
            this.tpFR.TabIndex = 1;
            this.tpFR.Text = "FR";
            this.tpFR.UseVisualStyleBackColor = true;
            this.tpFR.Click += new System.EventHandler(this.tpFR_Click);
            // 
            // tpEN
            // 
            this.tpEN.Controls.Add(this.rtbEN);
            this.tpEN.Location = new System.Drawing.Point(4, 22);
            this.tpEN.Name = "tpEN";
            this.tpEN.Padding = new System.Windows.Forms.Padding(3);
            this.tpEN.Size = new System.Drawing.Size(388, 372);
            this.tpEN.TabIndex = 2;
            this.tpEN.Text = "EN";
            this.tpEN.UseVisualStyleBackColor = true;
            // 
            // tpPT
            // 
            this.tpPT.Controls.Add(this.rtbPT);
            this.tpPT.Location = new System.Drawing.Point(4, 22);
            this.tpPT.Name = "tpPT";
            this.tpPT.Padding = new System.Windows.Forms.Padding(3);
            this.tpPT.Size = new System.Drawing.Size(388, 372);
            this.tpPT.TabIndex = 3;
            this.tpPT.Text = "PT";
            this.tpPT.UseVisualStyleBackColor = true;
            // 
            // rtbPT
            // 
            this.rtbPT.BackColor = System.Drawing.SystemColors.Window;
            this.rtbPT.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbPT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPT.Location = new System.Drawing.Point(3, 3);
            this.rtbPT.Name = "rtbPT";
            this.rtbPT.ReadOnly = true;
            this.rtbPT.Size = new System.Drawing.Size(382, 366);
            this.rtbPT.TabIndex = 0;
            this.rtbPT.Text = "";
            // 
            // rtbEN
            // 
            this.rtbEN.BackColor = System.Drawing.SystemColors.Window;
            this.rtbEN.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbEN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEN.Location = new System.Drawing.Point(3, 3);
            this.rtbEN.Name = "rtbEN";
            this.rtbEN.ReadOnly = true;
            this.rtbEN.Size = new System.Drawing.Size(382, 366);
            this.rtbEN.TabIndex = 0;
            this.rtbEN.Text = "";
            // 
            // rtbFR
            // 
            this.rtbFR.BackColor = System.Drawing.SystemColors.Window;
            this.rtbFR.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbFR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbFR.Location = new System.Drawing.Point(3, 3);
            this.rtbFR.Name = "rtbFR";
            this.rtbFR.ReadOnly = true;
            this.rtbFR.Size = new System.Drawing.Size(382, 366);
            this.rtbFR.TabIndex = 0;
            this.rtbFR.Text = "";
            // 
            // rtbDE
            // 
            this.rtbDE.BackColor = System.Drawing.SystemColors.Window;
            this.rtbDE.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbDE.Location = new System.Drawing.Point(3, 3);
            this.rtbDE.Name = "rtbDE";
            this.rtbDE.ReadOnly = true;
            this.rtbDE.Size = new System.Drawing.Size(382, 366);
            this.rtbDE.TabIndex = 0;
            this.rtbDE.Text = "";
            // 
            // frmSelectMeaning
            // 
            this.AcceptButton = this.btnAuswielen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(540, 398);
            this.Controls.Add(this.tcLang);
            this.Controls.Add(this.gbMeanings);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectMeaning";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bedeitung auswielen";
            this.TopMost = true;
            this.gbMeanings.ResumeLayout(false);
            this.tcLang.ResumeLayout(false);
            this.tpDE.ResumeLayout(false);
            this.tpFR.ResumeLayout(false);
            this.tpEN.ResumeLayout(false);
            this.tpPT.ResumeLayout(false);
            this.ResumeLayout(false);

        }

       

        #endregion
        private System.Windows.Forms.Button btnAuswielen;
        public System.Windows.Forms.GroupBox gbMeanings;
        public System.Windows.Forms.ToolTip tpInfo;
        private System.Windows.Forms.TabPage tpDE;
        private System.Windows.Forms.TabPage tpFR;
        private System.Windows.Forms.TabPage tpEN;
        private System.Windows.Forms.TabPage tpPT;
        public System.Windows.Forms.RichTextBox rtbDE;
        public System.Windows.Forms.RichTextBox rtbFR;
        public System.Windows.Forms.RichTextBox rtbEN;
        public System.Windows.Forms.RichTextBox rtbPT;
        public System.Windows.Forms.TabControl tcLang;
    }
}