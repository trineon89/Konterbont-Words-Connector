namespace Konterbont_Words_Connector
{
    partial class frmCloseDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnContinueClose = new System.Windows.Forms.Button();
            this.btnCancelClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "D\'Popups sinn na net an de Magazin kopéiert ginn.      Bass de der secher dass de" +
    " wells zoumaachen?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnContinueClose
            // 
            this.btnContinueClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnContinueClose.Location = new System.Drawing.Point(15, 47);
            this.btnContinueClose.Name = "btnContinueClose";
            this.btnContinueClose.Size = new System.Drawing.Size(113, 34);
            this.btnContinueClose.TabIndex = 1;
            this.btnContinueClose.Text = "Jo, zoumaachen";
            this.btnContinueClose.UseVisualStyleBackColor = true;
            // 
            // btnCancelClose
            // 
            this.btnCancelClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelClose.Location = new System.Drawing.Point(162, 47);
            this.btnCancelClose.Name = "btnCancelClose";
            this.btnCancelClose.Size = new System.Drawing.Size(127, 34);
            this.btnCancelClose.TabIndex = 2;
            this.btnCancelClose.Text = "Nee, oofbriechen";
            this.btnCancelClose.UseVisualStyleBackColor = true;
            // 
            // frmCloseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 108);
            this.Controls.Add(this.btnCancelClose);
            this.Controls.Add(this.btnContinueClose);
            this.Controls.Add(this.label1);
            this.Name = "frmCloseDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Secher dass de wells zoumaachen?";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnContinueClose;
        private System.Windows.Forms.Button btnCancelClose;
    }
}