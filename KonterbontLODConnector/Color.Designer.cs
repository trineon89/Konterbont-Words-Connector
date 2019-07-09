namespace KonterbontLODConnector
{
    partial class frmColor
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
            this.pnlColor = new System.Windows.Forms.Panel();
            this.rbArtikel = new System.Windows.Forms.RadioButton();
            this.rbEegen = new System.Windows.Forms.RadioButton();
            this.lblColorDesc = new System.Windows.Forms.Label();
            this.btnSelectColor = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dlgColor = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // pnlColor
            // 
            this.pnlColor.BackColor = System.Drawing.Color.Black;
            this.pnlColor.Location = new System.Drawing.Point(81, 50);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(50, 24);
            this.pnlColor.TabIndex = 0;
            // 
            // rbArtikel
            // 
            this.rbArtikel.AutoSize = true;
            this.rbArtikel.Location = new System.Drawing.Point(32, 12);
            this.rbArtikel.Name = "rbArtikel";
            this.rbArtikel.Size = new System.Drawing.Size(54, 17);
            this.rbArtikel.TabIndex = 1;
            this.rbArtikel.TabStop = true;
            this.rbArtikel.Text = "Artikel";
            this.rbArtikel.UseVisualStyleBackColor = true;
            this.rbArtikel.CheckedChanged += new System.EventHandler(this.RbArtikel_CheckedChanged);
            // 
            // rbEegen
            // 
            this.rbEegen.AutoSize = true;
            this.rbEegen.Location = new System.Drawing.Point(113, 12);
            this.rbEegen.Name = "rbEegen";
            this.rbEegen.Size = new System.Drawing.Size(56, 17);
            this.rbEegen.TabIndex = 2;
            this.rbEegen.TabStop = true;
            this.rbEegen.Text = "Eegen";
            this.rbEegen.UseVisualStyleBackColor = true;
            this.rbEegen.CheckedChanged += new System.EventHandler(this.RbEegen_CheckedChanged);
            // 
            // lblColorDesc
            // 
            this.lblColorDesc.AutoSize = true;
            this.lblColorDesc.Location = new System.Drawing.Point(9, 55);
            this.lblColorDesc.Name = "lblColorDesc";
            this.lblColorDesc.Size = new System.Drawing.Size(67, 13);
            this.lblColorDesc.TabIndex = 3;
            this.lblColorDesc.Text = "Aktuel Faarf:";
            // 
            // btnSelectColor
            // 
            this.btnSelectColor.Enabled = false;
            this.btnSelectColor.Location = new System.Drawing.Point(137, 50);
            this.btnSelectColor.Name = "btnSelectColor";
            this.btnSelectColor.Size = new System.Drawing.Size(32, 23);
            this.btnSelectColor.TabIndex = 4;
            this.btnSelectColor.Text = "...";
            this.btnSelectColor.UseVisualStyleBackColor = true;
            this.btnSelectColor.Click += new System.EventHandler(this.BtnSelectColor_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(13, 97);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(94, 97);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Ofbriechen";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // frmColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(196, 132);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectColor);
            this.Controls.Add(this.lblColorDesc);
            this.Controls.Add(this.rbEegen);
            this.Controls.Add(this.rbArtikel);
            this.Controls.Add(this.pnlColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmColor";
            this.Text = "Faarf auswielen";
            this.Load += new System.EventHandler(this.FrmColor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblColorDesc;
        private System.Windows.Forms.Button btnSelectColor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ColorDialog dlgColor;
        public System.Windows.Forms.Panel pnlColor;
        public System.Windows.Forms.RadioButton rbArtikel;
        public System.Windows.Forms.RadioButton rbEegen;
    }
}