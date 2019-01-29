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

        String theXMLResponse = null;
        String SearchXML = null;

        Wuert theWuert = new Wuert();


        public class Wuert
        {
            public string LU;
            public string LUs;
            public string Wordform;
            public string DE;
            public string FR;
            public string EN;
            public string PT;
            public string Example;
            public string MP3;

            public Wuert() // Constructor
            {
                LU = "";
                LUs = "";
                Wordform = "";
                DE = "";
                FR = "";
                EN = "";
                PT = "";
                Example = "";
            }
        }

        public Main()
        {
            InitializeComponent();
        }

        /* Print Class https://stackoverflow.com/questions/19823726/c-sharp-how-to-output-all-the-items-in-a-classstruct  */
        private void PrintProperties(Wuert myObj)
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

        private void GetWordLang(HtmlNodeCollection Meanings, string Lang, string Selection, Wuert theWuert)
        {
            int _j = 1;
            foreach (HtmlNode Meaning in Meanings)
            {
                string MeaningText = "";
                string MeaningTextAdd = "";
                string MeaningLUs = "";
                HtmlNode[] MeaningArray;
                string Example = Meaning.SelectSingleNode(".//span[@class='beispill']").InnerText; // Fetch 1st Example
                if (theWuert.LUs == theWuert.LU)
                {                 //Fetch Plural from mention
                    if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                    {
                        MeaningLUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
                    }
                }

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
                    theWuert.Example = Example;
                    theWuert.LUs = MeaningLUs;
                }
                _j++;
            }
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

        private void btnFetch_ClickAsync(object sender, EventArgs e)
        {
            rtbResult.Clear();
            Task.WaitAll(Task.Run(async () => await FetchXML(edtWord.Text)));
            rtbResult.Text = theXMLResponse;
            GetXML();
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "LU")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "DE")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "FR")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "EN")));
            Task.WaitAll(Task.Run(async () => await FetchWords(SearchXML, "PT")));

            GetMeanings();
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

        /*private void SetResultText(String txt, String lang)
        {
            //if (ControlInvokeRequired(rtbResult, () => SetResultText(txt))) return;
            switch (lang)
            {
                case "LU":
                    theResponse = txt;
                    break;
                case "DE":
                    theResponseDE = txt;
                    break;
                case "FR":
                    theResponseFR = txt;
                    break;
                case "EN":
                    theResponseEN = txt;
                    break;
                case "PT":
                    theResponsePT = txt;
                    break;
            }
        }*/

        // ---------------------------------------------------- //
        // ---------------------------------------------------- //
        private void GetMeanings()
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
                theWuert.LUs = LUsArray[1].InnerText;
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
                        MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                        MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();
                    RadioButton rb = new RadioButton();
                    rb.Name = _i.ToString();
                    rb.Text = MeaningNr + " " + MeaningText + "" + MeaningTextAdd;
                    rb.Location = new Point(10, _i * 30);
                    rb.Width = 500;
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);
                    _i++;
                }
                if (MeaningsCount > 1)
                {
                    if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                    {
                        var selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                        Selection = selectedMeaning.Name;
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
                theWuert.LUs = LUsArray[1].InnerText;
                if (LUsArray[2].InnerText != theWuert.LU)
                {
                    theWuert.LUs = theWuert.LUs + " / " + LUsArray[2].InnerText;
                }
            }

            PrintProperties(theWuert); // Write class to Console
        }
        // ---------------------------------------------------- //
        // ---------------------------------------------------- //


        private void GetXML()
        {
            var docXML = new HtmlAgilityPack.HtmlDocument();
            string onClickVal = "";

            docXML.LoadHtml(theXMLResponse);

            rtbTest.Text = docXML.DocumentNode.SelectSingleNode("//a[@onclick]").GetAttributeValue("onclick", "default");
            onClickVal = docXML.DocumentNode.SelectSingleNode("//a[@onclick]").GetAttributeValue("onclick", "default"); //Bsp: getart('PERSOUN1.xml','persoun1.mp3')

            // get XML and MP3 from Word search
            var QuotePos = onClickVal.IndexOf("'");
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: PERSOUN1.xml','persoun1.mp3')
            QuotePos = onClickVal.IndexOf("'");
            SearchXML = onClickVal.Substring(0, QuotePos);
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: ,'persoun1.mp3')
            QuotePos = onClickVal.IndexOf("'");
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: persoun1.mp3')
            QuotePos = onClickVal.IndexOf("'");
            theWuert.MP3 = onClickVal.Substring(0, QuotePos);
        }
    }
}
