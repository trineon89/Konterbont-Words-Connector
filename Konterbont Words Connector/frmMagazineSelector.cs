using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Konterbont_Words_Connector
{
    public partial class frmMagazineSelector : Form
    {
        private frmMain other;

        public frmMagazineSelector()
        {
            InitializeComponent();
        }

        public frmMagazineSelector(frmMain other)
        {
            this.other = other;
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
