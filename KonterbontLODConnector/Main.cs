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

namespace WindowsFormsApp1
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https:///sony/IRCC"),
                Method = HttpMethod.Get,
                Headers =
                        {
                            { HttpRequestHeader.ContentType.ToString(), "text/xml; charset=\"utf-8\";" },
                            { "Referer", "1337" }
                        },
                Content = new StringContent("<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body><u:X_SendIRCC xmlns:u=\"urn:schemas-sony-com:service:IRCC:1\"><IRCCCode>\"" + actionarr[3] + "\"</IRCCCode></u:X_SendIRCC></s:Body></s:Envelope> ", Encoding.UTF8, "text/xml")
            };

            HttpResponseMessage _response = await httpClient.SendAsync(httpContent);
        }
    }
}
