namespace WindowsFormsApp1
{
    partial class Main
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
            this.edtXML = new System.Windows.Forms.TextBox();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.btnFetch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // edtXML
            // 
            this.edtXML.Location = new System.Drawing.Point(44, 28);
            this.edtXML.Name = "edtXML";
            this.edtXML.Size = new System.Drawing.Size(684, 20);
            this.edtXML.TabIndex = 0;
            // 
            // rtbResult
            // 
            this.rtbResult.BackColor = System.Drawing.SystemColors.Window;
            this.rtbResult.Location = new System.Drawing.Point(44, 73);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(780, 576);
            this.rtbResult.TabIndex = 1;
            this.rtbResult.Text = "";
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(749, 28);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(75, 23);
            this.btnFetch.TabIndex = 2;
            this.btnFetch.Text = "&Fetch";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_ClickAsync);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1401, 791);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.edtXML);
            this.Name = "Main";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox edtXML;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.Button btnFetch;
    }
}

