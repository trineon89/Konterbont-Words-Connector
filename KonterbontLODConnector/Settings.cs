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

        private string articlePath = @"\\cubecluster01\Konterbont_Produktioun\Artikelen";

        public int MenuFoldedWidth { get => menuFoldedWidth; set => menuFoldedWidth = value; }
        public int MenuUnFoldedWidth { get => menuUnFoldedWidth; set => menuUnFoldedWidth = value; }
        public string ArticlePath { get => articlePath; set => articlePath = value; }

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
    }
}
