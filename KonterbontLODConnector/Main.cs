using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using HtmlAgilityPack;

namespace KonterbontLODConnector
{
    public partial class Main : Form
    {
        String theResponse = null;
        String theResponseDE = null;
        String theResponseFR = null;
        String theResponseEN = null;
        String theResponsePT = null;

        String theResponseTT = null;
        String theResponseTTDE = null;
        String theResponseTTFR = null;

        String theXMLResponse = null;
        String SearchXML = null;

        String SearchXMLTT = null;
        string MeaningsTT = "";

        Meaning theWuert = new Meaning();


        AutoComplete TheResults = new AutoComplete();

        public class AutoComplete
        {
            public List<Wuert> Wierder;
            public int Selection;

            public AutoComplete()
            {
                Wierder = new List<Wuert>();
            }
            
        }

        public class Wuert
        {
            public string WuertLu;
            public WordForm WuertForm;
            public List<Meaning> Meanings;
            public int Selection;
            public string MP3;

            public Wuert()
            {
                this.Meanings = new List<Meaning>();
            }
        }

        public partial class WordForm
        {
            public string WuertForm;

            public WordForm() { this.WuertForm = null; }
            public WordForm(string Word)
            {
                this.WuertForm = Word;
            }
        }

        public partial class Example
        {
            public string ExampleText;
            public Example()
            {
                this.ExampleText = null;
            }
            public Example(string ExText)
            {
                this.ExampleText = ExText;
            }
        }

        public class Meaning
        {
            public string LU;
            public string LUs;
            public string Wordform;
            public string DE;
            public string FR;
            public string EN;
            public string PT;
            public Example Example;
            public string MP3;

            public Meaning() // Constructor
            {
                LU = "";
                LUs = "";
                Wordform = "";
                DE = "";
                FR = "";
                EN = "";
                PT = "";
                Example = new Example();
            }
        }

        public Main()
        {
            InitializeComponent();
        }


        /* Print Class https://stackoverflow.com/questions/19823726/c-sharp-how-to-output-all-the-items-in-a-classstruct  */
        private void PrintProperties(Meaning myObj)
        {
            foreach (var prop in myObj.GetType().GetProperties())
            {
                Console.WriteLine(prop.Name + ": " + prop.GetValue(myObj, null));
            }

            foreach (var field in myObj.GetType().GetFields())
            {
                Console.WriteLine(field.Name + ": " + field.GetValue(myObj));
            }
        }
        /* Print Class End */


        private void GetWordLang(HtmlNodeCollection Meanings, string Lang, string Selection, Meaning theWuert)
        {
            int _j = 1;
            foreach (HtmlNode Meaning in Meanings)
            {
                string MeaningText = "";
                string MeaningTextAdd = "";
                string MeaningLUs = "";
                HtmlNode[] MeaningArray;
                if (theWuert.LUs == theWuert.LU)
                {
                    //Fetch Plural from mention
                    if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                    {
                        MeaningLUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
                    }
                }
                if (theWuert.Wordform != "Eegennumm")
                {
                    if (Meaning.SelectSingleNode("span[@class='text_gen']") != null)
                    {
                        if (Meaning.SelectSingleNode("span[@class='text_gen']").InnerText.Contains("["))
                        {
                            MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = Meaning.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            MeaningArray = Meaning.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1)
                                {
                                    MeaningText = MeaningText + ", ";
                                }

                            }
                        }
                    }
                }
                else
                {
                    MeaningText = Meaning.SelectSingleNode(".//span[@class='et']").InnerText;
                }

