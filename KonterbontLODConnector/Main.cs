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

            SetResultText(responseBody);
        }

        private void btnFetch_ClickAsync(object sender, EventArgs e)
        {
            rtbResult.Clear();
            Task.WaitAll(Task.Run(async () => await FetchWords(edtXML.Text)));
            rtbResult.Text = theResponse;
        }


        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        private void SetResultText(String txt)
        {
            //if (ControlInvokeRequired(rtbResult, () => SetResultText(txt))) return;
            theResponse = txt;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            Wuert theWuert = new Wuert();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(rtbResult.Text);
      

            theWuert.LU = doc.DocumentNode.SelectNodes("//span[@class='adress mentioun_adress']").First().InnerText;
            Console.WriteLine("LU: "+theWuert.LU);

            theWuert.Wordform = doc.DocumentNode.SelectNodes("//span[@class='klass']").First().InnerText.Trim();
            Console.WriteLine("Wordform: "+theWuert.Wordform);

            var meanings = new List<string>();

            if (doc.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                foreach (var node in doc.DocumentNode.SelectNodes("//div[@class='uds_block']"))
                {
                    rtbTest.Text = node.InnerHtml;
                    meanings.Add(node.InnerHtml);


                    // Display meanings in CBox
                }
            }
            label1.Text = theWuert.LU+theWuert.Wordform;
        }
    }
}
