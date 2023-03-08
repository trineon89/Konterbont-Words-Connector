using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using WMPLib;
using Ookii.Dialogs.WinForms;
using System.Collections.Generic;

namespace KonterbontLODConnector
{
    public partial class frmMain : Form
    {
        private TwixlAPI twixlAPI;

        public string MagazinePath = "\\\\autisme.local\\Ateliers\\InfoMedia\\Konterbont_Produktioun\\Magazines\\";
        public string ArticlePath = "\\\\autisme.local\\Ateliers\\InfoMedia\\Konterbont_Produktioun\\Artikelen\\";
        public string CustomAudioPath = "\\\\autisme.local\\Ateliers\\InfoMedia\\Konterbont_Audio\\";
        public Color OArticleColor = Color.Black;
        public Color ArticleColor = Color.Black;
        public bool TextFormClosed = true;
        WindowsMediaPlayer wplayer = null;
        public VistaFolderBrowserDialog folderBrowser;
        public VistaOpenFileDialog ArticleBrowser = new VistaOpenFileDialog
        {
            Filter = "RTF (*.rtf)|*.rtf",
            Title = "Dokument fir den Artikel auswielen"
        };

        public VistaOpenFileDialog CustomAudioBrowser = new VistaOpenFileDialog
        {
            Filter = "MP3 (*.mp3)|*.mp3",
            InitialDirectory = "\\\\autisme.local\\Ateliers\\InfoMedia\\Konterbont_Audio\\",
            //RestoreDirectory = true,
            Title = "Neien Toun fir den Popup auswielen"
        };

        public ProgressDialog progressDialog;
        private INDesignPlugin iNDesignPlugin;
        public DataHandler globaldt = null;

        public LogClass Log = new LogClass();

        private Form TextForm;
        private RichTextBox rtb;

        private FormStartPosition textFormPosition = FormStartPosition.WindowsDefaultLocation;
        private Point textFormLocation = new Point();

        void generateTextForm()
        {
            TextForm = new Form()
            {
                Name = "frmTextForm",
                Text = "Dokument",
                Width = 500,
                Height = 500,
                Top = 0,
                Left = 0,
                MaximizeBox = false,
                MinimizeBox = false,
                FormBorderStyle = FormBorderStyle.SizableToolWindow
        };
            if (!textFormLocation.IsEmpty)
            {
                TextForm.StartPosition = FormStartPosition.Manual;
                TextForm.Location = textFormLocation;
            }
            TextForm.FormClosing += new FormClosingEventHandler(textFormClosing);
            generateRichTextBox();
        }

        private void textFormClosing(object sender, FormClosingEventArgs e)
        {
            textFormLocation = TextForm.Location;
        }

        void generateRichTextBox()
        {
            rtb = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                Name = "rtbDocument",
                ReadOnly = true,
                BackColor = SystemColors.Window,
                Font = new Font("Arial", 12)

            };
        }

