using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KonterbontLODConnector
{
    public partial class frmColor : Form
    {
        public Color ArticleColor = Color.Black;
        public Color OArticleColor = Color.Black;
        
        public frmColor()
        {
            InitializeComponent();
        }

        private void RbEegen_CheckedChanged(object sender, EventArgs e)
        {
            btnSelectColor.Enabled = true;
            pnlColor.BackColor = ArticleColor;
        }

        private void RbArtikel_CheckedChanged(object sender, EventArgs e)
        {
            btnSelectColor.Enabled = false;
            pnlColor.BackColor = OArticleColor;
        }

        private void BtnSelectColor_Click(object sender, EventArgs e)
        {
            dlgColor.Color = pnlColor.BackColor;
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                pnlColor.BackColor = dlgColor.Color;
            }
        }

        private void FrmColor_Load(object sender, EventArgs e)
        {

        }
    }
}
