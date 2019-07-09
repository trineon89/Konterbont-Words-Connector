using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WMPLib;

using Independentsoft.Office.Odf;
using IStyles = Independentsoft.Office.Odf.Styles;

using Ookii.Dialogs.WinForms;

namespace KonterbontLODConnector
{
    public partial class frmMain : Form
    {
        public string MagazinePath = "\\\\cubecluster\\Konterbont_Produktioun\\Magazines\\";
        public string ArticlePath = "\\\\cubecluster\\Konterbont_Produktioun\\Artikelen\\";
        public string CustomAudioPath = "\\\\cubecluster\\Konterbont_Audio\\";
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
            InitialDirectory = "\\\\cubecluster\\Konterbont_Audio\\",
            RestoreDirectory = true,
            Title = "Neien Toun fir den Popup auswielen"
        };

        public ProgressDialog progressDialog;
        private INDesignPlugin iNDesignPlugin;
        public DataHandler globaldt = null;

        private Form TextForm = new Form()
        {
            Name = "frmTextForm",
            Text = "Dokument",
            Width = 500,
            Height = 500,
            Top = 0,
            Left = 0,
            MaximizeBox = false,
            MinimizeBox = false,
            FormBorderStyle = FormBorderStyle.FixedToolWindow
        };

        RichTextBox rtb = new RichTextBox()
        {
            Dock = DockStyle.Fill,
            Name = "rtbDocument",
            ReadOnly = true,
            BackColor = SystemColors.Window,
            Font = new Font("Arial", 12)

        };

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
            TextForm.Owner = this;
        }
      
        private async Task<AutoComplete> GetFullTranslationsAsync(string searchstring, bool compare = true)
        {
            return await Task.Run(() => GetFullTranslations(searchstring, compare));
        }
        
        private async Task<AutoComplete> GetFullTranslations(string searchstring, bool compare)
        {
            Task<string> task = Task.Run(async () => await FetchXMLasync(searchstring));
            task.Wait();
            string fetchedXml = task.Result;

            AutoComplete acwuert = ParseXMLWords(fetchedXml, compare, searchstring);

            Task<AutoComplete> taskLU = FetchFullWordsAsync(acwuert, "LU", !compare);
            Task<AutoComplete> taskDE = FetchFullWordsAsync(acwuert, "DE", !compare);
            Task<AutoComplete> taskFR = FetchFullWordsAsync(acwuert, "FR", !compare);
            Task<AutoComplete> taskEN = FetchFullWordsAsync(acwuert, "EN", !compare);
            Task<AutoComplete> taskPT = FetchFullWordsAsync(acwuert, "PT", !compare);

            var ret = await Task.WhenAll(taskLU, taskDE, taskFR, taskEN, taskPT);
            return ret.First();
        }

        private async Task<string> FetchXMLasync(string Word)
        {
            return await Task.Run(() => FetchXML(Word));
        }

        private async Task<string> FetchXML(string Word)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/lod-search.php?v=H&s=lu&w=" + Word.ToLower()), //Word has to be Lowercase
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
            return responseBody;
        }
        
        private async Task<string> HttpRequest(string Lang, string XML)
        {
            HttpClient httpClient = new HttpClient();
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


            var _responseT = httpClient.SendAsync(httpContent, new HttpCompletionOption());
            /*
            var _response = await httpClient.SendAsync(httpContent, new HttpCompletionOption());
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            return  responseBody;
            */
            _responseT.Wait();
            var _response = _responseT.Result;
            _response.EnsureSuccessStatusCode();

            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            return responseBody;

        }
        
        private async Task<AutoComplete> FetchFullWordsAsync(AutoComplete acwuert, string Lang, bool showselection = false)
        {
            AutoComplete reswuert = new AutoComplete();
            reswuert.Occurence = acwuert.Occurence;
            int _c = 1;
            int _MeaningsCount = 0;
            int _Total = 0;

            frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();

            foreach (Wuert wuert in acwuert.Wierder)
            {
                string responseBody = await HttpRequest(Lang, wuert.XMLFile);

                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(responseBody);

                // Meanings START
                if (htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
                {
                    int _i = 1;
                    string Selection = "";

                    if (Lang == "LU")
                    {
                        if (htmlDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                        {
                            wuert.WuertForm.WuertForm = htmlDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim();
                        }
                        else
                        {
                            wuert.WuertForm.WuertForm = htmlDocument.DocumentNode.SelectSingleNode(".//span[@class='klass'][2]").InnerText.Trim();
                        }
                    }

                    HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']");
                    foreach (HtmlNode Meaning in htmlNodes)
                    {
                        string MeaningText = "";
                        string MeaningTextAdd = "";
                        string Pluriel;

                        HtmlNode[] LUsArray = htmlDocument.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();

                        if (htmlDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                        {
                            //HtmlNode[] MeaningArray;
                            if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']") != null) //has base pluriel
                            {
                                if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']") != null) //Failsafe pluriel
                                {
                                    Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                                    Pluriel = Pluriel.Replace("&lt;", "<");
                                    Pluriel = Pluriel.Replace("&gt;", ">");
                                }
                                else
                                    Pluriel = null;
                            }
                            else
                                Pluriel = null; // no base pluriel
                        }
                        else
                        {
                            if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='text_gen']") != null) //has base pluriel
                            {
                                if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='text_gen']/span[@class='mentioun_adress']") != null) //Failsafe pluriel
                                {
                                    Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                                    Pluriel = Pluriel.Replace("&lt;", "<");
                                    Pluriel = Pluriel.Replace("&gt;", ">");
                                }
                                else
                                    Pluriel = null;
                            }
                            else
                                Pluriel = null; // no base pluriel                       
                        }
                        wuert.WuertLuS = Pluriel;

                        Meaning meaning = new Meaning();

                        if (wuert.WuertForm.WuertForm == "Verb" || wuert.WuertForm.WuertForm == "Modalverb")
                        {
                            meaning.LU = wuert.WuertLu;
                            meaning.HV = Pluriel; // writes "Hëllefsverb" to class
                                                  // write PP to LUs variable
                            if (htmlDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                            {
                                if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress']") != null)
                                {
                                    Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress']").InnerText;
                                    if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress'][2]") != null)
                                    {
                                        string Pluriel2 = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress'][2]").InnerText;
                                        Pluriel = Pluriel + " / " + Pluriel2;
                                    }
                                    Pluriel = Pluriel.Replace("&lt;", "<");
                                    Pluriel = Pluriel.Replace("&gt;", ">");
                                }
                                else
                                    Pluriel = null;

                            }
                            else
                            {
                                if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress']") != null)
                                {
                                    Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress']").InnerText;
                                    if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress'][2]") != null)
                                    {
                                        string Pluriel2 = Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress'][2]").InnerText;
                                        Pluriel = Pluriel + " / " + Pluriel2;
                                    }
                                    Pluriel = Pluriel.Replace("&lt;", "<");
                                    Pluriel = Pluriel.Replace("&gt;", ">");
                                }
                                else
                                    Pluriel = null;
                            }
                            meaning.LUs = Pluriel;
                            wuert.WuertLuS = Pluriel;
                        }
                        else
                        {
                            if (Lang == "LU")
                            {

                                /*
                                 * 1 Meaning (kee Pluriel) ::html:: <span class=text_gen> ( <span class=info_plex>kee Pluriel</span>  ) </span>
                                 * 
                                 * 2 Meaning (Pluriel PlurielWuert) ::html:: <span class=text_gen> ( <span class=info_plex>Pluriel <span class=mentioun_adress> 
                                 *      <span class=mentioun_adress> PlurielWuert </span> </span> </span>  ) </span>            
                                 * 
                                 * 3 Meaning SpecialWuert ::html:: <span class=polylex> SpecialWuert </span>
                                 * 
                                 * 4 Meaning DE Wuert ::html:: <span class=intro_et> ..... </span>
                                 * 
                                 */

                                if (Meaning.SelectSingleNode(".//span[@class='text_gen']") != null)
                                { // Meaning 1 or 2 or 4
                                    meaning.LU = wuert.WuertLu;
                                    meaning.LUs = Pluriel;
                                    Console.WriteLine("Meaning 1, 2, 4");
                                    if (Meaning.SelectSingleNode(".//span[@class='text_gen'][1]").ChildNodes.Count() == 3)
                                    { // -> Meaning 1
                                        Console.WriteLine("Meaning 1");
                                        if (Meaning.SelectSingleNode(".//span[@class='polylex']") != null)
                                        {
                                            meaning.LU = Meaning.SelectSingleNode(".//span[@class='polylex']").InnerText;
                                        }
                                        //meaning.LUs = null;
                                    }
                                    else
                                    {
                                        if (Meaning.SelectSingleNode(".//span[@class='polylex']") != null)
                                        { // -> Meaning 3
                                            Console.WriteLine("Meaning 3");
                                            meaning.LU = Meaning.SelectSingleNode(".//span[@class='polylex']").InnerText;
                                            meaning.LUs = wuert.WuertLuS;
                                        }
                                        else if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                                        { // Meaning 2 or 4
                                            Console.WriteLine("Meaning 2, 4");
                                            if (Meaning.SelectSingleNode(".//span[@class='info_flex']") != null)
                                            { // -> Meaning 2
                                                Console.WriteLine("Meaning 2");
                                                meaning.LUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
                                            }
                                            else
                                            { // -> Meaning 4
                                                Console.WriteLine("Meaning 4");
                                            }
                                        }
                                        else
                                        { // -> Meaning 1
                                            Console.WriteLine("Meaning 1");
                                            meaning.LUs = null;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Meaning.SelectSingleNode(".//span[@class='polylex']") != null)
                                    { // -> Meaning 3
                                        Console.WriteLine("Meaning 3");
                                        meaning.LU = Meaning.SelectSingleNode(".//span[@class='polylex']").InnerText;
                                        meaning.LUs = wuert.WuertLuS;
                                    }
                                    else
                                    { // -> Meaning 4 safe
                                        Console.WriteLine("Meaning 4 (safe)");
                                        meaning.LUs = wuert.WuertLuS;
                                        meaning.LU = wuert.WuertLu;
                                    }
                                }
                            }
                        }
                        
                        // Source: https://stackoverflow.com/questions/541954/how-would-you-count-occurrences-of-a-string-actually-a-char-within-a-string
                        // check expression
                        if (meaning.LU != null)
                        {
                            int count = meaning.LU.Count(f => f == ' ');
                            if (count > 0)
                            {
                                meaning.LUs = null;
                            }
                        }
                        //
                        if (Lang != "LU")
                        {
                            // var ModMean = Meaning;
                            int i = 0;
                            while (Meaning.InnerHtml.Contains("intro_et"))
                            {
                                Meaning.ChildNodes[i].Remove();
                            }

                            var RemoveNode = Meaning.SelectSingleNode("./div[@class='bspsblock']");
                            if (RemoveNode != null)
                                Meaning.RemoveChild(RemoveNode);
                            RemoveNode = Meaning.SelectSingleNode("./div[@class='syn_block']");
                            if (RemoveNode != null)
                                Meaning.RemoveChild(RemoveNode);

                            Console.Write(Meaning.InnerText);

                            MeaningText = Meaning.InnerText;

                            string regex = "(\\&lt;.*\\&gt;)";
                            MeaningText = Regex.Replace(MeaningText, regex, "");

                            if (MeaningText.Contains("--- coming soon ---") || MeaningText.Contains("--- disponível em breve ---"))
                            {
                                MeaningText = null;


                            }
                        }

                        switch (Lang)
                        {
                            case "LU":
                                meaning.MP3 = wuert.MP3;
                                HtmlNodeCollection htmlExamples = Meaning.SelectNodes(".//span[@class='beispill']");
                                foreach (HtmlNode htmlexample in htmlExamples)
                                {
                                    var RemoveNode = htmlexample.SelectSingleNode("span[@id='sprangop']");
                                    if (RemoveNode != null)
                                    {
                                        htmlexample.RemoveChild(RemoveNode);
                                    }

                                    string EGS = "";
                                    if (htmlexample.SelectSingleNode(".//span[@class='text_gen']") != null)
                                    {
                                        EGS = htmlexample.SelectSingleNode(".//span[@class='text_gen']").InnerText;
                                    }

                                    RemoveNode = htmlexample.SelectSingleNode(".//span[@class='text_gen']");
                                    if (RemoveNode != null)
                                    {
                                        htmlexample.RemoveChild(RemoveNode);
                                    }
                                    Example example = new Example(htmlexample.InnerText, EGS.Trim());

                                    meaning.Examples.Add(example);
                                }
                                break;
                            case "DE":
                                wuert.Meanings[_i - 1].DE = MeaningText + MeaningTextAdd;
                                break;
                            case "FR":
                                wuert.Meanings[_i - 1].FR = MeaningText + MeaningTextAdd;
                                break;
                            case "EN":
                                wuert.Meanings[_i - 1].EN = MeaningText + MeaningTextAdd;
                                break;
                            case "PT":
                                wuert.Meanings[_i - 1].PT = MeaningText + MeaningTextAdd;
                                break;
                        }
                        Selection = (_i + 1).ToString();

                        // Wuert wuert = acwuert.Wierder[acwuert.Selection - 1];
                        if (Lang == "PT")
                        {
                            if (showselection && acwuert.Selection == _c)
                            {
                                string thename = _i.ToString();
                                RadioButton rb = new RadioButton
                                {
                                    Name = thename,
                                    Text = thename,// + " DE: " + wuert.Meanings[_i - 1].DE + "; FR: " + wuert.Meanings[_i - 1].FR + "; EN:" + wuert.Meanings[_i - 1].EN + "; PT:" + wuert.Meanings[_i - 1].PT,

                                    Location = new Point(10, _i * 30),
                                    Width = 100
                                };
                                if (_i == 1)
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

                                foreach (Example _ex in wuert.Meanings[_i - 1].Examples)
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
                                frmSelectMeaning.rtbDE.AppendText(thename + ". " + wuert.Meanings[_i - 1].DE + Environment.NewLine);
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
                                frmSelectMeaning.rtbFR.AppendText(thename + ". " + wuert.Meanings[_i - 1].FR + Environment.NewLine);
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
                                frmSelectMeaning.rtbEN.AppendText(thename + ". " + wuert.Meanings[_i - 1].EN + Environment.NewLine);
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
                                frmSelectMeaning.rtbPT.AppendText(thename + ". " + wuert.Meanings[_i - 1].PT + Environment.NewLine);
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

                                _MeaningsCount = htmlNodes.Count();
                            }
                        }

                        if (Lang == "LU")
                        { wuert.Meanings.Add(meaning); }
                        _i++;
                        _Total = _i;
                    }
                }
                

                RadioButton rbtn = new RadioButton
                {
                    Name = "0",
                    Text = "Passt nët",
                    Location = new Point(10, _Total * 30),
                    Width = 100
                };
                frmSelectMeaning.gbMeanings.Controls.Add(rbtn);


                if (_MeaningsCount > 1 && Lang == "PT" && _c == acwuert.Selection)
                {
                    frmSelectMeaning.gbMeanings_Click(this, null);
                    ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.HighlightSelText(TextForm.Controls.OfType<RichTextBox>().First(), acwuert.Occurence));
                    ControlInvokeRequired(TextForm, () => TextForm.Activate());
                    if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                    {
                        RadioButton selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                        ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.UnSelText(TextForm.Controls.OfType<RichTextBox>().First()));
                        string Selection = selectedMeaning.Name;
                        wuert.Selection = Int32.Parse(Selection);
                        //TheResults.Wierder[TheResults.Selection].Selection = Int32.Parse(Selection);
                    }
                    else
                    {
                        return null;
                    }

                    if (wuert.Meanings[wuert.Selection - 1].EN == "")
                    {
                        InputDialog ENid = new InputDialog();
                        ENid.MainInstruction = "Eng Bedeitung antippen:";
                        ENid.Content = "DE: " + wuert.Meanings[wuert.Selection - 1].DE + "; FR: " + wuert.Meanings[wuert.Selection - 1].FR;
                        ENid.WindowTitle = "Englesch Iwwersetzung";
                        if (ENid.ShowDialog() == DialogResult.OK)
                        {
                            wuert.Meanings[wuert.Selection - 1].EN = ENid.Input;
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
                        }
                        PTid.Dispose();
                    }
                }


                if (_MeaningsCount <= 1 && Lang == "PT" && _c == acwuert.Selection)
                {
                    if (wuert.Meanings[wuert.Selection - 1].EN == "")
                    {
                        InputDialog ENid = new InputDialog()
                        {
                            MainInstruction = "Eng Bedeitung antippen:",
                            Content = "DE: " + wuert.Meanings[wuert.Selection - 1].DE + "; FR: " + wuert.Meanings[wuert.Selection - 1].FR,
                            WindowTitle = "Englesch Iwwersetzung"
                        };
                        if (ENid.ShowDialog() == DialogResult.OK)
                        {
                            wuert.Meanings[wuert.Selection - 1].EN = ENid.Input;
                        }
                        ENid.Dispose();
                    }

                    if (wuert.Meanings[wuert.Selection - 1].PT == "")
                    {
                        InputDialog PTid = new InputDialog()
                        {
                            MainInstruction = "Eng Bedeitung antippen:",
                            Content = "DE: " + wuert.Meanings[wuert.Selection - 1].DE + "; FR: " + wuert.Meanings[wuert.Selection - 1].FR,
                            WindowTitle = "Portugisesch Iwwersetzung"
                        };
                        if (PTid.ShowDialog() == DialogResult.OK)
                        {
                            wuert.Meanings[wuert.Selection - 1].PT = PTid.Input;
                        }
                        PTid.Dispose();
                    }
                }

                if (globaldt == null)
                {
                    globaldt = new DataHandler();
                }
                reswuert.Wierder.Add(wuert);
                _c++;
            }
            
            reswuert.Selection = acwuert.Selection;
            return reswuert;
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

        
        private AutoComplete ParseXMLWords(string XML, bool onlycompare = false, string occurence = null)
        {
            AutoComplete ac = new AutoComplete();
            ac.Occurence = occurence;

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            HtmlNode[] htmlNodes;

            htmlDocument.LoadHtml(XML);
            htmlNodes = htmlDocument.DocumentNode.SelectNodes("//a[@onclick]").ToArray();

            frmSelectMeaning frm = new frmSelectMeaning();

            int _i = 1;
            string attrib = null;

            foreach (HtmlNode htmlNode in htmlNodes)
            {
                attrib = htmlDocument.DocumentNode.SelectNodes("//a[@onclick]")[_i - 1].GetAttributeValue("onclick", "default");
                string tmpxml = attrib.Remove(0, attrib.IndexOf("'") + 1); // result: PERSOUN1.xml','persoun1.mp3')
                string tmpmp3 = tmpxml.Remove(0, tmpxml.IndexOf("'") + 1); // result: ,'persoun1.mp3')
                tmpmp3 = tmpmp3.Remove(0, tmpmp3.IndexOf("'") + 1);

                string tmpWordForm = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='s4']").InnerText.Trim();
                htmlNode.RemoveChild(htmlDocument.DocumentNode.SelectNodes("//span[@class='s4']").First());

                Wuert wuert = new Wuert
                {
                    WuertLu = htmlNode.InnerText.Trim(),
                    WuertForm = new WordForm(null),
                    Selection = 1,
                    XMLFile = tmpxml.Substring(0, tmpxml.IndexOf("'")),
                    MP3 = tmpmp3.Substring(0, tmpmp3.IndexOf("'"))
                };

                //ac.Wierder.Add(wuert);

                RadioButton rb = new RadioButton
                {
                    Name = _i.ToString(),
                    Text = wuert.WuertLu + " (" + tmpWordForm + ")",
                    Location = new Point(10, _i * 30),
                    Width = 500
                };
                if (_i == 1)
                { rb.Checked = true; }

                Task<string> task = Task.Run(async () => await GetSelectionTooltipAsync(wuert.XMLFile));
                task.Wait();
                string tooltip = task.Result;
                if (tooltip.Contains("Variant"))
                {
                    rb.Enabled = false;
                    var result = tooltip.Substring(tooltip.LastIndexOf(' ') + 1);
                    rb.Text = rb.Text + " (Variant vun " + result + ")";
                    wuert.IsVariant = true;
                }
                ac.Wierder.Add(wuert);
                frm.gbMeanings.Dock = DockStyle.Fill;
                frm.tcLang.Visible = false;
                frm.gbMeanings.Controls.Add(rb);
                frm.gbMeanings.Text = "Wuert auswielen:";
                frm.Text = "Wuert auswielen";
                frm.tpInfo.SetToolTip(rb, tooltip);

                _i++;
            }

            if (onlycompare)
                return ac;

            if (htmlNodes.Count() > 1)
            {
                ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.HighlightSelText(TextForm.Controls.OfType<RichTextBox>().First(), ac.Occurence));
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ControlInvokeRequired(TextForm.Controls.OfType<RichTextBox>().First(), () => Utility.UnSelText(TextForm.Controls.OfType<RichTextBox>().First()));
                    RadioButton radioButton = frm.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    ac.Selection = Int32.Parse(radioButton.Name);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                ac.Selection = 1;
            }
            return ac;
        }

    /// <summary>
    /// Calls the Async Function GetSelectionTooltip
    /// </summary>
    /// <param name="XMLTT"></param>
    /// <returns></returns>
    private async Task<string> GetSelectionTooltipAsync(string XMLTT)
    {
        return await Task.Run(() => GetSelectionTooltip(XMLTT));
    }
   
    /// <summary>
    /// Calls the functions for the Tooltip(s) of the Selection Form
    /// </summary>
    /// <param name="XMLTT">Den LOD XML-Fichiersnumm</param>
    /// <returns></returns>
    private async Task<string> GetSelectionTooltip(string XMLTT)
    {
        string tooltip = null;

        string tooltipLU = await Task.Run(() => FetchWordsTT(XMLTT, "LU"));
        string tooltipDE = await Task.Run(() => FetchWordsTT(XMLTT, "DE"));
        string tooltipFR = await Task.Run(() => FetchWordsTT(XMLTT, "FR"));

        tooltip = tooltipLU + tooltipDE + tooltipFR;

        return tooltip;
    }

        /*
        private async Task<Boolean> CheckIfWordHasChangedAsync(string searchstring, List<AutoComplete> ac)
        {
            return await Task.Run(() => CheckIfWordHasChanged(searchstring, ac));
        }*/

        /*
        private async Task<Boolean> CheckIfWordHasChanged(string searchstring, List<AutoComplete> ac)
        {
            AutoComplete acresults = await Task.Run(async () => await GetFullTranslationsAsync(searchstring));

            var atmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.WuertLu == x.WuertLu))));
            var btmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.MP3 == x.MP3))));
            var ctmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.WuertForm.WuertForm == x.WuertForm.WuertForm))));
            var dtmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.XMLFile == x.XMLFile))));
            var etmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (acx.Occurence == searchstring))));
                                                                                                                                                //&& (b.WuertLuS == x.WuertLuS)
            var tmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.WuertLu == x.WuertLu) && (b.MP3 == x.MP3)
                && (b.WuertForm.WuertForm == x.WuertForm.WuertForm) && (b.XMLFile == x.XMLFile) && (acx.Occurence == searchstring))));

            if (tmp == null)
            {
                return true;
            }
            else
            {
                bool res = acresults.DeepCheck(acresults, tmp);
                return res;
            }
        }*/

        private DataHandler OpenDocument(DataHandler dt)
        {
            /*TextDocument doc = null;
            try
            {
                doc = new TextDocument(dt.DocPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return dt;
            }

            dt.StyleName = new List<string>();
            foreach (var style in doc.AutomaticStyles.Styles)
            {
                if (style.GetType() == typeof(IStyles.TextStyle))
                {
                    IStyles.TextStyle textstyle = (IStyles.TextStyle)style;
                    if (textstyle.BackgroundColor != null && textstyle.BackgroundColor != "#ffffff" && textstyle.BackgroundColor != "transparent")
                    {
                        dt.StyleName.Add(textstyle.Name);
                    }
                }
            }

            Color myRgbColor = Color.FromArgb(20, 118, 212);

            IList<AttributedText> attTexts = doc.GetAttributedTexts();
            IList<Section> Texts = doc.GetSections();
            string text = "";
            Font Bold = new Font(rtb.SelectionFont, FontStyle.Bold);
            Font Normal = new Font(rtb.SelectionFont, FontStyle.Regular);
            IContentElement parent = null;
            for (int _i = 0; _i < attTexts.Count; _i++)
            {
                var style = attTexts[_i].Style.ToString();

                text = attTexts[_i].Content[0].ToString();


                
                text = text.Replace("&quot;", "\"");
                text = text.Replace("&apos;", "'");
                IContentElement tempparent = attTexts[_i].GetParent();
                if (parent == null)
                {
                    parent = tempparent;
                }

                if (tempparent != parent && parent.GetType() == typeof(Independentsoft.Office.Odf.Paragraph) && tempparent.GetType() == typeof(Independentsoft.Office.Odf.Paragraph))
                {
                    rtb.AppendText(Environment.NewLine + Environment.NewLine);
                    parent = tempparent;
                }

                if (dt.StyleName.Contains(style))
                {
                    rtb.SelectionColor = myRgbColor;
                    rtb.SelectionFont = Bold;
                    rtb.AppendText(text);
                    rtb.SelectionColor = Color.Black;
                    rtb.SelectionFont = Normal;
                }
                else
                {
                    if (!text.Contains("<"))
                    {
                        rtb.AppendText(text);
                    }
                }

            }*/

            rtb.LoadFile(dt.DocPath);

            TextForm.Controls.Add(rtb);
            return dt;
        }

        /// <summary>
        /// Menu -> Artikel Opmaachen click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArtikelOpmaachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TextForm.Visible) { TextForm.Hide(); }
            if (folderBrowser.ShowDialog() == DialogResult.OK) { ArticleWorker();  }
        }

        /// <summary>
        /// Loads the .words File and fills (if .wordslists exists) into the software
        /// </summary>
        /// <param name="DirectLoadPath"></param>
        private async void ArticleWorker(String DirectLoadPath = null)
        {
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
                int c = 0;

                DataHandler dtt = new DataHandler
                {
                    CustomColor = dt.CustomColor,
                    Filename = dt.Filename,
                    Filepath = dt.Filepath,
                    Globrgb = dt.Globrgb,
                    DocPath = dt.DocPath
                };

                foreach (string line in lines)
                {
                    if (c == 0) { dt.SetRGB(line); }
                    else if (line != "")
                    {
                        //Find Occurence line in dt.Wordlist
                       var tmp = dt.WordList.FirstOrDefault(occ => occ.Occurence == line);
                       if (tmp != null)
                        {
                            dtt.WordList.Add(tmp);
                        } else
                        {
                            // New Word here
                            Console.WriteLine("New Word: " + line);
                            AutoComplete acword = await Task.Run(async () => await GetFullTranslationsAsync(line, false));
                            if (acword != null)
                            {
                                dtt.AddWordToList(acword);
                                acword.internalId = c;
                            }
                        }
                    }
                    c++;
                }  

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
                btnCustomAudio.Enabled = true;
                btnPlayAudio.Enabled = true;
                tsmiText.Enabled = true;
            }

            SetIsSaved(true);

            string lastFolderName = Path.GetFileName(Path.GetDirectoryName(folderBrowser.SelectedPath));
            tssArticle.Text = lastFolderName;
            tssMagazine.Text = globaldt.TargetMag();
        }

        private void LbWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ✓ 
            if (lbWords.SelectedIndex == -1)
                return;
            var _i = 0;
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
                btnCustomAudio.Enabled = false;
                btnPlayAudio.Enabled = false;
            }
            else
            {
                btnCustomAudio.Enabled = true;
                btnPlayAudio.Enabled = true;
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
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.EN);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "PT: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.PT);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "MP3: ");
            rtbDetails.SelectionFont = Normal;
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
            }
            // gets mp3 for new selected meaning
            globaldt.GetMp3(globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3, globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].hasCustomAudio);
        }

        private void MagazineSelectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show MagazineSelector Form
            if (globaldt == null)
            {
                globaldt = new DataHandler();
            }
            globaldt.ShowMagazineSelector();
            tssMagazine.Text = globaldt.TargetMag();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //Save
            globaldt.SaveToFile(globaldt);
            SetIsSaved(true);
            //tsmiSave.Enabled = false;
        }

        private void BtnCreatePopups_Click(object sender, EventArgs e)
        {
            //CreatePopups
            if (globaldt.OutputPopups())
            {
                MessageBox.Show(this, "Popups sinn erstallt!", "Okay", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            globaldt.HasPopups(true);
            btnCopyToMag.Enabled = true;
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
                globaldt.WordList[lbWords.SelectedIndex].Selection = lbSelectWord.SelectedIndex + 1;
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection = 1;
                lbSelectMeaning.SelectedIndex = 0;
                // save
                globaldt.SaveToFile(globaldt);
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
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection = lbSelectMeaning.SelectedIndex + 1;
                // if EN & PT are empty (no meaning on LOD (coming soon))
                if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].EN == "")
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
                    }
                    ENid.Dispose();
                    LbSelectMeaning_SelectedIndexChanged(sender, e);
                }

                if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1].PT == "")
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
                    }
                    PTid.Dispose();
                    LbSelectMeaning_SelectedIndexChanged(sender, e);
                }

                // save
                globaldt.SaveToFile(globaldt);
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
            btnCreatePopups.Enabled = false;
            btnCopyToMag.Enabled = false;
            btnCustomAudio.Enabled = false;
            btnPlayAudio.Enabled = false;
            tsmiSave.Enabled = false;
            tsmiText.Enabled = false;
            lbSelectMeaning.Items.Clear();
            lbSelectWord.Items.Clear();
            lbWords.Items.Clear();
            rtbDetails.Clear();
            TextForm.Close();
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
            string mp3Path = globaldt.Filepath + "WebResources\\popupbase-web-resources\\audio\\";
            string mp3File = mp3Path + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3;
            wplayer = new WindowsMediaPlayer();
            wplayer.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
            wplayer.URL = mp3File;
            btnPlayAudio.Enabled = false;
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
                btnPlayAudio.Enabled = true;
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
            File.Delete(globaldt.Filepath + "WebResources\\popupbase-web-resources\\audio\\" + globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3);
            if (CustomAudioBrowser.ShowDialog() == DialogResult.OK)
            {
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3 = Path.GetFileName(CustomAudioBrowser.FileName);
                globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].hasCustomAudio = true;
                globaldt.GetMp3(globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].MP3, globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex].hasCustomAudio);

                LbSelectMeaning_SelectedIndexChanged(sender, e);
            }
        }

        private void textUweisenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsmiText.Checked)
            {
                TextForm.Close();
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
    }
}