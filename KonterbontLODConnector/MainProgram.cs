﻿using KonterbontLODConnector.forms;
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

        public ArticleFile _articleFile;
        public Article _article;

        private static frmMainProgram instance = null;
        public static frmMainProgram getInstance()
        {
            return instance;
        }

        #region variables

        private bool _Menufolded = false;

        public static Settings Settings { get => settings; set => settings = value; }

        #endregion

        #region public functions

        public void openArticle(string articlePath)
        {
            //check if articleFile exists
            if (System.IO.File.Exists(articlePath))
            {
                // Load aricleFile
                _articleFile = ArticleFile.LoadFromFile(new System.IO.DirectoryInfo(articlePath).Parent.FullName);
            } else
            {
                //Create new articleFile
                _articleFile = new ArticleFile(new System.IO.DirectoryInfo(articlePath).Parent.FullName);
                _articleFile.SaveToFile();
                Console.WriteLine(_articleFile.ToString());
            }

            _article = _articleFile.article;

            LoadArticleInText();
        }

        private void LoadArticleInText()
        {
            RichTextFormatter.Article = _article;

            //rbText.LoadFile(_article.RtfPath);
            RichTextFormatter.LoadArticle(_article.RtfPath);
            RichTextFormatter.Decorate();
            RichTextFormatter.ReDecorate();
        }

        public frmMainProgram()
        {
            InitializeComponent();
            instance = this;
            Settings = new Settings();
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
                panelMenu.Width = Settings.MenuFoldedWidth;
            }
            else
            {
                btnMenuFolder.Text = "<<";
                panelMenu.Width = Settings.MenuUnFoldedWidth;
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
            Settings.ShowDialog();
        }


    

        private void btnMenuFolder_Click(object sender, EventArgs e)
        {
            foldMenu();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnArtikelOpman_Click(object sender, EventArgs e)
        {
            ArticleSelector articleSelector = new ArticleSelector();

            Helpers _helper = new Helpers();
            List<ArticleFile> articleFiles = _helper.getArticles();

            articleSelector.ClearItemsInView();

            foreach (ArticleFile articleFile in articleFiles)
            {
                articleSelector.AddItemToView(articleFile);
            }

            DialogResult ar = articleSelector.ShowDialog();
            if (ar == DialogResult.OK)
            {
                string selectedItem = articleSelector.listView1.SelectedItems[0].Tag as string;
                openArticle(selectedItem + @"\" + articleSelector.listView1.SelectedItems[0].Name);
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

                //MessageBox.Show("Clicked on the Word: " + RichTextFormatter.activeWord);

                if (_article._Words.ContainsKey(RichTextFormatter.activeWord) )
                {
                    
                } else
                {
                    classes.WordOverview wo = new classes.WordOverview();
                    _articleFile.article._Words.Add(RichTextFormatter.activeWord, wo);
                    _articleFile.SaveToFile();
                }

                /*
                 * Check State
                 */
            }
        }

        #endregion
    }

    public class Helpers
    {
       //
       public List<ArticleFile> getArticles()
        {
            List<ArticleFile> res = new List<ArticleFile>();

            string path = frmMainProgram.Settings.GetArticlePath();
            var dirs = from dir in System.IO.Directory.EnumerateDirectories(path, "????_*", System.IO.SearchOption.TopDirectoryOnly) select dir;
            
            foreach (var dir in dirs)
            {
                ArticleFile _articleFile = new ArticleFile(dir);
                res.Add(_articleFile);
            }

            return res;
        }
    }
}
