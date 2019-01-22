namespace Konterbont_Words_Connector
{
    partial class frmMagazineSelector
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
            this.cbMagazine = new System.Windows.Forms.ComboBox();
            this.lblMText = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbMagazine
            // 
            this.cbMagazine.FormattingEnabled = true;
            this.cbMagazine.Location = new System.Drawing.Point(116, 6);
            this.cbMagazine.Name = "cbMagazine";
            this.cbMagazine.Size = new System.Drawing.Size(138, 21);
            this.cbMagazine.TabIndex = 0;
            // 
            // lblMText
            // 
            this.lblMText.AutoSize = true;
            this.lblMText.Location = new System.Drawing.Point(12, 9);
            this.lblMText.Name = "lblMText";
            this.lblMText.Size = new System.Drawing.Size(98, 13);
            this.lblMText.TabIndex = 1;
            this.lblMText.Text = "Konterbont Ausgab";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(83, 33);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Späicheren";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmMagazineSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 69);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblMText);
            this.Controls.Add(this.cbMagazine);
            this.Name = "frmMagazineSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmMagazineSelector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbMagazine;
        private System.Windows.Forms.Label lblMText;
        private System.Windows.Forms.Button btnSave;
    }
}