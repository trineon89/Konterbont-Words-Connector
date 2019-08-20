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

            rtbDE.SelectAll();
            rtbDE.SelectionColor = Color.Black;
            rtbDE.SelectionLength = 0;
            if (Int32.Parse(Selection) < 10)
            {
                Selection = "0" + Selection;
            }
            foreach (string line in rtbDE.Lines)
            {
                if (Regex.IsMatch(line, Selection + ". "))
                {
                    Color myRgbColor = Color.FromArgb(20, 118, 212);
                    Utility.HighlightText(rtbDE, line, myRgbColor);
                }
            }

            rtbFR.SelectAll();
            rtbFR.SelectionColor = Color.Black;
            rtbFR.SelectionLength = 0;
            foreach (string line in rtbFR.Lines)
            {
                if (Regex.IsMatch(line, Selection + ". "))
                {
                    Color myRgbColor = Color.FromArgb(20, 118, 212);
                    Utility.HighlightText(rtbFR, line, myRgbColor);
                }
            }

            rtbEN.SelectAll();
            rtbEN.SelectionColor = Color.Black;
            rtbEN.SelectionLength = 0;
            foreach (string line in rtbEN.Lines)
            {
                if (Regex.IsMatch(line, Selection + ". "))
                {
                    Color myRgbColor = Color.FromArgb(20, 118, 212);
                    Utility.HighlightText(rtbEN, line, myRgbColor);
                }
            }

            rtbPT.SelectAll();
            rtbPT.SelectionColor = Color.Black;
            rtbPT.SelectionLength = 0;
            foreach (string line in rtbPT.Lines)
            {
                if (Regex.IsMatch(line, Selection + ". "))
                {
                    Color myRgbColor = Color.FromArgb(20, 118, 212);
                    Utility.HighlightText(rtbPT, line, myRgbColor);
                }
            }
        }
    }

    static class Utility
    {
        public static Color LastColor { get; set; }

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

        public static void HighlightSelText(this RichTextBox myRtb, string word)
        {

            if (word == string.Empty)
                return;
            Color myRgbColor = Color.FromArgb(128, 20, 118, 212);
            int s_start = myRtb.SelectionStart, startIndex = 0, index;

            while ((index = myRtb.Text.IndexOf(word, startIndex)) != -1)
            {
                myRtb.Select(index, word.Length);

                if (myRtb.SelectionBackColor.A != 255 || myRtb.SelectionBackColor.R != 255 
                    || myRtb.SelectionBackColor.G != 255 || myRtb.SelectionBackColor.B != 255)
                {
                    LastColor = myRtb.SelectionBackColor;
                    myRtb.SelectionBackColor = myRgbColor;
                    return;
                }

                startIndex = index + word.Length;
            }
            myRtb.SelectionStart = s_start;
            myRtb.SelectionLength = 0;
        }

        public static void UnSelText(this RichTextBox myRtb)
        {
            myRtb.SelectionBackColor = LastColor;
            myRtb.SelectionLength = 0;
        }
    }
}
