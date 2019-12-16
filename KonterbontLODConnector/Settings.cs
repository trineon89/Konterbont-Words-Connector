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

        public int MenuFoldedWidth { get => menuFoldedWidth; set => menuFoldedWidth = value; }
        public int MenuUnFoldedWidth { get => menuUnFoldedWidth; set => menuUnFoldedWidth = value; }

        public Settings()
        {
            InitializeComponent();
            FillLanguageCombobox();
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
