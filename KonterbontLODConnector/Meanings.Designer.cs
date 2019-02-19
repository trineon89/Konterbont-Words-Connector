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
            this.gbMeanings.SuspendLayout();
            this.tcLang.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMeanings
            // 
            this.gbMeanings.AutoSize = true;
            this.gbMeanings.Controls.Add(this.tcLang);
            this.gbMeanings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbMeanings.Location = new System.Drawing.Point(0, 0);
            this.gbMeanings.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.gbMeanings.Name = "gbMeanings";
            this.gbMeanings.Padding = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.gbMeanings.Size = new System.Drawing.Size(466, 268);
            this.gbMeanings.TabIndex = 0;
            this.gbMeanings.TabStop = false;
            this.gbMeanings.Text = "Bedeitung auswielen:";
            // 
            // btnAuswielen
            // 
            this.btnAuswielen.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAuswielen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAuswielen.Location = new System.Drawing.Point(196, 233);
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
            this.tcLang.Location = new System.Drawing.Point(11, 114);
            this.tcLang.Name = "tcLang";
            this.tcLang.SelectedIndex = 0;
            this.tcLang.Size = new System.Drawing.Size(441, 110);
            this.tcLang.TabIndex = 0;
            // 
            // tpDE
            // 
            this.tpDE.Location = new System.Drawing.Point(4, 22);
            this.tpDE.Name = "tpDE";
            this.tpDE.Padding = new System.Windows.Forms.Padding(3);
            this.tpDE.Size = new System.Drawing.Size(433, 84);
            this.tpDE.TabIndex = 0;
            this.tpDE.Text = "DE";
            this.tpDE.UseVisualStyleBackColor = true;
            // 
            // tpFR
            // 
            this.tpFR.Location = new System.Drawing.Point(4, 22);
            this.tpFR.Name = "tpFR";
            this.tpFR.Padding = new System.Windows.Forms.Padding(3);
            this.tpFR.Size = new System.Drawing.Size(433, 177);
            this.tpFR.TabIndex = 1;
            this.tpFR.Text = "FR";
            this.tpFR.UseVisualStyleBackColor = true;
            // 
            // tpEN
            // 
            this.tpEN.Location = new System.Drawing.Point(4, 22);
            this.tpEN.Name = "tpEN";
            this.tpEN.Padding = new System.Windows.Forms.Padding(3);
            this.tpEN.Size = new System.Drawing.Size(433, 177);
            this.tpEN.TabIndex = 2;
            this.tpEN.Text = "EN";
            this.tpEN.UseVisualStyleBackColor = true;
            // 
            // tpPT
            // 
            this.tpPT.Location = new System.Drawing.Point(4, 22);
            this.tpPT.Name = "tpPT";
            this.tpPT.Padding = new System.Windows.Forms.Padding(3);
            this.tpPT.Size = new System.Drawing.Size(433, 177);
            this.tpPT.TabIndex = 3;
            this.tpPT.Text = "PT";
            this.tpPT.UseVisualStyleBackColor = true;
            // 
            // frmSelectMeaning
            // 
            this.AcceptButton = this.btnAuswielen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(466, 268);
            this.Controls.Add(this.btnAuswielen);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAuswielen;
        public System.Windows.Forms.GroupBox gbMeanings;
        public System.Windows.Forms.ToolTip tpInfo;
        private System.Windows.Forms.TabControl tcLang;
        private System.Windows.Forms.TabPage tpDE;
        private System.Windows.Forms.TabPage tpFR;
        private System.Windows.Forms.TabPage tpEN;
        private System.Windows.Forms.TabPage tpPT;
    }
}