        /// <summary>
        /// Init frmMain (Main Form)
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            folderBrowser = new VistaFolderBrowserDialog
            {
                SelectedPath = ArticlePath
            };
            iNDesignPlugin = new INDesignPlugin();
            //
            ResetInstance();
        }

        // https://stackoverflow.com/questions/20433937/casting-a-rgb-string-to-a-color-type
        public Color convertToColorArray(string getcolor)
        {
            //This gives us an array of 3 strings each representing a number in text form.
            var splitString = getcolor.Split(',');
            List<string> cleansplitstring = new List<string>();
            foreach (var color in splitString)
            {
                string c = color;
                if (color.IndexOf('.') >= 0) c = color.Split('.')[0];
                cleansplitstring.Add(c);
            }
            //converts the array of 3 strings in to an array of 3 ints.
            var splitInts = cleansplitstring.ToArray().Select(item => int.Parse(item)).ToArray();

            //takes each element of the array of 3 and passes it in to the correct slot
            Color setcolor = Color.FromArgb(splitInts[0], splitInts[1], splitInts[2]);
            return setcolor;
        }

        /*
        private async Task<AutoComplete> GetFullTranslationsAsync(string searchstring, bool compare = true)
        {
            return await Task.Run(() => GetFullTranslations(searchstring, compare));
        }
        */
        private async Task<AutoComplete> ReturnFullAutoComplete(string searchstring)
        {
            AutoComplete ac = new AutoComplete();
            //Yolo & Lolo
            Task<LodSearch> task = Task.Run(async () => await FetchJSONSearchAsync(searchstring));
            task.Wait();
            LodSearch fetchedSearch = task.Result;
            Log.WriteToLog("fetched");

            Task<AutoComplete> act = Task.Run(async () => await ac.GetAutoComplete2022(searchstring, fetchedSearch, Log));
            act.Wait();
            AutoComplete actemp = new AutoComplete();
            actemp = act.Result;
            return actemp;
        }

        private async Task<LodSearch> FetchJSONSearchAsync(string Word)
        {
            return await Task.Run(() => FetchJSONSearch(Word));
        }

        private async Task<LodSearch> FetchJSONSearch(string Word)
        {
            bool blocked = true;
            string responseBody = null;
            int i = 0;
            LodSearch LodSearch = new LodSearch();
            while (blocked)
            {
                HttpClient httpClient;
                //if more than 30 connections => Exception
                if (i >= 2) { throw new Exception("Probably blocked?"); }
                if (_Globals.useProxy && i > 0)
                {
                    //get random Proxy & setup Proxy
                    AutoComplete _ac = new AutoComplete();
                    string _proxy = _ac.GetProxie();
                    string proxyHost = _proxy.Substring(0, _proxy.IndexOf(":"));
                    string proxyPort = _proxy.Substring(_proxy.IndexOf(":") + 1);
                    var proxy = new WebProxy
                    {
                        Address = new Uri($"http://{proxyHost}:{proxyPort}"),
                        UseDefaultCredentials = true
                    };

                    HttpClientHandler handler = new HttpClientHandler() { Proxy = proxy };
                    handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    httpClient = new HttpClient(handler);
                }
                else
                {
                    httpClient = new HttpClient();
                }
            
                var httpContent = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://lod.lu/api/lb/search?lang=lb&query=" + Word), //Word has to be Lowercase
                    Method = HttpMethod.Get,
                    /*Headers =
                               {
                                  { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                                  { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                               }*/
                };
                try { 
                    var _response = await httpClient.SendAsync(httpContent);
                    blocked = !_response.IsSuccessStatusCode;
                    responseBody = await _response.Content.ReadAsStringAsync();
                    LodSearch = LodSearch.FromJson(responseBody);
                } catch (Exception e)
                {
                    Log.WriteToLog(e.ToString());
                }
                httpClient.Dispose();
                i++;
            }
            return LodSearch;
        }

        private LodSearch cleanupSearch(LodSearch lodSearch)
        {
            AutoComplete ac = new AutoComplete();

            foreach (Result rs in lodSearch.Results)
            {
                rs.Pos = ac.GetWuertForm(rs.Pos);
            }
            return lodSearch;
        }

        private void rbClicked(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            frmSelectMeaning frmSelectMeaning = (frmSelectMeaning)rb.TopLevelControl;
            frmSelectMeaning.gbMeanings_Click(sender, e);
        }

        private async Task<string> FetchWordsTT(string XML, string Lang)
        {
            var httpClient = new HttpClient();
            string LangURL;

            if (Lang == "LU")
            {
                LangURL = "";
            }
            else
            {
                LangURL = Lang.ToLower();
            }
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/getart" + LangURL + ".php?artid=" + XML),
                Method = HttpMethod.Get,
                Headers =
                           {
                              { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                              { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                           }
            };

            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            // Meanings START
            if (Lang == "DE")
            {
                int _i = 1;
                string Selection = null;
                string MeaningsTT = null;
                HtmlNodeCollection MeaningsDE = htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']");

                if (MeaningsDE == null) // keen normale Fall, Variant
                {
                    HtmlNode Node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='artikel']");
                    HtmlNode RemoveNode = null;
                    try
                    {
                        if (Node.SelectSingleNode("//span[@class='mentioun_adress']") != null)
                        {
                            RemoveNode = Node.SelectSingleNode("//span[@class='mentioun_adress']");
                            if (RemoveNode != null)
                                Node.RemoveChild(RemoveNode);
                            RemoveNode = Node.SelectSingleNode("//span[@class='klass']");
                            if (RemoveNode != null)
                                Node.RemoveChild(RemoveNode);
                            RemoveNode = Node.SelectSingleNode("//div[@class='lux.map']");
                            if (RemoveNode != null)
                                Node.RemoveChild(RemoveNode);
                        }
                    }
                    catch
                    {
                        MeaningsTT = Node.InnerText;
                        return MeaningsTT;
                    }
                    //Meaning.RemoveChild(RemoveNode);
                    MeaningsTT = Node.InnerText;
                    return MeaningsTT;
                }

                foreach (HtmlNode htmlNode in MeaningsDE)
                {
                    string MeaningNr = null;
                    string MeaningText = null;
                    string MeaningTextAdd = null;
                    HtmlNode[] MeaningArray;

                    if (htmlNode.SelectSingleNode("span[@class='text_gen']") != null)
                    {
                        if (htmlNode.SelectSingleNode("span[@class='text_gen']").InnerText.Contains("["))
                        {
                            if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null)
                            { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                            MeaningText = htmlNode.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = htmlNode.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null)
                            { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                            MeaningArray = htmlNode.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1)
                                { MeaningText = MeaningText + ", "; }
                            }
                        }
                    }
                    else
                    {
                        if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null)
                        { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                        MeaningText = htmlNode.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();

                    MeaningsTT = MeaningsTT + " " + MeaningNr + " " + MeaningText + "" + MeaningTextAdd + Environment.NewLine;
                }
                return MeaningsTT;
                ;
            }
            return null;
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired)
                c.Invoke(new MethodInvoker(delegate
                { a(); }));
            else
                return false;

            return true;
        }

        /// <summary>
        /// Lies den Tooltip aus dem Wuert
        /// </summary>
        /// <param name="wuert"></param>
        /// <returns></returns>
        private string GetSelectionTooltip(Wuert wuert)
        {
            string tempstring = "";
            try
            {
                tempstring += "DE: " + wuert.Meanings[0].DE + " | FR: " + wuert.Meanings[0].FR;
            }
            catch
            {
                tempstring = "";
            }

            return tempstring;
        }

        private DataHandler OpenDocument(DataHandler dt)
        {
            rtb.LoadFile(dt.DocPath);

            TextForm.Controls.Add(rtb);
            return dt;
        }

        /// <summary>
        /// Weist all Bedeitungen zu engem Wuert un (Gëtt als Parameter uginn)
        /// </summary>
        /// <param name="wuert"></param>
        /// <param name="occurence"></param>
        /// <returns></returns>
        private async Task<Wuert> SelectMeaning(Wuert wuert, string _occurence)
        {
            frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();
            int _m = 1;
            int _Total = 0;
            foreach (Meaning meaning in wuert.Meanings)
            {
                string thename = _m.ToString();
                RadioButton rb = new RadioButton
                {
                    Name = thename,
                    Text = thename,

                    Location = new Point(10, _m * 30),
                    Width = 100
                };
                if (_m == 1)
                {
                    rb.Checked = true;
                }
                rb.CheckedChanged += new EventHandler(rbClicked);

                frmSelectMeaning.gbMeanings.Text = "Fir '" + wuert.WuertLu + "':";
                frmSelectMeaning.gbMeanings.Controls.Add(rb);
                Font currentFont = frmSelectMeaning.rtbDE.SelectionFont;
                Font Normal = new Font(currentFont, FontStyle.Regular);
                Font Italic = new Font(currentFont, FontStyle.Italic);
                Font Bold = new Font(currentFont, FontStyle.Bold);
                Font BoldItalic = new Font(currentFont, FontStyle.Bold | FontStyle.Italic);
                Font BigBold = new Font(currentFont.FontFamily, 10, FontStyle.Bold);
                string examples = "";
                string egs = "";

                foreach (Example _ex in wuert.Meanings[_m - 1].Examples)
                {
                    examples += _ex.ExampleText + Environment.NewLine;
                    if (_ex.EGS != "")
                    {
                        egs = _ex.EGS + Environment.NewLine;
                    }

                }

                if (Int32.Parse(thename) < 10)
                {
                    thename = "0" + thename;
                }


                frmSelectMeaning.rtbDE.SelectionFont = BigBold;
                frmSelectMeaning.rtbDE.AppendText(thename + ". " + wuert.Meanings[_m - 1].DE + Environment.NewLine);
                frmSelectMeaning.rtbDE.SelectionFont = Bold;
                frmSelectMeaning.rtbDE.AppendText("Beispiele:" + Environment.NewLine);
                frmSelectMeaning.rtbDE.SelectionFont = Normal;
                frmSelectMeaning.rtbDE.AppendText(examples);
                frmSelectMeaning.rtbDE.SelectionFont = Italic;
                if (egs != "")
                {
                    frmSelectMeaning.rtbDE.SelectionFont = BoldItalic;
                    frmSelectMeaning.rtbDE.AppendText("umgangssprachlich: ");
                    frmSelectMeaning.rtbDE.SelectionFont = Italic;
                    frmSelectMeaning.rtbDE.AppendText(egs);
                }
                frmSelectMeaning.rtbDE.AppendText(Environment.NewLine);

                frmSelectMeaning.rtbFR.SelectionFont = BigBold;
                frmSelectMeaning.rtbFR.AppendText(thename + ". " + wuert.Meanings[_m - 1].FR + Environment.NewLine);
                frmSelectMeaning.rtbFR.SelectionFont = Bold;
                frmSelectMeaning.rtbFR.AppendText("exemples:" + Environment.NewLine);
                frmSelectMeaning.rtbFR.SelectionFont = Normal;
                frmSelectMeaning.rtbFR.AppendText(examples);
                frmSelectMeaning.rtbFR.SelectionFont = Italic;
                if (egs != "")
                {
                    frmSelectMeaning.rtbFR.SelectionFont = BoldItalic;
                    frmSelectMeaning.rtbFR.AppendText("familier: ");
                    frmSelectMeaning.rtbFR.SelectionFont = Italic;
                    frmSelectMeaning.rtbFR.AppendText(egs);
                }
                frmSelectMeaning.rtbFR.AppendText(Environment.NewLine);

                frmSelectMeaning.rtbEN.SelectionFont = BigBold;
                frmSelectMeaning.rtbEN.AppendText(thename + ". " + wuert.Meanings[_m - 1].EN + Environment.NewLine);
                frmSelectMeaning.rtbEN.SelectionFont = Bold;
                frmSelectMeaning.rtbEN.AppendText("examples:" + Environment.NewLine);
                frmSelectMeaning.rtbEN.SelectionFont = Normal;
                frmSelectMeaning.rtbEN.AppendText(examples);
                frmSelectMeaning.rtbEN.SelectionFont = Italic;
                if (egs != "")
                {
                    frmSelectMeaning.rtbEN.SelectionFont = BoldItalic;
                    frmSelectMeaning.rtbEN.AppendText("colloquial: ");
                    frmSelectMeaning.rtbEN.SelectionFont = Italic;
                    frmSelectMeaning.rtbEN.AppendText(egs);
                }
                frmSelectMeaning.rtbEN.AppendText(Environment.NewLine);

                frmSelectMeaning.rtbPT.SelectionFont = BigBold;
                frmSelectMeaning.rtbPT.AppendText(thename + ". " + wuert.Meanings[_m - 1].PT + Environment.NewLine);
                frmSelectMeaning.rtbPT.SelectionFont = Bold;
                frmSelectMeaning.rtbPT.AppendText("exemplos:" + Environment.NewLine);
                frmSelectMeaning.rtbPT.SelectionFont = Normal;
                frmSelectMeaning.rtbPT.AppendText(examples);
                frmSelectMeaning.rtbPT.SelectionFont = Italic;
                if (egs != "")
                {
                    frmSelectMeaning.rtbPT.SelectionFont = BoldItalic;
                    frmSelectMeaning.rtbPT.AppendText("coloquial: ");
                    frmSelectMeaning.rtbPT.SelectionFont = Italic;
                    frmSelectMeaning.rtbPT.AppendText(egs);
                }
                frmSelectMeaning.rtbPT.AppendText(Environment.NewLine);

                int _MeaningsCount = wuert.Meanings.Count();

                _m++;
                _Total = _m;          
            }

            RadioButton rbtn = new RadioButton
            {
                Name = "0",
                Text = "Passt nët",
                Location = new Point(10, _Total * 30),
                Width = 100
            };
            frmSelectMeaning.gbMeanings.Controls.Add(rbtn);

            frmSelectMeaning.gbMeanings_Click(this, null);
            ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.HighlightSelText(TextForm.Controls.OfType<RichTextBox>().First(), _occurence));
            ControlInvokeRequired(TextForm, () => TextForm.Activate());

            if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
            {
                RadioButton selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.UnSelText(TextForm.Controls.OfType<RichTextBox>().First()));
                string Selection = selectedMeaning.Name;
                wuert.Selection = Int32.Parse(Selection);
                Log.WriteToLog("Meaning \"" + wuert.Meanings[wuert.Selection-1].DE + "\"(" + Selection + ") for \"" + wuert.WuertLu + "\" selected");
            }
            else
            {
                return null;
            }
            wuert = MissingENPT(wuert);

            return wuert;
        }

        /// <summary>
        /// Weist en dialog in wann di englesch bzw. portugiesech Iwwersetzung eiden ass (Meaning[x].EN = null / Meaning[x].PT = null)
        /// </summary>
        /// <param name="wuert"></param>
        /// <returns></returns>
        private Wuert MissingENPT (Wuert wuert)
        {
            if (wuert.Meanings[wuert.Selection - 1].EN == "")
            {
                InputDialog ENid = new InputDialog();
                ENid.MainInstruction = "Eng Bedeitung antippen:";
                ENid.Content = "DE: " + wuert.Meanings[wuert.Selection - 1].DE + "; FR: " + wuert.Meanings[wuert.Selection - 1].FR;
                ENid.WindowTitle = "Englesch Iwwersetzung";
                if (ENid.ShowDialog() == DialogResult.OK)
                {
                    wuert.Meanings[wuert.Selection - 1].EN = ENid.Input;
                    wuert.Meanings[wuert.Selection - 1].hasCustomEN = true;
                    Log.WriteToLog("EN  " + ENid.Input + " for " + wuert.Meanings[wuert.Selection - 1].LU + ") entered");
                }
                ENid.Dispose();
            }

            if (wuert.Meanings[wuert.Selection - 1].PT == "")
            {
                InputDialog PTid = new InputDialog();
                PTid.MainInstruction = "Eng Bedeitung antippen:";
                PTid.Content = "DE: " + wuert.Meanings[wuert.Selection - 1].DE + "; FR: " + wuert.Meanings[wuert.Selection - 1].FR;
                PTid.WindowTitle = "Portugisesch Iwwersetzung";
                if (PTid.ShowDialog() == DialogResult.OK)
                {
                    wuert.Meanings[wuert.Selection - 1].PT = PTid.Input;
                    wuert.Meanings[wuert.Selection - 1].hasCustomPT = true;
                    Log.WriteToLog("PT  " + PTid.Input + " for " + wuert.Meanings[wuert.Selection - 1].LU + ") entered");
                }
                PTid.Dispose();
            }
            return wuert;
        }


        private async Task<AutoComplete> ShowSelections(AutoComplete ac)
        {
            Wuert tmpwuert = null;
            if (ac.Wierder.Count() > 1)
            {
                frmSelectMeaning frm = new frmSelectMeaning();
                int _i = 1;

                foreach (Wuert wuert in ac.Wierder)
                {
                    tmpwuert = wuert;
                    RadioButton rb = new RadioButton
                    {
                        Name = _i.ToString(),
                        Text = tmpwuert.WuertLu + " (" + tmpwuert.WuertForm.WuertForm + ")",
                        Location = new Point(10, _i * 30),
                        Width = 500
                    };

                    if (_i == 1)
                    {
                        rb.Checked = true;
                    }

                    /*
                     * Beise Yannick, mir hunn eis Informatiounen dach schonn alleguerte virleien?
                    Task<string> task = Task.Run(async () => await GetSelectionTooltipAsync(tmpwuert.XMLFile));
                    task.Wait();
                    string tooltip = task.Result;
                    */

                    string tooltip = GetSelectionTooltip(tmpwuert);

                    if (tooltip.Contains("Variant") || tooltip == "")
                    {
                        rb.Enabled = false;
                        var result = tooltip.Substring(tooltip.LastIndexOf(' ') + 1);
                        //rb.Text = rb.Text + " (Variant vun " + result + ")";
                        rb.Text = rb.Text + " (Variant)";
                        tmpwuert.IsVariant = true;
                    }

                    //ac.Wierder.Add(wuert);
                    frm.gbMeanings.Dock = DockStyle.Fill;
                    frm.tcLang.Visible = false;
                    frm.gbMeanings.Controls.Add(rb);
                    frm.gbMeanings.Text = "Wuert auswielen:";
                    frm.Text = "Wuert auswielen";
                    frm.tpInfo.SetToolTip(rb, tooltip);
                    _i++;
                }

                ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.HighlightSelText(TextForm.Controls.OfType<RichTextBox>().First(), ac.Occurence));

                DialogResult dlg = await Task.Run(() => frm.ShowDialog());
                if (dlg == DialogResult.OK)
                {
                    ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.UnSelText(TextForm.Controls.OfType<RichTextBox>().First()));
                    RadioButton radioButton = frm.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    ac.Selection = Int32.Parse(radioButton.Name);
                    Wuert SelWuert = ac.Wierder[ac.Selection - 1];
                    if (SelWuert.Meanings.Count() > 1)
                    {
                        //SelWuert = SelectMeaning(SelWuert);

                        SelWuert= await Task.Run(() => SelectMeaning(SelWuert, ac.Occurence));
                    }
                    else
                    {
                        SelWuert = MissingENPT(SelWuert);
                    }
                    ac.Wierder[ac.Selection - 1] = SelWuert;
                    Log.WriteToLog("Word \"" + SelWuert.WuertLu + "\"(" + ac.Selection +") selected");
                }
                else
                {
                    return null;
                }


            }
            else
            {
                ac.Selection = 1;
                // meanings
                Wuert wuert = ac.Wierder[0];
                wuert.Selection = 1;
                if (wuert.Meanings.Count() > 1)
                {
                    //wuert = SelectMeaning(wuert);
                    wuert = await Task.Run(() => SelectMeaning(wuert, ac.Occurence));
                }
                else
                {
                    wuert = MissingENPT(wuert);
                }
                ac.Wierder[0] = wuert;
            }
            return ac;
        }


        /// <summary>
        /// Menu -> Artikel Opmaachen click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArtikelOpmaachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (TextForm.Visible) { TextForm.Hide(); }
            ResetInstance();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                ArticleWorker();
            }
        }

        /// <summary>
        /// Loads the .words File and fills (if .wordslists exists) into the software
        /// </summary>
        /// <param name="DirectLoadPath"></param>
        private async void ArticleWorker(String DirectLoadPath = null)
        {
            string lastFolderName = Path.GetFileName(Path.GetDirectoryName(folderBrowser.SelectedPath));
            Log.CreateLog(folderBrowser.SelectedPath, lastFolderName);
            if (DirectLoadPath != null)
            { folderBrowser.SelectedPath = DirectLoadPath; }

            string[] files = Directory.GetFiles(folderBrowser.SelectedPath, "*.words");
            if (files.Length == 0)
            {
                MessageBox.Show(this, "Keen Words Fichier an dësem Dossier!", "Upsi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int filec = 0;
            foreach (var file in files)
            {
                string tmpfilename = Path.GetFileNameWithoutExtension(file) + ".wordslist";

                DataHandler dt = new DataHandler();

                if (File.Exists(folderBrowser.SelectedPath + "\\" + tmpfilename)) { dt = dt.LoadFromFile(folderBrowser.SelectedPath, tmpfilename); }
                else { dt = new DataHandler(tmpfilename, folderBrowser.SelectedPath + "\\"); }

                // add Function
                if (dt.DocPath == null)
                {
                    if (ArticleBrowser.ShowDialog() == DialogResult.OK)
                    {
                        dt.DocPath = ArticleBrowser.FileName;
                        try
                        {
                            TextForm.Show();
                            dt = OpenDocument(dt);

                            tsmiText.Checked = true;
                        }
                        catch (Exception ee) { MessageBox.Show(ee.ToString()); }
                    }
                    else { return; }
                }
                else
                {
                    try
                    {
                        TextForm.Show();
                        dt = OpenDocument(dt);

                        tsmiText.Checked = true;
                    }
                    catch (Exception ee) { MessageBox.Show(ee.ToString()); }
                }

                if (filec == 0) { dt.PrepareOutputTmpFolder(); }
                filec++;

                string tfile = new StreamReader(file).ReadToEnd();
                string[] lines = tfile.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int countlines = lines.GetLength(0);
                lines = lines.Take(lines.Count() - 1).ToArray();
                int c = 0;

                DataHandler dtt = new DataHandler
                {
                    CustomColor = dt.CustomColor,
                    Filename = dt.Filename,
                    Filepath = dt.Filepath,
                    Globrgb = dt.Globrgb,
                    DocPath = dt.DocPath,
                    targetMag = dt.targetMag
                };

                Pietschsoft.NativeProgressDialog progressDialog = new Pietschsoft.NativeProgressDialog(this.Handle)
                {
                    Title = "Wierder sichen",
                    CancelMessage = "Eieiei... Da wart elo...",
                    Maximum = 100,
                    Value = 0,
                    Line3 = "Calculating Time Remaining..."
                };
                
                progressDialog.ShowDialog(Pietschsoft.NativeProgressDialog.PROGDLG.Modal, Pietschsoft.NativeProgressDialog.PROGDLG.AutoTime, Pietschsoft.NativeProgressDialog.PROGDLG.NoMinimize);

                foreach (string line in lines)
                {
                    if (progressDialog.HasUserCancelled)
                    {
                        progressDialog.CloseDialog();
                    }
                    progressDialog.Line1 = "Wuert: " + line;
                    if (c == 0)
                    {
                        dtt.SetRGB(line);
                        ArticleColor = convertToColorArray(dtt.Globrgb);
                        OArticleColor = convertToColorArray(line);
                        tssCustomColor.BackColor = ArticleColor;

                    }
                    else if (line != "")
                    {
                        //Find Occurence line in dt.Wordlist
                        var tmp = dt.WordList.FirstOrDefault(occ => occ.Occurence == line);
                        if (tmp != null)
                        {
                            dtt.WordList.Add(tmp);
                        }
                        else
                        {
                            // New Word here
                            Console.WriteLine("New Word: " + line);
                            AutoComplete acword = await Task.Run(async () => await ReturnFullAutoComplete(line));
                            if (acword != null)
                            {
                                //
                                try
                                {
                                    acword = await ShowSelections(acword);
                                    dtt.AddWordToList(acword);
                                    acword.internalId = c;
                                    dtt.SaveToFile(dtt);
                                }
                                catch
                                {
                                    MessageBox.Show("\"" + acword.Occurence + "\" ass eng Variant oder eng Diminutivform. W.e.g. erausläschen","Variant oder Diminutivform",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                    
                    double dbl = 100d / countlines * c;
                    uint _currprog = Convert.ToUInt32(Math.Round(dbl));
                    progressDialog.Line2 = "Oofgeschloss zu " + _currprog.ToString() + "%";
                    progressDialog.Value = _currprog;
                    c++;
                    //Break out of the loop if User Has Cancelled
                    if (progressDialog.HasUserCancelled)
                    {
                        progressDialog.CloseDialog();
                        break;
                    }
                }

                progressDialog.CloseDialog();

                lbWords.Items.Clear();
                foreach (AutoComplete ac in dtt.WordList) // Adds Words to lbWords on Main Form
                {
                    lbWords.Items.Add(ac.Occurence);
                }

                globaldt = dtt;

                if (dtt.WordList.Count() > 0)
                { lbWords.SelectedIndex = 0; }
                tsmiSave.Enabled = true;
                btnCreatePopups.Enabled = true;
                tsbCustomAudio.Enabled = true;
                tsbPlayAudio.Enabled = true;
                tsmiText.Enabled = true;
                tsmiColor.Enabled = true;
            }
            globaldt = globaldt.AddM4A(globaldt);

            globaldt.SaveToFile(globaldt);
            SetIsSaved(true);

            
            Log.WriteToLog("Article " + lastFolderName + " opened");
            tssArticle.Text = lastFolderName;
            tssMagazine.Text = globaldt.targetMag;
        }

        private void LbWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ✓ 
            if (lbWords.SelectedIndex == -1)
                return;
            var _i = 0;
            Utility.UnSelText(TextForm.Controls.OfType<RichTextBox>().First());
            lbSelectWord.Items.Clear();
            foreach (Wuert SelWuert in globaldt.WordList[lbWords.SelectedIndex].Wierder)
            {
                if (globaldt.WordList[lbWords.SelectedIndex].Selection - 1 == _i)
                {
                    if (SelWuert.Meanings.Count() > 0)
                    {
                        lbSelectWord.Items.Add(SelWuert.WuertLu + " ✓");
                        lbSelectWord.SelectedIndex = _i;
                    }
                    else
                    {
                        lbSelectWord.Items.Add(SelWuert.WuertLu + " (Variant) ✓");
                        lbSelectWord.SelectedIndex = _i;
                    }

                }
                else
                {
                    if (SelWuert.Meanings.Count() > 0) { lbSelectWord.Items.Add(SelWuert.WuertLu); }
                    else { lbSelectWord.Items.Add(SelWuert.WuertLu + " (Variant)"); }
                }
                _i++;
            }
            Utility.HighlightSelText(TextForm.Controls.OfType<RichTextBox>().First(), globaldt.WordList[lbWords.SelectedIndex].Occurence);
        }

        private void LbSelectWord_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ✓ 
            if (lbSelectWord.SelectedIndex == -1)
                return;
            if (lbWords.SelectedIndex == -1)
                return;
            var _i = 0;
            lbSelectMeaning.Items.Clear();

            foreach (Meaning SelMeaning in globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings)
            {
                if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1 == _i)
                {
                    lbSelectMeaning.Items.Add(SelMeaning.DE.Trim() + " ✓");
                    lbSelectMeaning.SelectedIndex = _i;
                }
                else
                {
                    lbSelectMeaning.Items.Add(SelMeaning.DE.Trim());
                }
                _i++;
            }
            // disables audio buttons on variant
            if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].IsVariant)
            {
                tsbCustomAudio.Enabled = false;
                tsbPlayAudio.Enabled = false;
            }
            else
            {
                tsbCustomAudio.Enabled = true;
                tsbPlayAudio.Enabled = true;
            }

        }

        private void LbSelectMeaning_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSelectWord.SelectedIndex == -1)
                return;
            if (lbWords.SelectedIndex == -1)
                return;
            if (lbSelectMeaning.SelectedIndex == -1)
                return;
            rtbDetails.Clear();
            Wuert SelWord = globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex];

            Meaning SelMeaning = globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex];

            Font Bold = new Font(rtbDetails.SelectionFont, FontStyle.Bold);
            Font Normal = new Font(rtbDetails.SelectionFont, FontStyle.Regular);
            Font Italic = new Font(rtbDetails.SelectionFont, FontStyle.Italic);
            Font BoldItalic = new Font(rtbDetails.SelectionFont, FontStyle.Bold | FontStyle.Italic);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText("LU: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.LU);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "Wuertform: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelWord.WuertForm.WuertForm);

            rtbDetails.SelectionFont = Bold;
            if (SelWord.WuertForm.WuertForm == "Verb" || SelWord.WuertForm.WuertForm == "Modalverb")
            {
                rtbDetails.AppendText(Environment.NewLine + "Participe Passé: ");

                if (SelMeaning.LUs != null)
                {
                    rtbDetails.SelectionFont = Normal;
                    string tmpPluriel = SelMeaning.LUs;
                    tmpPluriel = tmpPluriel.Replace("&lt;", "<");
                    tmpPluriel = tmpPluriel.Replace("&gt;", ">");
                    rtbDetails.AppendText(tmpPluriel);
                }
                else
                {
                    rtbDetails.SelectionFont = Italic;
                    rtbDetails.AppendText("(Kee Pluriel)");
                }
                rtbDetails.SelectionFont = Bold;
                rtbDetails.AppendText(Environment.NewLine + "Hëllefsverb: ");
                rtbDetails.SelectionFont = Normal;
                rtbDetails.AppendText(SelMeaning.HV);

            }
            else
            {
                if (SelWord.WuertForm.WuertForm.Contains("Substantiv"))
                {
                    rtbDetails.AppendText(Environment.NewLine + "Pluriel: ");

                    if (SelMeaning.LUs != null)
                    {
                        rtbDetails.SelectionFont = Normal;
                        string tmpPluriel = SelMeaning.LUs;
                        tmpPluriel = tmpPluriel.Replace("&lt;", "<");
                        tmpPluriel = tmpPluriel.Replace("&gt;", ">");
                        rtbDetails.AppendText(tmpPluriel);
                    }
                    else
                    {
                        rtbDetails.SelectionFont = Italic;
                        rtbDetails.AppendText("(Kee Pluriel)");
                    }
                    rtbDetails.AppendText(Environment.NewLine);
                }
                else
                {
                    rtbDetails.AppendText(Environment.NewLine);
                    rtbDetails.AppendText(Environment.NewLine);
                }
            }



            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "DE: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.DE);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "FR: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.FR);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "EN: ");
            if (SelMeaning.hasCustomEN)
            {
                rtbDetails.SelectionFont = BoldItalic;
                tsbCustomEN.Enabled = true;
            }
            else
            {
                rtbDetails.SelectionFont = Normal;
                tsbCustomEN.Enabled = false;
            }
            rtbDetails.AppendText(SelMeaning.EN);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "PT: ");
            if (SelMeaning.hasCustomPT)
            {
                rtbDetails.SelectionFont = BoldItalic;
                tsbCustomPT.Enabled = true;
            }
            else
            {
                rtbDetails.SelectionFont = Normal;
                tsbCustomPT.Enabled = false;
            }
            rtbDetails.AppendText(SelMeaning.PT);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "MP3: ");
            if (SelMeaning.hasCustomAudio)
            {
                rtbDetails.SelectionFont = BoldItalic;
            }
            else
            {
                rtbDetails.SelectionFont = Normal;
            }
            rtbDetails.AppendText(SelMeaning.MP3);


            var Ex = 1;
            foreach (Example SelExample in SelMeaning.Examples)
            {
                rtbDetails.SelectionFont = Bold;
                rtbDetails.AppendText(Environment.NewLine + "Beispill " + Ex.ToString() + ": ");
                rtbDetails.SelectionFont = Normal;
                rtbDetails.AppendText(SelExample.ExampleText);
                // ëmgangssproochlech
                if (SelExample.EGS != "")
                {
                    rtbDetails.SelectionFont = Bold;
                    rtbDetails.AppendText(Environment.NewLine + "Beispill " + Ex.ToString() + "(EGS): ");
                    rtbDetails.SelectionFont = Normal;
                    rtbDetails.AppendText(SelExample.EGS);
                }
                Ex++;
            }
            // delete all mp3 of word meanings
            /*
            foreach (Meaning Meaning in globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings)
            {
                if (File.Exists(globaldt.Filepath + "WebResources\\popupbase-web-resources\\audio\\" + Meaning.MP3))
                {
                    try
                    {
                        File.Delete(globaldt.Filepath + "WebResources\\popupbase-web-resources\\audio\\" + Meaning.MP3);
                    }
                    catch
                    {
                    }
                }
            }*/
            // gets mp3 for new selected meaning
            globaldt.GetMp3(globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3,
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].hasCustomAudio,
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].M4A);
        }

        private void MagazineSelectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show MagazineSelector Form
            if (globaldt == null)
            {
                globaldt = new DataHandler();
            }
            globaldt.ShowMagazineSelector();
            tssMagazine.Text = globaldt.targetMag;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //Save
            globaldt.SaveToFile(globaldt);
            SetIsSaved(true);
            Log.WriteToLog("Saved");
            //tsmiSave.Enabled = false;
        }

        /// <summary>
        /// Button <Create Popups> Action.
        /// Generates Popups and copy to OutputFolder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreatePopups_Click(object sender, EventArgs e)
        {
            globaldt.SaveToFile(globaldt);
            //CreatePopups
            if (globaldt.OutputPopups())
            {
                globaldt.CopyTmpToArt();
                MessageBox.Show(this, "Popups sinn erstallt!", "Okay", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            globaldt.HasPopups(true);
            btnCopyToMag.Enabled = true;
            Log.WriteToLog("Popups created");
        }

        private void lbSelectWord_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int Item = 0; Item < lbSelectWord.Items.Count; Item++)
            {
                lbSelectWord.Items[Item] = Regex.Replace(lbSelectWord.Items[Item].ToString(), @"( ✓)", "");
            }


            if (!lbSelectWord.Items[lbSelectWord.SelectedIndex].ToString().Contains("✓"))
            {
                lbSelectWord.Items[lbSelectWord.SelectedIndex] = lbSelectWord.Items[lbSelectWord.SelectedIndex] + " ✓";
                for (int Item = 0; Item < lbSelectMeaning.Items.Count; Item++)
                {
                    lbSelectMeaning.Items[Item] = Regex.Replace(lbSelectMeaning.Items[Item].ToString(), @"( ✓)", "");
                }
                lbSelectMeaning.Items[0] = lbSelectMeaning.Items[0] + " ✓";

                Log.WriteToLog("Changed Word \"" + globaldt.WordList[lbWords.SelectedIndex].Occurence + "\" (" + globaldt.WordList[lbWords.SelectedIndex].Selection + ") to ", false);
                
                globaldt.WordList[lbWords.SelectedIndex].Selection = lbSelectWord.SelectedIndex + 1;
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection = 1;
                lbSelectMeaning.SelectedIndex = 0;
                // save
                globaldt.SaveToFile(globaldt);
                Log.WriteToLog(" \"" + lbSelectWord.Items[lbSelectWord.SelectedIndex] + "\" for \"" + lbWords.Items[lbWords.SelectedIndex]+ "\"",true,true);
            }
        }

        private void lbSelectMeaning_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            for (int Item = 0; Item < lbSelectMeaning.Items.Count; Item++)
            {
                lbSelectMeaning.Items[Item] = Regex.Replace(lbSelectMeaning.Items[Item].ToString(), @"( ✓)", "");
            }

            if (!lbSelectMeaning.Items[lbSelectMeaning.SelectedIndex].ToString().Contains("✓"))
            {
                lbSelectMeaning.Items[lbSelectMeaning.SelectedIndex] = lbSelectMeaning.Items[lbSelectMeaning.SelectedIndex] + " ✓";

                // globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection
                Log.WriteToLog("Changed Meaning \"" + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].WuertLu + "\" (" + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection + ") to ",false);
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection = lbSelectMeaning.SelectedIndex + 1;
                // if EN & PT are empty (no meaning on LOD (coming soon))
                if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].EN == "" || globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].hasCustomEN)
                {
                    InputDialog ENid = new InputDialog()
                    {
                        MainInstruction = "Eng Bedeitung antippen:",
                        Content = "DE: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].DE + "; FR: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].FR,
                        WindowTitle = "Englesch Iwwersetzung"
                    };
                    if (ENid.ShowDialog(this) == DialogResult.OK)
                    {
                        globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].EN = ENid.Input;
                        globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].hasCustomEN = true;
                    }
                    ENid.Dispose();


                    LbSelectMeaning_SelectedIndexChanged(sender, e);
                }

                if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex]
                    .Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex]
                    .Selection - 1].PT == "" || globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex]
                    .Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].hasCustomPT)
                {
                    InputDialog PTid = new InputDialog()
                    {
                        MainInstruction = "Eng Bedeitung antippen:",
                        Content = "DE: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].DE + "; FR: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].FR,
                        WindowTitle = "Portugisesch Iwwersetzung"
                    };
                    if (PTid.ShowDialog(this) == DialogResult.OK)
                    {
                        globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].PT = PTid.Input;
                        globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].hasCustomPT = true;
                    }
                    PTid.Dispose();

                    LbSelectMeaning_SelectedIndexChanged(sender, e);
                }

                // save
                globaldt.SaveToFile(globaldt);
                Log.WriteToLog(" \"" + lbSelectMeaning.Items[lbSelectMeaning.SelectedIndex] + "\" for \"" + lbSelectWord.Items[lbSelectWord.SelectedIndex] + "\" in \"" + lbWords.Items[lbWords.SelectedIndex]+ "\"",true,true);
            }
        }

        private void btnCopyMag_Click(object sender, EventArgs e)
        {
            if (globaldt == null)
            {
                globaldt = new DataHandler();
            }
            string[] Files = globaldt.InitCopyToMag();

            if (Files == null)
            {
                MessageBox.Show("Kee Magasinn gesat!");
                return;
            }

            Pietschsoft.NativeProgressDialog progressDialog = new Pietschsoft.NativeProgressDialog(this.Handle)
            {
                Title = "Kopéieren vun den Popupen an den Magasinn",
                CancelMessage = "Eieiei... Da wart elo...",
                Maximum = 100,
                Value = 0,
                Line3 = "Calculating Time Remaining..."
            };

            progressDialog.ShowDialog(Pietschsoft.NativeProgressDialog.PROGDLG.Modal, Pietschsoft.NativeProgressDialog.PROGDLG.AutoTime, Pietschsoft.NativeProgressDialog.PROGDLG.NoMinimize);
            int c = 1;
            foreach (string _file in Files)
            {
                progressDialog.Line1 = "Datei: " + Path.GetFileName(_file);
                globaldt.CopyToMag(_file);

                System.Threading.Thread.Sleep(50);
                double dbl = 100d / Files.Count() * c;
                uint _currprog = Convert.ToUInt32(Math.Round(dbl));
                progressDialog.Line2 = "Oofgeschloss zu " + _currprog.ToString() + "%";
                progressDialog.Value = _currprog;
                c++;
            }
            progressDialog.CloseDialog();

            globaldt.IsInMag(true);
        }


        private void StartCopyToMagazine()
        {
            //CopyToMagazine();
            UpdatebtnCopyToMag(false);
        }

        public void UpdatebtnCopyToMag(Boolean _bool)
        {
            if (ControlInvokeRequired(btnCopyToMag, () => UpdatebtnCopyToMag(_bool)))
                return;
            btnCopyToMag.Enabled = _bool;
        }

        private void TsmiExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (globaldt != null)
            {
                if (!globaldt.HasPopups())
                {
                    if (MessageBox.Show("D'Popupen goufen nach net erstallt. Zoumaachen?", "Zoumaachen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    { e.Cancel = false; }
                    else
                    { e.Cancel = true; }
                }
                else
                {
                    if (!globaldt.IsInMag())
                    {
                        if (MessageBox.Show("D'Popupen goufen nach net an den Magasinn kopéiert. Zoumaachen?", "Zoumaachen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        { e.Cancel = false; }
                        else
                        { e.Cancel = true; }
                    }
                }
            }
        }

        private void MagazineOpmaachenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //
            if (globaldt == null)
            {
                globaldt = new DataHandler();
            }
            else
            {
                TsmiNew_Click(sender, e);
            }
            if (!globaldt.InitParseMagazine())
                MagazineSelectorToolStripMenuItem_Click(sender, e);
        }

        private void TsmiNew_Click(object sender, EventArgs e)
        {
            if (SafeToClear())
                ResetInstance();
        }

        private bool SafeToClear()
        {
            if (globaldt != null)
            {
                if (MessageBox.Show(this, "Wëlls de wirklech resetten?", "Sëcher?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        private void ResetInstance()
        {
            if (tssArticle.Text != "_ART_")
            {
                Log.WriteToLog(tssArticle.Text + " closed");
                Log.CloseLog();
            }
            btnCreatePopups.Enabled = false;
            btnCopyToMag.Enabled = false;
            tsbCustomAudio.Enabled = false;
            tsbPlayAudio.Enabled = false;
            tsbCustomEN.Enabled = false;
            tsbCustomPT.Enabled = false;
            tsmiSave.Enabled = false;
            tsmiText.Enabled = false;
            tssArticle.Text = "_ART_";
            tssMagazine.Text = "_MAG_";
            tssCustomColor.BackColor = SystemColors.Control;
            lbSelectMeaning.Items.Clear();
            lbSelectWord.Items.Clear();
            lbWords.Items.Clear();
            rtbDetails.Clear();
            if (TextForm!=null) TextForm.Close();
            generateTextForm();
            TextForm.Owner = this;
            if (TextForm.Visible)
            {
                TextForm.Hide();
            }
            globaldt = null;
        }

        private void SetIsSaved(bool setter)
        {
            if (globaldt == null)
            {
                globaldt = new DataHandler();
            }
            globaldt.IsSaved(setter);
            if (setter)
                tssNeedSave.Image = null;
            else
                tssNeedSave.Image = Properties.Resources.SaveStatusBar8_16x;
        }

        private void btnPlayAudio_Click(object sender, EventArgs e)
        {
            string mp3Path = globaldt.getTemppath() + "WebResources\\popupbase-web-resources\\audio\\";
            string mp3File = mp3Path + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3;
            wplayer = new WindowsMediaPlayer();
            wplayer.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
            wplayer.URL = mp3File;
            tsbPlayAudio.Enabled = false;
            wplayer.controls.play();


        }
        // https://electronic-designer.net/c_sharp/audio/playing-audio-with-wmplib-windowsmediaplayer
        //*************************************************
        //*************************************************
        //********** MP3 PLAYER FINISHED PLAYING **********
        //*************************************************
        //*************************************************
        void wplayer_PlayStateChange(int NewState)
        {
            if (NewState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                wplayer.close();
                tsbPlayAudio.Enabled = true;
                wplayer = null;

                try
                {
                    System.Diagnostics.Process[] prc = System.Diagnostics.Process.GetProcessesByName("wmplayer");
                    if (prc.Length > 0)
                        prc[prc.Length - 1].Kill();
                }
                catch (Exception)
                {
                }
            }
        }

        private void btnCustomAudio_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(globaldt.Filepath + "WebResources\\popupbase-web-resources\\audio\\" + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (CustomAudioBrowser.ShowDialog() == DialogResult.OK)
            {
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3 = Path.GetFileName(CustomAudioBrowser.FileName);
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].hasCustomAudio = true;
                globaldt.GetMp3(globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3,
                    globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].hasCustomAudio);

                LbSelectMeaning_SelectedIndexChanged(sender, e);
                Log.WriteToLog("Audio for " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].LU + " changed");
            }
        }

        private void textUweisenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsmiText.Checked)
            {
                TextForm.Hide();
                tsmiText.Checked = false;
            }
            else
            {
                if (globaldt.DocPath == null)
                {
                    if (ArticleBrowser.ShowDialog() == DialogResult.OK)
                    {
                        globaldt.DocPath = ArticleBrowser.FileName;
                        globaldt = OpenDocument(globaldt);
                        TextForm.Show();
                        tsmiText.Checked = true;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    // globaldt = OpenDocument(globaldt);
                    TextForm.Show();
                    tsmiText.Checked = true;
                }
            }
        }

        private void iNDesignConnectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tssInDesign.Text = INDConnector.getBook() ? "Connected!" : "Connection failed";
        }

        private void bookContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iNDesignConnectorToolStripMenuItem_Click(sender, e);
            var list = INDConnector.getBookContent();
            bookContentToolStripMenuItem.DropDownItems.Clear();
            ToolStripMenuItem[] slist = new ToolStripMenuItem[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                slist[i] = new ToolStripMenuItem();
                slist[i].Name = i.ToString();
                slist[i].Text = list[i];
                slist[i].Click += new EventHandler(BookItemClickHandler);
            }
            bookContentToolStripMenuItem.DropDownItems.AddRange(slist);
        }

        private void tssExperimental_Click(object sender, EventArgs e)
        {
            bookContentToolStripMenuItem_Click(sender, e);
        }

        private void BookItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            Console.WriteLine(clickedItem.ToString());
            DialogResult dialogResult = MessageBox.Show("Wells de den Artikel " + clickedItem.ToString() + " lueden?", "Artikel " + clickedItem.ToString(), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                String APath = INDConnector.getPathOfArticleInBook(clickedItem.ToString());
                Console.WriteLine(APath);
                //ArticleWorker(APath);
            }
        }

        private void ToolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void EegenFaarfSetzenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmColor frmColor = new frmColor();
            frmColor.pnlColor.BackColor = ArticleColor;
            frmColor.ArticleColor = ArticleColor;
            frmColor.OArticleColor = OArticleColor;

            if (globaldt.CustomColor == true)
            {
                frmColor.rbEegen.Checked = true;
            }
            if (frmColor.ShowDialog() == DialogResult.OK)
            {
                ArticleColor = frmColor.pnlColor.BackColor;
                tssCustomColor.BackColor = ArticleColor;
                globaldt.CustomColor = true;
                globaldt.Globrgb = ArticleColor.R + "," + ArticleColor.G + "," + ArticleColor.B;
                Log.WriteToLog("Color changed to " + globaldt.Globrgb);
            }
            else
            {

            }
        }

        private void TsbCustomPT_Click(object sender, EventArgs e)
        {
            InputDialog PTid = new InputDialog()
            {
                MainInstruction = "Eng Bedeitung antippen:",
                Content = "DE: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].DE + "; FR: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].FR,
                WindowTitle = "Portugisesch Iwwersetzung",
                Input = globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].PT
            };
            if (PTid.ShowDialog(this) == DialogResult.OK)
            {
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].PT = PTid.Input;
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].hasCustomPT = true;
            }
            PTid.Dispose();
            LbSelectMeaning_SelectedIndexChanged(sender, e);
            Log.WriteToLog("PT for " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].LU + " changed");
        }

        private void TlbCustomEN_Click(object sender, EventArgs e)
        {
            InputDialog ENid = new InputDialog()
            {
                MainInstruction = "Eng Bedeitung antippen:",
                Content = "DE: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].DE + "; FR: " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].FR,
                WindowTitle = "Englesch Iwwersetzung",
                Input = globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].EN

            };
            if (ENid.ShowDialog(this) == DialogResult.OK)
            {
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].EN = ENid.Input;
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].hasCustomEN = true;
            }
            ENid.Dispose();
            LbSelectMeaning_SelectedIndexChanged(sender, e);
            Log.WriteToLog("EN for " + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].LU + " changed");

        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.CloseLog();
        }

        private void PublishStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get Indesign File and open it
            string tempstring = globaldt.Filename.Replace("_.wordslist", "");
            
            string[] filePaths = Directory.GetFiles(globaldt.Filepath, tempstring+"*.indd");
            if (filePaths.Length > 0) //Has found the Indesign File
            {

                Pietschsoft.NativeProgressDialog progressDialog = new Pietschsoft.NativeProgressDialog(this.Handle)
                {
                    Title = "Article exportéieren....",
                    CancelMessage = "D'Fënster get nom Export vum selwen zou....",
                    Maximum = 2,
                    Value = 0,
                    Line1 = "Den Artikel gëtt exportéiert...",
                    Line2 = "Artikel " +globaldt.Filename.Substring(0, 4),
                    Line3 = "D'Fënster get nom Export vum selwen zou"
                };

                progressDialog.ShowDialog(Pietschsoft.NativeProgressDialog.PROGDLG.Modal, Pietschsoft.NativeProgressDialog.PROGDLG.AutoTime, Pietschsoft.NativeProgressDialog.PROGDLG.NoMinimize);

                string indesignfilepath = filePaths[0];

                Type oType = Type.GetTypeFromProgID("InDesign.Application");
                dynamic oIndesign = Activator.CreateInstance(oType);
                dynamic oDocument = oIndesign.open(indesignfilepath);
                
                string scriptpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Adobe\InDesign\Version 14.0\en_US\Scripts\Scripts Panel\KB4\02_export_article.jsx";

                string[] myParams = { globaldt.getTemppath(), globaldt.targetMag };

                //oIndesign.DoScript(@scriptpath, InDesign.idScriptLanguage.idJavascript, myParams);
                oDocument.Save();
                oDocument.Close();

                if (twixlAPI == null) { twixlAPI = new TwixlAPI(); }

                progressDialog.Value=1;
                progressDialog.Line1 = "Den Artikel gëtt eropgelueden...";

                string Result = Path.GetFileNameWithoutExtension(indesignfilepath);

                string temppath = globaldt.getTemppath() + globaldt.targetMag + @"\export\articles\" + Result + ".article";

                var ggtask = twixlAPI.uploadIssue(temppath);
                var gg = ggtask.Result;

                frmTwixlCategorySelector _frmTwixlCategorySelector = new frmTwixlCategorySelector();

                int _issueid = gg.issue.id;

                _frmTwixlCategorySelector.cbCategories.Items.Clear();
                foreach (TwixlCategory cat in twixlAPI._twixlCategories.categories)
                {

                    _frmTwixlCategorySelector.cbCategories.Items.Add(cat.name, twixlAPI.IsInCategory(_issueid, cat.id));
                }

                if (_frmTwixlCategorySelector.ShowDialog() == DialogResult.OK)
                {
                    var sel = _frmTwixlCategorySelector.cbCategories.CheckedIndices.Cast<int>().ToArray();
                    twixlAPI.pushIssueToCategory(_issueid, sel);
                }

                Console.WriteLine();

                progressDialog.CloseDialog();

            }

           
            //Show ModalWindow, so the User has a Chance to export using the InDesign Twixl export engine
        }

        private void TwixlAPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TwixlAPIForm twixlAPIForm = new TwixlAPIForm();
            twixlAPIForm.ShowDialog();
        }

        private void TsmUseProxy_Click(object sender, EventArgs e)
        {
            _Globals.useProxy = !_Globals.useProxy;
            tsmUseProxy.Checked = _Globals.useProxy;
        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //
            
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Reload this word from LOD
            if (lbWords.SelectedIndex >= 0)
            {
                string theWord = lbWords.SelectedItem.ToString();
                AutoComplete acword = await Task.Run(async () => await ReturnFullAutoComplete(theWord));

                //(acword.Occurence)
                var res = acword.CompareAutoComplete(acword, globaldt.WordList[lbWords.SelectedIndex], globaldt, lbWords.SelectedIndex);
                if (res)
                {
                    MessageBox.Show(this, "Update fir d'Wuert "+theWord+" beim LOD fonnt gin.", "Upsi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LbSelectWord_SelectedIndexChanged(sender, e);
                    globaldt.SaveToFile(globaldt);
                } else
                {
                    MessageBox.Show(this, "Keen neien Update fir d'Wuert " + theWord + ".", "Alles ok", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                Console.Write("");
            }
        }
    }


    public static class _Globals
    {
        public static bool useProxy = false;
    }
}