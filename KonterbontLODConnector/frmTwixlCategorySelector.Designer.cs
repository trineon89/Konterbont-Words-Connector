namespace KonterbontLODConnector
{
    partial class frmTwixlCategorySelector
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbCategories = new System.Windows.Forms.CheckedListBox();
            this.btnSelectCategory = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.cbCategories, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSelectCategory, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(436, 273);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cbCategories
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.cbCategories, 2);
            this.cbCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbCategories.FormattingEnabled = true;
            this.cbCategories.Location = new System.Drawing.Point(3, 3);
            this.cbCategories.Name = "cbCategories";
            this.tableLayoutPanel1.SetRowSpan(this.cbCategories, 2);
            this.cbCategories.Size = new System.Drawing.Size(430, 226);
            this.cbCategories.TabIndex = 0;
            // 
            // btnSelectCategory
            // 
            this.btnSelectCategory.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelectCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSelectCategory.Location = new System.Drawing.Point(3, 235);
            this.btnSelectCategory.Name = "btnSelectCategory";
            this.btnSelectCategory.Size = new System.Drawing.Size(212, 35);
            this.btnSelectCategory.TabIndex = 1;
            this.btnSelectCategory.Text = "Auswielen";
            this.btnSelectCategory.UseVisualStyleBackColor = true;
            this.btnSelectCategory.Click += new System.EventHandler(this.BtnSelectCategory_Click);
            // 
            // frmTwixlCategorySelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 273);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmTwixlCategorySelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kategorie auswielen";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSelectCategory;
        public System.Windows.Forms.CheckedListBox cbCategories;
    }
}