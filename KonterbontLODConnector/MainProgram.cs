using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using wpf = System.Windows.Controls;

namespace KonterbontLODConnector
{
    public partial class frmMainProgram : Form
    {
        private static Settings settings;

        private Article _article;

        private static frmMainProgram instance = null;
        public static frmMainProgram getInstance()
        {
            return instance;
        }

    #region variables

        private bool _Menufolded = false;

#endregion

    #region public functions

        public void openArticle(string articlePath)
        {
            //MessageBox.Show("Loading article: "+articlePath, "Yes!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _article = new Article(articlePath);

            LoadArticleInText();
        }

        private void LoadArticleInText()
        {
            RichTextFormatter.Article = _article;

            //rbText.LoadFile(_article.RtfPath);
            RichTextFormatter.LoadArticle(_article.RtfPath);

            RichTextFormatter.ReDecorate();
        }

        public frmMainProgram()
        {
            InitializeComponent();
            instance = this;
            settings = new Settings();
            InitReset();

            RichTextFormatter.elementHost = elementHost1;
            RichTextFormatter.LoadRtfHandler(richTextWPF1.richTextBox);

        }

        #endregion

        #region UI Buttons/Actions

        private void InitReset()
        {
            //toolStripStatusLabel.Visible = true;
            toolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
        }

        private void foldMenu()
        {
            _Menufolded = !_Menufolded;
            foldMenu(_Menufolded);
        }

        private void foldMenu(bool fold)
        {
            HideMenuButtons(!fold);

            if (_Menufolded)
            {
                btnMenuFolder.Text = ">>";
                panelMenu.Width = settings.MenuFoldedWidth;
            }
            else
            {
                btnMenuFolder.Text = "<<";
                panelMenu.Width = settings.MenuUnFoldedWidth;
            }
        }

        private void HideMenuButtons(bool hide)
        {
            btnArtikelOpman.Visible = hide;
        }

        private void artikelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //New Article Click
        }

        private void opmachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open Article Click
        }

        private void magazinnOpmachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open Magazine Click
        }

        private void zoumachenxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close Click
            Application.Exit();
        }

        private void programmAstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open Settings
            settings.ShowDialog();
        }


#endregion

        private void btnMenuFolder_Click(object sender, EventArgs e)
        {
            foldMenu();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnArtikelOpman_Click(object sender, EventArgs e)
        {
            //Open File
            if (vistaOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                openArticle(vistaOpenFileDialog.FileName);
            }
        }

        /*
         *  Call from WPF ->
         */
        public void RichTextBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //check what item is clicked
            if (RichTextFormatter.getClickedWord() != null)
            {
                //clicked on a marked Word

                MessageBox.Show("Clicked on the Word: " + RichTextFormatter.activeWord);

                /*
                 * Check State
                 */
            }
        }
    }

    public class Helpers
    {
       //
    }
}
