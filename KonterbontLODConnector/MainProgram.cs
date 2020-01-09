using KonterbontLODConnector.forms;
using KonterbontLODConnector.classes;
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
using System.Net.Http;

namespace KonterbontLODConnector
{
    public partial class frmMainProgram : Form
    {
        private static Settings settings;

        public ArticleFile _articleFile;

        private static frmMainProgram instance = null;
        public static frmMainProgram getInstance()
        {
            return instance;
        }

        public static Settings getSettings()
        {
            return settings;
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

            LoadArticleInText();
        }

        private void LoadArticleInText()
        {
            RichTextFormatter.Article = _articleFile.article;

            //rbText.LoadFile(_article.RtfPath);
            RichTextFormatter.LoadArticle(_articleFile);
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
            splitContainer1.Panel2Collapsed = true;
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

                _articleFile.ArticleFileName = articleSelector.activeArticleFile.ArticleFileName;
                _articleFile.ArticleId = articleSelector.activeArticleFile.ArticleId;
                _articleFile.ArticleName = articleSelector.activeArticleFile.ArticleName;
                _articleFile.ArticlePath = articleSelector.activeArticleFile.ArticlePath;
            }
        }

        /*
         *  Call from WPF ->
         */
        public async void RichTextBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //check what item is clicked
            if (RichTextFormatter.getClickedWord() != null)
            {
                //clicked on a marked Word

                //MessageBox.Show("Clicked on the Word: " + RichTextFormatter.activeWord);

                if (_articleFile.article._Words.ContainsKey(RichTextFormatter.activeWord) )
                {
                    //
                    splitContainer1.Panel2Collapsed = false;
                } else
                {
                    classes.WordOverview wo = new classes.WordOverview();

                    var task = checkLodForWord(wo, RichTextFormatter.activeWord);
                    wo = await task;

                    _articleFile.article._Words.Add(RichTextFormatter.activeWord, wo);

                    _articleFile.SaveToFile();
                }

                /*
                 * Check State
                 */
            }
        }

        #endregion

        private async Task<WordOverview> checkLodForWord(WordOverview wo, string occurence)
        {
            /*
            check LOD
            */
            Task<WordOverview> task = getLodWords(wo, occurence);
            wo = await task;
            return wo;

            /*
            Task<WordOverview> task = Task.Run<WordOverview>(async () => await getLodOccurences(wo, occurence).ConfigureAwait(true));
            wo = task.Result;

            if (wo == null)
                return null;

            if (wo.valid)
            {
                //wo._wordMeanings = getLodForWord(wo);
            }

            return wo;
            */
        }

        private async Task<WordOverview> getLodWords(WordOverview wo, string occ)
        {
            if (wo==null)
            {
                wo = new WordOverview();
            }

            Implementation.LodApiResults lodResults = new Implementation.LodApiResults();
            lodResults.wordBase = _articleFile.article._WordBase;

            var xmlTask = lodResults.GetXML(occ);
            string xmlRes = await xmlTask;
            (List<string> xml, List<string> mp3, List<string> occs, Boolean valid) = lodResults.ReturnBaseXmlData(xmlRes);

            int i = 0;
            foreach (var thexml in xml)
            {
                Word _word = new Word();
                _word.occurence = occs[i];
                _word.wordBasePointer = thexml;

                i++;
                wo._wordPossibleMeanings.Add(_word);
            }

            var tt = lodResults.insertBaseWords(xml, mp3, occs);
            await tt;

            return wo;
        }

        private async Task<WordOverview> getLodOccurences(WordOverview wo, string occurence)
        {
            HttpClient httpClient = new HttpClient();
            string responseBody = null;
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://api.lod.lu/php/api.php?m=s&t=os&q={\"t\":\"" + occurence + "\"}"),
                Method = HttpMethod.Get
            };

            try
            {
                var _response = await httpClient.SendAsync(httpContent);
                responseBody = await _response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            httpClient.Dispose();

            Implementation.LodApiResults lodApiResults = new Implementation.LodApiResults(responseBody);

            lodApiResults.wordBase = _articleFile.article._WordBase;

            //Fill WordMeanings
            wo = lodApiResults.FillResults(wo);
            Console.WriteLine("");
            //Select BaseWord

            return wo;
        }

        private List<Word> getLodForWord(WordOverview wo)
        {
            List < Word > lwo = new List<Word>();

            return lwo;
        }
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
