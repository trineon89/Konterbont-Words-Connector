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

namespace WindowsFormsApp1
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
    }
}
