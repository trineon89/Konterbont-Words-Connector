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
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(rtbResult.Text);

            var navigator = (HtmlAgilityPack.HtmlNodeNavigator)doc.CreateNavigator();

            var xpath1 = "//span[@class='adress mentioun_adress']";
            //

            

            var result = navigator.SelectSingleNode(xpath1);
            Console.WriteLine(result.Value);
            label1.Text = result.Value;
            var xpath2 = "//span[@class='text_gen']";
            var text_gen = navigator.Select(xpath2);

            Console.WriteLine(doc.DocumentNode.SelectNodes("//span[@class='text_gen']").First().InnerText);
            Console.WriteLine(doc.DocumentNode.SelectNodes("//span[@class='text_gen']").ElementAt(1).InnerText);




            /*result = navigator.SelectSingleNode(xpath2);
            Console.WriteLine(result.Value);*/
        }
    }
}
