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
        public Main()
        {
            InitializeComponent();
        }

        private async Task<HttpResponseMessage> FetchWords(string XML)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage


           {
               RequestUri = new Uri("https://www.lod.lu/php/getartde.php?artid="+XML+".xml"),
               Method = HttpMethod.Get,
               Headers =
                           {
                              // { HttpRequestHeader.ContentType.ToString(), "text/xml; charset=\"utf-8\";" },
                              { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                               { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                           }
           };

            //HttpResponseMessage _response =  await httpClient.SendAsync(httpContent);
            httpClient.Timeout = TimeSpan.FromMilliseconds(250);
            var _response = await httpClient.SendAsync(httpContent);
            return _response;
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            rtbResult.Text = FetchWords(edtXML.Text).Result.ToString();
        }
    }
}
