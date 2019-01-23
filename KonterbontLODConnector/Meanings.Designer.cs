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
            this.gbMeanings = new System.Windows.Forms.GroupBox();
            this.btnAuswielen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gbMeanings
            // 
            this.gbMeanings.AutoSize = true;
            this.gbMeanings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbMeanings.Location = new System.Drawing.Point(0, 0);
            this.gbMeanings.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.gbMeanings.Name = "gbMeanings";
            this.gbMeanings.Padding = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.gbMeanings.Size = new System.Drawing.Size(466, 268);
            this.gbMeanings.TabIndex = 0;
            this.gbMeanings.TabStop = false;
            this.gbMeanings.Text = "Meanings";
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
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectMeaning";
            this.ShowInTaskbar = false;
            this.Text = "Select Meaning";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAuswielen;
        public System.Windows.Forms.GroupBox gbMeanings;
    }
}