                if (Selection == _j.ToString())
                {
                    switch (Lang)
                    {
                        case "DE":
                            theWuert.DE = MeaningText + "" + MeaningTextAdd;
                            break;
                        case "FR":
                            theWuert.FR = MeaningText + "" + MeaningTextAdd;
                            break;
                        case "EN":
                            theWuert.EN = MeaningText + "" + MeaningTextAdd;
                            break;
                        case "PT":
                            theWuert.PT = MeaningText + "" + MeaningTextAdd;
                            break;
                    }
                    //TheResults.Wierder[_j].Meanings[_j].Example = new Example(Example);
                    theWuert.Example = new Example(Meaning.SelectSingleNode(".//span[@class='beispill']").InnerText); // Fetch 1st Example
                    //TheResults.Wierder[_j].Meanings[_j].LUs = MeaningLUs;
                    theWuert.LUs = MeaningLUs;
                }
                _j++;
            }
            Console.WriteLine(theWuert.Example.ExampleText);
        }

        private async Task FetchXML(string Word)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/lod-search.php?v=H&s=lu&w=" + Word),
                Method = HttpMethod.Get,
                Headers =
                           {
                              { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                              { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                           }
            };
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            theXMLResponse = responseBody;

        }

        private async Task FetchWords(string XML, string Lang)
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

            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            switch (Lang)
            {
                case "LU":
                    theResponse = responseBody;
                    break;
                case "DE":
                    theResponseDE = responseBody;
                    break;
                case "FR":
                    theResponseFR = responseBody;
                    break;
                case "EN":
                    theResponseEN = responseBody;
                    break;
                case "PT":
                    theResponsePT = responseBody;
                    break;
            }
        }

        private async Task FetchWordsTT(string XML, string Lang)
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

            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            switch (Lang)
            {
                case "LU":
                    theResponseTT = responseBody;
                    break;
                case "DE":
                    theResponseTTDE = responseBody;
                    break;
                case "FR":
                    theResponseTTFR = responseBody;
                    break;
            }
        }

        private void btnFetch_ClickAsync(object sender, EventArgs e)
        {
            Task.WaitAll(Task.Run(async () => await FetchXML(edtWord.Text)));
            GetXML();
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "LU")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "DE")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "FR")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "EN")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "PT")));

            GetMeanings(TheResults.Selection);
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

        private void GetMeaningsTT()
        {
            MeaningsTT = "";
            var docLU = new HtmlAgilityPack.HtmlDocument();
            var docDE = new HtmlAgilityPack.HtmlDocument();
            var docFR = new HtmlAgilityPack.HtmlDocument();

            docLU.LoadHtml(theResponseTT);
            docDE.LoadHtml(theResponseTTDE);
            docFR.LoadHtml(theResponseTTFR);

            theWuert.LU = docLU.DocumentNode.SelectNodes("//span[@class='adress mentioun_adress']").First().InnerText;
            theWuert.Wordform = docLU.DocumentNode.SelectNodes("//span[@class='klass']").First().InnerText.Trim();

            // for Meaning check
            HtmlNode[] LUsArray = docLU.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();
            if (theWuert.LU == theWuert.LUs || theWuert.LUs == "")
            {
                switch (theWuert.Wordform)
                {
                    case "Eegennumm":
                        theWuert.LUs = "";
                        break;
                    case "Adverb":
                        theWuert.LUs = "";
                        break;
                    case "Adjektiv":
                        theWuert.LUs = "";
                        break;
                    case "Pronomen":
                        theWuert.LUs = "";
                        break;
                    default:
                        theWuert.LUs = LUsArray[1].InnerText;
                        break;

                }
            }

            // Meanings START
            if (docDE.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                int _i = 1;
                string Selection = "";


                HtmlNodeCollection MeaningsDE = docDE.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsFR = docFR.DocumentNode.SelectNodes("//div[@class='uds_block']");

                int MeaningsCount = MeaningsDE.Count();

                foreach (HtmlNode Meaning in MeaningsDE)
                {
                    string MeaningNr = "";
                    string MeaningText = "";
                    string MeaningTextAdd = "";
                    HtmlNode[] MeaningArray;

                    if (Meaning.SelectSingleNode("span[@class='text_gen']") != null)
                    {
                        if (Meaning.SelectSingleNode("span[@class='text_gen']").InnerText.Contains("["))
                        {
                            if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                            {
                                MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                            }
                            MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = Meaning.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                            {
                                MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                            }
                            MeaningArray = Meaning.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1)
                                {
                                    MeaningText = MeaningText + ", ";
                                }

                            }
                        }
                    }
                    else
                    {
                        if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                        {
                            MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                        }
                        MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();

                    MeaningsTT = MeaningsTT + " " + MeaningNr + " " + MeaningText + "" + MeaningTextAdd;
                    _i++;
                }
            }
        }

        private void GetMeanings(int SelWord)
        {

            var docLU = new HtmlAgilityPack.HtmlDocument();
            var docDE = new HtmlAgilityPack.HtmlDocument();
            var docFR = new HtmlAgilityPack.HtmlDocument();
            var docEN = new HtmlAgilityPack.HtmlDocument();
            var docPT = new HtmlAgilityPack.HtmlDocument();

            docLU.LoadHtml(theResponse);
            docDE.LoadHtml(theResponseDE);
            docFR.LoadHtml(theResponseFR);
            docEN.LoadHtml(theResponseEN);
            docPT.LoadHtml(theResponsePT);

            TheResults.Wierder[SelWord].WuertLu = docLU.DocumentNode.SelectNodes("//span[@class='adress mentioun_adress']").First().InnerText;
            TheResults.Wierder[SelWord].WuertForm.WuertForm = docLU.DocumentNode.SelectNodes("//span[@class='klass']").First().InnerText.Trim();

            // Meanings START
            if (docDE.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                int _i = 1;
                string Selection = "";
                string MeaningNr = "";
                string MeaningText = "";
                string MeaningTextAdd = "";
                frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();
                HtmlNodeCollection MeaningsDE = docDE.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsFR = docFR.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsEN = docEN.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsPT = docPT.DocumentNode.SelectNodes("//div[@class='uds_block']");

                int MeaningsCount = MeaningsDE.Count();

                foreach (HtmlNode Meaning in MeaningsDE)
                {
                    HtmlNode[] MeaningArray;
                    Meaning tmpMeaning = new Meaning();
                    TheResults.Wierder[SelWord].Meanings.Add(tmpMeaning);

                    if (Meaning.SelectSingleNode("span[@class='text_gen']") != null)
                    {
                        if (Meaning.SelectSingleNode("span[@class='text_gen']").InnerText.Contains("["))
                        {
                            MeaningNr = _i.ToString() + ".";

                            TheResults.Wierder[SelWord].Meanings[_i-1].DE = Meaning.SelectSingleNode("span[@class='et']").InnerText + Meaning.SelectSingleNode("span[@class='text_gen']").InnerText;
                            MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = Meaning.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                            {
                                MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                            }
                            MeaningArray = Meaning.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1)
                                {
                                    MeaningText = MeaningText + ", ";
                                }

                            }
                        }
                    }
                    else
                    {
                        if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                        {
                            MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                        }
                        MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();
                    RadioButton rb = new RadioButton();
                    rb.Name = _i.ToString();
                    rb.Text = MeaningNr + " " + MeaningText + "" + MeaningTextAdd;
                    rb.Location = new Point(10, _i * 30);
                    rb.Width = 500;
                    if (_i == 1)
                    {
                        rb.Checked = true;
                    }
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);
                    _i++;

                    //TheResults.Wierder[SelWord].Meanings.Add(tmpMeaning);
                }
                if (MeaningsCount > 1)
                {
                    if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                    {
                        var selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                        Selection = selectedMeaning.Name;
                        TheResults.Wierder[TheResults.Selection].Selection = Int32.Parse(Selection);
                    }
                }

                GetWordLang(MeaningsDE, "DE", Selection, theWuert);
                GetWordLang(MeaningsFR, "FR", Selection, theWuert);
                GetWordLang(MeaningsEN, "EN", Selection, theWuert);
                GetWordLang(MeaningsPT, "PT", Selection, theWuert);
            }
            // Meanings END

            // Add LUs if not from Meaning
            HtmlNode[] LUsArray = docLU.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();
            if (theWuert.LU == theWuert.LUs || theWuert.LUs == "")
            {
                switch (theWuert.Wordform)
                {
                    case "Eegennumm":
                        theWuert.LUs = "";
                        break;
                    case "Adverb":
                        theWuert.LUs = "";
                        break;
                    case "Adjektiv":
                        theWuert.LUs = "";
                        break;
                    case "Pronomen":
                        theWuert.LUs = "";
                        break;
                    default:
                        theWuert.LUs = LUsArray[1].InnerText;
                        if (LUsArray[2].InnerText != theWuert.LU)
                        {
                            theWuert.LUs = theWuert.LUs + " / " + LUsArray[2].InnerText;
                        }
                        break;

                }
            }
            PrintProperties(theWuert); // Write class to Console
        }

        // ---------------------------------------------------- //
        // ---------------------------------------------------- //
        /*private void GetMeanings(int SelWord)
        {

            var docLU = new HtmlAgilityPack.HtmlDocument();
            var docDE = new HtmlAgilityPack.HtmlDocument();
            var docFR = new HtmlAgilityPack.HtmlDocument();
            var docEN = new HtmlAgilityPack.HtmlDocument();
            var docPT = new HtmlAgilityPack.HtmlDocument();

            docLU.LoadHtml(theResponse);
            docDE.LoadHtml(theResponseDE);
            docFR.LoadHtml(theResponseFR);
            docEN.LoadHtml(theResponseEN);
            docPT.LoadHtml(theResponsePT);

            theWuert.LU = docLU.DocumentNode.SelectNodes("//span[@class='adress mentioun_adress']").First().InnerText;
            theWuert.Wordform = docLU.DocumentNode.SelectNodes("//span[@class='klass']").First().InnerText.Trim();

            // for Meaning check
            HtmlNode[] LUsArray = docLU.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();
            if (theWuert.LU == theWuert.LUs || theWuert.LUs == "")
            {
                switch (theWuert.Wordform)
                {
                    case "Eegennumm":
                        theWuert.LUs = "";
                        break;
                    case "Adverb":
                        theWuert.LUs = "";
                        break;
                    case "Adjektiv":
                        theWuert.LUs = "";
                        break;
                    case "Pronomen":
                        theWuert.LUs = "";
                        break;
                    default:
                        theWuert.LUs = LUsArray[1].InnerText;
                        break;

                }
            }

            // Meanings START
            if (docDE.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                int _i = 1;
                string Selection = "";
                frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();
                HtmlNodeCollection MeaningsDE = docDE.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsFR = docFR.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsEN = docEN.DocumentNode.SelectNodes("//div[@class='uds_block']");
                HtmlNodeCollection MeaningsPT = docPT.DocumentNode.SelectNodes("//div[@class='uds_block']");

                int MeaningsCount = MeaningsDE.Count();

                foreach (HtmlNode Meaning in MeaningsDE)
                {
                    string MeaningNr = "";
                    string MeaningText = "";
                    string MeaningTextAdd = "";
                    HtmlNode[] MeaningArray;

                    if (Meaning.SelectSingleNode("span[@class='text_gen']") != null)
                    {
                        if (Meaning.SelectSingleNode("span[@class='text_gen']").InnerText.Contains("["))
                        {
                            if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                            {
                                MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                            }
                            MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = Meaning.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                            {
                                MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                            }
                            MeaningArray = Meaning.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1)
                                {
                                    MeaningText = MeaningText + ", ";
                                }

                            }
                        }
                    }
                    else
                    {
                        if (Meaning.SelectSingleNode("span[@class='uds_num']") != null)
                        {
                            MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                        }
                        MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();
                    RadioButton rb = new RadioButton();
                    rb.Name = _i.ToString();
                    rb.Text = MeaningNr + " " + MeaningText + "" + MeaningTextAdd;
                    rb.Location = new Point(10, _i * 30);
                    rb.Width = 500;
                    if (_i == 1)
                    {
                        rb.Checked = true;
                    }
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);
                    _i++;
                }
                if (MeaningsCount > 1)
                {
                    if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                    {
                        var selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                        Selection = selectedMeaning.Name;
                        TheResults.Wierder[TheResults.Selection].Selection = Int32.Parse(Selection);
                    }
                }

                GetWordLang(MeaningsDE, "DE", Selection, theWuert);
                GetWordLang(MeaningsFR, "FR", Selection, theWuert);
                GetWordLang(MeaningsEN, "EN", Selection, theWuert);
                GetWordLang(MeaningsPT, "PT", Selection, theWuert);
            }
            // Meanings END

            // Add LUs if not from Meaning
            LUsArray = docLU.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();
            if (theWuert.LU == theWuert.LUs || theWuert.LUs == "")
            {
                switch (theWuert.Wordform)
                {
                    case "Eegennumm":
                        theWuert.LUs = "";
                        break;
                    case "Adverb":
                        theWuert.LUs = "";
                        break;
                    case "Adjektiv":
                        theWuert.LUs = "";
                        break;
                    case "Pronomen":
                        theWuert.LUs = "";
                        break;
                    default:
                        theWuert.LUs = LUsArray[1].InnerText;
                        if (LUsArray[2].InnerText != theWuert.LU)
                        {
                            theWuert.LUs = theWuert.LUs + " / " + LUsArray[2].InnerText;
                        }
                        break;

                }
            }
            PrintProperties(theWuert); // Write class to Console
        }*/
        // ---------------------------------------------------- //
        // ---------------------------------------------------- //

        private void GetXML()
        {
            var docXML = new HtmlAgilityPack.HtmlDocument();
            string onClickVal = "";
            frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();
            HtmlNode[] XMLArray;
            int _i = 1;
            string Selection = "";

            docXML.LoadHtml(theXMLResponse);
            XMLArray = docXML.DocumentNode.SelectNodes("//a[@onclick]").ToArray();

            int XMLCount = XMLArray.Count();

            if (XMLCount > 1)
            {
                foreach (HtmlNode XML in XMLArray)
                {
                    Wuert tmpWuert = new Wuert
                    {
                        WuertLu = XML.InnerText,
                        WuertForm = new WordForm(docXML.DocumentNode.SelectSingleNode("//span[@class='s4']").InnerText),
                        Selection = 0
                    };

                    TheResults.Wierder.Add(tmpWuert);

                  //  GetXMLTT(XML);

                    RadioButton rb = new RadioButton();
                    rb.Name = _i.ToString();
                    rb.Text = tmpWuert.WuertLu + " (" + tmpWuert.WuertForm.WuertForm + ")";
                    rb.Location = new Point(10, _i * 30);
                    rb.Width = 500;
                    if (_i == 1)
                    {
                        rb.Checked = true;
                    }
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);
                    frmSelectMeaning.gbMeanings.Text = "Wuert auswielen:";
                    frmSelectMeaning.Text = "Wuert auswielen";
                    frmSelectMeaning.tpInfo.SetToolTip(rb, MeaningsTT);
                    _i++;
                }
                if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                {
                    var selectedXML = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    Selection = selectedXML.Name;
                    TheResults.Selection = Int32.Parse(Selection);
                    onClickVal = docXML.DocumentNode.SelectNodes("//a[@onclick]")[TheResults.Selection - 1].GetAttributeValue("onclick", "default"); //Bsp: getart('PERSOUN1.xml','persoun1.mp3')
                }

            }
            else
            {
                Wuert tmpWuert = new Wuert
                {
                    WuertLu = docXML.DocumentNode.SelectSingleNode("//a[@onclick]").InnerText,
                    WuertForm = new WordForm(docXML.DocumentNode.SelectSingleNode("//span[@class='s4']").InnerText),
                    Selection = 0
                };

                TheResults.Wierder.Add(tmpWuert);
                onClickVal = docXML.DocumentNode.SelectSingleNode("//a[@onclick]").GetAttributeValue("onclick", "default"); //Bsp: getart('PERSOUN1.xml','persoun1.mp3')
                TheResults.Selection = 0;
            }

            // get XML and MP3 from Word search
            var QuotePos = onClickVal.IndexOf("'");
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: PERSOUN1.xml','persoun1.mp3')

            QuotePos = onClickVal.IndexOf("'");
            SearchXML = onClickVal.Substring(0, QuotePos);   // result: PERSOUN1.xml
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: ,'persoun1.mp3')

            QuotePos = onClickVal.IndexOf("'");
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: persoun1.mp3')

            QuotePos = onClickVal.IndexOf("'");
            TheResults.Wierder[TheResults.Selection].MP3 = onClickVal.Substring(0, QuotePos); // result: persoun1.mp3
            theWuert.MP3 = onClickVal.Substring(0, QuotePos); // result: persoun1.mp3

        }

        private void GetXMLTT(HtmlNode XMLTT)
        {
            // string onClickVal = "";
            var a = new HtmlAgilityPack.HtmlDocument();

            a.LoadHtml(XMLTT.OuterHtml);

            string onClickVal = a.DocumentNode.SelectSingleNode("//a[@onclick]").GetAttributeValue("onclick", "default"); //Bsp: getart('PERSOUN1.xml','persoun1.mp3')

            // get XML and MP3 from Word search
            var QuotePos = onClickVal.IndexOf("'");
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: PERSOUN1.xml','persoun1.mp3')

            QuotePos = onClickVal.IndexOf("'");
            SearchXMLTT = onClickVal.Substring(0, QuotePos);   // result: PERSOUN1.xml

            Task.WaitAll(Task.Run(async () => await FetchWordsTT(SearchXMLTT, "LU")));
            Task.WaitAll(Task.Run(async () => await FetchWordsTT(SearchXMLTT, "DE")));
            Task.WaitAll(Task.Run(async () => await FetchWordsTT(SearchXMLTT, "FR")));
            GetMeaningsTT();
        }
    }
}
