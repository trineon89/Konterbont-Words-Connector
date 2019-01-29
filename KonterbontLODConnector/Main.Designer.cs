namespace KonterbontLODConnector
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
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.btnFetch = new System.Windows.Forms.Button();
            this.rtbTest = new System.Windows.Forms.RichTextBox();
            this.edtWord = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rtbResult
            // 
            this.rtbResult.BackColor = System.Drawing.SystemColors.Window;
            this.rtbResult.Location = new System.Drawing.Point(30, 68);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(780, 238);
            this.rtbResult.TabIndex = 1;
            this.rtbResult.Text = "";
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(735, 29);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(75, 23);
            this.btnFetch.TabIndex = 2;
            this.btnFetch.Text = "&Fetch";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_ClickAsync);
            // 
            // rtbTest
            // 
            this.rtbTest.Location = new System.Drawing.Point(30, 322);
            this.rtbTest.Name = "rtbTest";
            this.rtbTest.Size = new System.Drawing.Size(533, 178);
            this.rtbTest.TabIndex = 5;
            this.rtbTest.Text = "";
            // 
            // edtWord
            // 
            this.edtWord.Location = new System.Drawing.Point(30, 31);
            this.edtWord.Name = "edtWord";
            this.edtWord.Size = new System.Drawing.Size(684, 20);
            this.edtWord.TabIndex = 6;
            this.edtWord.Text = "Persoun";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1401, 791);
            this.Controls.Add(this.edtWord);
            this.Controls.Add(this.rtbTest);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.rtbResult);
            this.Name = "Main";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.RichTextBox rtbTest;
        private System.Windows.Forms.TextBox edtWord;
    }
}

