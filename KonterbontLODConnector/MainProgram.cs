﻿using KonterbontLODConnector.forms;
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
using System.Diagnostics;

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
            RichTextFormatter.ResetWordList();
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
            InitReset();

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
            //splitContainer1.Panel2Collapsed = true;

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

                //Fill splitContainer Content

                FillsplitContainerContent();
                splitContainer1.Panel2Collapsed = false;

                /*
                 * Check State
                 */
            }
        }

        public void ForwardStateTo(int newState)
        {
            classes.WordOverview wo = new WordOverview();
            _articleFile.article._Words.TryGetValue(RichTextFormatter.activeWord, out wo);
            wo.state = newState;
            RichTextFormatter.ReDecorate();
        }

        #endregion

        private void FillsplitContainerContent()
        {
            classes.WordOverview wo = new WordOverview();
            _articleFile.article._Words.TryGetValue(RichTextFormatter.activeWord, out wo);

            
            if (wo._wordPossibleMeanings.Count > 1)
            {
                //Mei ewei 1 basiswuert
                FillWordsTab(wo);
            }
            else
            {
                //sprang direkt bei d'Meaning Selection
                wo.WordPointer = 1;
                FillWordsTab(wo);
            }

            //splitContainer1.Panel2.Controls.Add();
        }

        private void FillsplitContainerContent_MeaningsByWordBaseString(string wordBase, int meaningPointer, WordOverview wo)
        {
            classes.WordBase wb = new WordBase();
            bool wbExists = _articleFile.article._WordBase.TryGetValue(wordBase, out wb);
            if (wbExists)
            {
                if (wb.meanings.Count > 1)
                {
                    //Mei ewei 1 Meaning
                } else
                {
                    wo._wordPossibleMeanings[wo.WordPointer - 1].meaningPointer = 1;
                }
            }
        }

        private void FillWordsTab(WordOverview wo)
        {
            ClearWordsTab();
            FontFamily fm = new FontFamily("Segoe UI");
            Font defaultFont = new Font(fm, 9, FontStyle.Regular, GraphicsUnit.Point);
            Font underlineFont = new Font(fm, 9, FontStyle.Underline, GraphicsUnit.Point);

            tabControl1.SelectedTab = tabPage1;
            int i = 0;

            if (wo._wordPossibleMeanings.Count == 1)
            {
                wo.WordPointer = 1;
            }

            foreach (Word _w in wo._wordPossibleMeanings)
            {
                classes.WordBase wb = new WordBase();
                _articleFile.article._WordBase.TryGetValue(_w.wordBasePointer, out wb);

                ListViewGroup _listViewGroup = new ListViewGroup();
                _listViewGroup.Header = wb.baseWordLu + " " + wb.wordForm.WordFormStringLu;
                _listViewGroup.Tag = i;
                _listViewGroup.Name = wb.baseWordXml;
                listView_Words.Groups.Add(_listViewGroup);

                int j = 0;

                if (wb.meanings.Count == 1 && wo._wordPossibleMeanings.Count == 1)
                {
                    _w.meaningPointer = 1;
                    ForwardStateTo(1);
                }

                foreach (classes.Meaning _m in wb.meanings)
                {
                    ListViewItem _listViewItem = new ListViewItem();
                    if (_w.meaningPointer == j+1 && _w.meaningPointer != 0)
                    {
                        _listViewItem.Checked = true;
                    }
                    _listViewItem.Tag = j;
                    _listViewItem.Name = wb.baseWordXml;
                    _listViewItem.UseItemStyleForSubItems = false;
                    _listViewItem.Group = _listViewGroup;
                    if (_m.LU == null)
                    {
                        _listViewItem.Text = wb.baseWordLu;
                        _listViewItem.Font = defaultFont;
                    }
                    else
                    {
                        _listViewItem.Text = _m.LU;
                        _listViewItem.Font = underlineFont;
                    }

                    _listViewItem.SubItems.Add(_m.DE, _listViewItem.ForeColor, _listViewItem.BackColor, defaultFont);
                    _listViewItem.SubItems.Add(_m.FR, _listViewItem.ForeColor, _listViewItem.BackColor, defaultFont);

                    listView_Words.Items.Add(_listViewItem);
                    j++;
                }

                i++;
            }
        }

        private void ClearWordsTab()
        {
            listView_Words.Items.Clear();
            listView_Words.Groups.Clear();
        }


        private void FillMeaningTab()
        {
            classes.WordOverview wo = new WordOverview();
            _articleFile.article._Words.TryGetValue(RichTextFormatter.activeWord, out wo);
            FillMeaningTab(wo);
        }

        private void FillMeaningTab(WordOverview wo)
        {

            FontFamily fm = new FontFamily("Segoe UI");
            Font defaultFont = new Font(fm, 9, FontStyle.Regular, GraphicsUnit.Point);
            Font underlineFont = new Font(fm, 9, FontStyle.Underline, GraphicsUnit.Point);
            Font italicFont = new Font(fm, 9, FontStyle.Italic, GraphicsUnit.Point);

            int wordPointer = wo.WordPointer - 1;
            int meaningPointer = wo._wordPossibleMeanings[wordPointer].meaningPointer - 1;
            string xmlSelector = wo._wordPossibleMeanings[wordPointer].wordBasePointer;

            classes.WordBase wb = new WordBase();
            _articleFile.article._WordBase.TryGetValue(xmlSelector, out wb);

            //check if has custom Translation
            if (wo._wordPossibleMeanings[wordPointer].customMeaning != null)
            {
                //TODO
            } else
            {
                if (wb.meanings[meaningPointer].LU != null)
                {
                    label_MeaningTab_Header.Text = wb.meanings[meaningPointer].LU;
                } else
                {
                    label_MeaningTab_Header.Text = wo._wordPossibleMeanings[wordPointer].occurence;
                }
                edtBasisWuert.Text = wb.baseWordLu;
                edtWordform.Text = wb.wordForm.WordFormStringLu;
                edtMp3.Text = wb.baseMp3;
                string hyperlink = "http://www.lod.lu/?" + wb.baseWordXml.Substring(0, wb.baseWordXml.Length - 4);
                linkLod.Text = "um LOD ukucken...";
                linkLod.Links.Clear();
                LinkLabel.Link link = new LinkLabel.Link();
                link.LinkData = hyperlink;
                linkLod.Links.Add(link);

                if (wb.wordForm.WordFormStringLu == "Verb" ||
                    wb.wordForm.WordFormStringLu == "Modalverb")
                {
                    panelVerb.Visible = true;
                    panelPlural.Visible = false;
                    edtHelperVerb.Text = wb.wordForm.WordFormHelperVerb;
                    edtParticipePasse.Text = wb.wordForm.pastParticiple;
                } else
                {
                    if (wb.wordForm.WordFormStringLu == "Adjektiv" ||
                        wb.wordForm.WordFormStringLu == "Adverb")
                    {
                        panelVerb.Visible = false;
                        panelPlural.Visible = false;
                    }
                    else
                    {
                        panelVerb.Visible = false;
                        panelPlural.Visible = true;
                        string plural = null;
                        if (wb.wordForm.WordFormPlurals != null)
                        foreach (string wfp in wb.wordForm.WordFormPlurals)
                        {
                            plural += wfp;
                        }
                        edtPlural.Text = plural;
                        if (plural == null)
                        {
                            edtPlural.Text = "<kee Pluriel>";
                            edtPlural.Font = italicFont;
                        }
                        else edtPlural.Font = defaultFont;
                    }
                }

                //Translation
                edtDE.Text = wb.meanings[meaningPointer].DE;
                edtFR.Text = wb.meanings[meaningPointer].FR;
                edtEN.Text = wb.meanings[meaningPointer].EN;
                edtPT.Text = wb.meanings[meaningPointer].PT;

                richExamples.Clear();

                int i = 1;
                //Examples
                if (wb.meanings[meaningPointer].examples != null)
                {
                    foreach (classes.Example _ex in wb.meanings[meaningPointer].examples)
                    {
                        richExamples.AppendText(i.ToString() + ": " +_ex.exampleText + Environment.NewLine);
                        i++;
                    }
                }

                if (wb.meanings[meaningPointer].examples_Extended != null)
                {
                    foreach (classes.Example_Extended _exe in wb.meanings[meaningPointer].examples_Extended)
                    {
                        richExamples.AppendText(i.ToString() + ": " + _exe.exampleText 
                            + " <" + _exe.enunciation.ToString() + "> "
                            + _exe.enunciationText + Environment.NewLine);
                        i++;
                    }
                }
            }

            //switch to meaningTab
            tabControl1.SelectedTab = tabPage2;
        }

        private void PrepareMeaningTab()
        {

        }

        private async Task<WordOverview> checkLodForWord(WordOverview wo, string occurence)
        {
            /*
            check LOD
            */
            Task<WordOverview> task = getLodWords(wo, occurence);
            wo = await task;
            return wo;
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
                _word.occurence = occ;
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

        private void listView_Words_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //before check Raises

            if (e.NewValue == CheckState.Checked)
            {
                try
                {                    
                    classes.WordOverview wo = new WordOverview();
                    _articleFile.article._Words.TryGetValue(RichTextFormatter.activeWord, out wo);

                    string meaningTag = (sender as ListView).Items[e.Index].Tag.ToString();
                    string wordTag = (sender as ListView).Items[e.Index].Group.Tag.ToString();

                    wo.WordPointer = Int32.Parse(wordTag) + 1;
                    wo._wordPossibleMeanings[wo.WordPointer - 1].meaningPointer = Int32.Parse(meaningTag) + 1;

                    //Unset old elements
                    int i = 0;
                    foreach (ListViewItem item in (sender as ListView).Items)
                    {
                        if (i!=e.Index)
                        {
                            item.Checked = false;
                        }
                    }

                    _articleFile.SaveToFile();

                    //ShowMeaning
                    FillMeaningTab();
                    ForwardStateTo(1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("");
        }

        private void listView_Words_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //after check Raised
            Console.WriteLine("");
        }

        private void linkLod_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;
            //if (e.Link.LinkData != null)
            url = e.Link.LinkData.ToString();

            var si = new ProcessStartInfo(url);
            Process.Start(si);
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
