using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KonterbontLODConnector
{
    public partial class frmSelectMeaning : Form
    {
        public frmSelectMeaning()
        {
            InitializeComponent();
        }

        private void tpInfo_Popup(object sender, PopupEventArgs e)
        {

        }

        private void tpFR_Click(object sender, EventArgs e)
        {

        }

        private void gbMeanings_Click(object sender, EventArgs e)
        {
            RadioButton selectedMeaning = gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            string Selection = selectedMeaning.Name;
            foreach (string line in rtbDE.Lines)
            {
                if (Regex.IsMatch(line, Selection + ". "))
                {
                    rtbDE. 
                }
            }
        }
    }
}
