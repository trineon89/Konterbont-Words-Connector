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

        public void gbMeanings_Click(object sender, EventArgs e)
        {
            RadioButton selectedMeaning = gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            string Selection = selectedMeaning.Name;
            foreach (string line in rtbDE.Lines)
            {
                if (Regex.IsMatch(line, Selection + ". "))
                {
                    Utility.HighlightText(rtbDE, line, Color.Blue);
                }
            }
        }
    }

    static class Utility { 

        public static void HighlightText(this RichTextBox myRtb, string word, Color color)
        {

            if (word == string.Empty)
                return;

            int s_start = myRtb.SelectionStart, startIndex = 0, index;

            while ((index = myRtb.Text.IndexOf(word, startIndex)) != -1)
            {
                myRtb.Select(index, word.Length);
                myRtb.SelectionColor = color;

                startIndex = index + word.Length;
            }

            myRtb.SelectionStart = s_start;
            myRtb.SelectionLength = 0;
            myRtb.SelectionColor = Color.Black;
    }
    }
}
