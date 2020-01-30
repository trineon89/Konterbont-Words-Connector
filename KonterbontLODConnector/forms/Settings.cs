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
using System.Windows.Forms;

namespace KonterbontLODConnector
{
    public partial class Settings : Form
    {
        private int langId;

        private int menuFoldedWidth = 30;
        private int menuUnFoldedWidth = 240;

        private string magazinePath = @"\\cubecluster01\Konterbont_Produktioun\Magazines\";
        private string mp3Path = @"\\cubecluster01\Konterbont_Audio";
        private string articlePath = @"\\cubecluster01\Konterbont_Produktioun\Artikelen";

        public int MenuFoldedWidth { get => menuFoldedWidth; set => menuFoldedWidth = value; }
        public int MenuUnFoldedWidth { get => menuUnFoldedWidth; set => menuUnFoldedWidth = value; }
        public string MagazinePath { get => magazinePath; set => magazinePath = value; }
        public string ArticlePath { get => articlePath; set => articlePath = value; }
        public string Mp3Path { get => mp3Path; set => mp3Path = value; }

        public Settings()
        {
            InitializeComponent();
            FillLanguageCombobox();
        }

        public string GetArticlePath()
        {
            if (System.IO.Directory.Exists(articlePath))
            {
                return articlePath;
            } else
            {
                MessageBox.Show("Wiel den Dossier aus, an deem alleguerten d'Artikele léien.","Articles not found");
                Ookii.Dialogs.WinForms.VistaFolderBrowserDialog folderBrowserDialog = new Ookii.Dialogs.WinForms.VistaFolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    articlePath = folderBrowserDialog.SelectedPath;
                }
                return articlePath;
            }
        }

        public string GetMagazinePath()
        {
            if (System.IO.Directory.Exists(magazinePath))
            {
                return magazinePath;
            }
            else
            {
                MessageBox.Show("Wiel den Dossier aus, an deem alleguerten d'Magazinne léien.", "Magazines not found");
                Ookii.Dialogs.WinForms.VistaFolderBrowserDialog folderBrowserDialog = new Ookii.Dialogs.WinForms.VistaFolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    magazinePath = folderBrowserDialog.SelectedPath;
                }
                return magazinePath;
            }
        }

        private void FillLanguageCombobox()
        {
            cbLanguage.Items.Add("(Default)");
            cbLanguage.Items.Add("Luxemburgish");
        }

        public string GetLanguage()
        {
            string language = null;

            switch (langId)
            {
                case 0:
                    language = "en-US";
                    break;
                case 1:
                    language = "lb";
                    break;
                default:
                    language = "en-US";
                    break;
            }

            return language;
        }

        private void btnImportOldContent_Click(object sender, EventArgs e)
        {
            /*
             *  Import
             */
        }

        private void btnCleanupArticle_Click(object sender, EventArgs e)
        {
            /*
             *  Cleanup Article
             */
        }
    }
}
