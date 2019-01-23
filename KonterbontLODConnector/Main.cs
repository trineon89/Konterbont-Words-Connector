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
                MP3 = "";
            }
        } 

        public Main()
        {
            InitializeComponent();
        }

        private async Task FetchWords(string XML)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/getart.php?artid=" + XML + ".xml"),
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

            SetResultText(responseBody,"LU");
        }

        private async Task FetchWordsDE(string XML)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/getartde.php?artid=" + XML + ".xml"),
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

            SetResultText(responseBody,"DE");
        }

        private async Task FetchWordsFR(string XML)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/getartfr.php?artid=" + XML + ".xml"),
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

            SetResultText(responseBody,"FR");
        }

        private async Task FetchWordsEN(string XML)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/getarten.php?artid=" + XML + ".xml"),
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

            SetResultText(responseBody,"EN");
        }

        private async Task FetchWordsPT(string XML)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/getartpt.php?artid=" + XML + ".xml"),
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

            SetResultText(responseBody,"PT");
        }

        private void btnFetch_ClickAsync(object sender, EventArgs e)
        {
            rtbResult.Clear();
            Task.WaitAll(Task.Run(async () => await FetchWords(edtXML.Text)));
            Task.WaitAll(Task.Run(async () => await FetchWordsDE(edtXML.Text)));
            Task.WaitAll(Task.Run(async () => await FetchWordsFR(edtXML.Text)));
            Task.WaitAll(Task.Run(async () => await FetchWordsEN(edtXML.Text)));
            Task.WaitAll(Task.Run(async () => await FetchWordsPT(edtXML.Text)));
            rtbResult.Text = theResponse;
        }


        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        private void SetResultText(String txt,String lang)
        {
            //if (ControlInvokeRequired(rtbResult, () => SetResultText(txt))) return;
            switch (lang)
            {
                case "LU": theResponse = txt; break;
                case "DE": theResponseDE = txt; break;
                case "FR": theResponseFR = txt; break;
                case "EN": theResponseEN = txt; break;
                case "PT": theResponsePT = txt; break;
            }
           
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            Wuert theWuert = new Wuert();
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
            Console.WriteLine("LU: "+theWuert.LU);

            theWuert.Wordform = docLU.DocumentNode.SelectNodes("//span[@class='klass']").First().InnerText.Trim();
            Console.WriteLine("Wordform: "+theWuert.Wordform);


            if (docDE.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                cbMeanings.Items.Clear();
                int _i = 1;

                frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();
                HtmlNodeCollection Meanings = docDE.DocumentNode.SelectNodes("//div[@class='uds_block']");
                foreach (HtmlNode Meaning in Meanings)
                {
                    rtbTest.Text = Meaning.InnerHtml;

                    string MeaningNr = Meaning.SelectSingleNode("span[@class='uds_num']").InnerText;
                    string MeaningText = Meaning.SelectSingleNode("span[@class='et']").InnerText;
                    string MeaningTextAdd = Meaning.SelectSingleNode("span[@class='text_gen']").InnerText;
                    Console.WriteLine(MeaningText + " " + MeaningTextAdd);

                    RadioButton rb = new RadioButton();
                    rb.Name = _i.ToString();
                    rb.Text = MeaningNr+" "+ MeaningText + " "+MeaningTextAdd;
                    rb.Location = new Point(10, _i * 30);
                    rb.Width = 500;
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);

                    cbMeanings.Items.Add(MeaningNr + " " + MeaningText + " " + MeaningTextAdd);
                    _i++;
                }
                

                if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                {
                    var selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    Console.WriteLine(selectedMeaning.Name);
                    

                    // Add selection to class!
                }
            }
            label1.Text = theWuert.LU+theWuert.Wordform;
        }
    }
}
