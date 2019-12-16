using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace KonterbontLODConnector
{
    static class RichTextFormatter
    {
        private static RichTextBox richTextBox;
        private static Article article;

        public static RichTextBox RichTextBox { get => richTextBox; set => richTextBox = value; }
        public static Article Article { get => article; set => article = value; }

        public static void ReDecorate()
        {

            int pos = 0;
            foreach (string ln in richTextBox.Lines)
            {
                
            }
        }
    }

    public class RichText_WinformHost : System.Windows.Forms.Integration.ElementHost
    {
        protected RichTextWPF m_richTextWPF = new RichTextWPF();

        public RichText_WinformHost()
        {
            base.Child = m_richTextWPF;
        }

        public FlowDocument flowDocument()
        {
            return m_richTextWPF.richTextBox.Document;
        }

        public System.Windows.Controls.RichTextBox getWPFRichTextBox()
        {
            return m_richTextWPF.richTextBox;
        }
    }
}
