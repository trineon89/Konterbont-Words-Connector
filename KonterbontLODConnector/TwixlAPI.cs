using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    class TwixlAPI
    {
        private string KB_API_KEY = "1fe3c54f57f1abe4e1851dafbf5b1dd4";
        private string KB_APP_KEY_SANDBOX = "6f257e03bf92e48472a6a01bc80defec";

        private static readonly HttpClient client = new HttpClient();

        public TwixlAPI()
        {
            this.Init();
        }

        public void Init()
        {

        }

        public async Task<string> GetIssueAsync()
        {
            var values = new Dictionary<string, string>
            {
                { "admin_api_key", KB_API_KEY },
                { "app_key", KB_APP_KEY_SANDBOX },
                { "id", "54767" }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://platform.twixlmedia.com/admin-api/1/issue", content);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        public async Task<TwixlAPIJsonResponse> uploadIssue(string filepath)
        {
            filepath = ZipContent(filepath);

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var client = new RestClient("https://platform.twixlmedia.com/admin-api/1");
            var request = new RestRequest("upload", Method.POST);

            request.AddParameter("admin_api_key", KB_API_KEY);
            request.AddParameter("app_key", KB_APP_KEY_SANDBOX);
            request.AddParameter("issue_identifier", 12345);
            request.AddParameter("issue_publish_on", unixTimestamp);

            request.AddFile("issue_file", filepath);

            IRestResponse<TwixlAPIJsonResponse> resp = client.Execute<TwixlAPIJsonResponse>(request);

            return resp.Data;
        }

        private string ZipContent(string filepath)
        {
            string newfilename = Path.GetFullPath(filepath) + ".zip";
            if (File.Exists(newfilename)) File.Delete(newfilename);
            ZipFile.CreateFromDirectory(filepath, newfilename, CompressionLevel.Optimal, true);
            return newfilename;
        }
    }

    class TwixlAPIRequest
    {
        public string admin_api_key { get; set; }
        public string app_key { get; set; }
        public string issue_identifier { get; set; }
        public string issue_file; //Should be a file, can't declare static <File>
    }

    class TwixlAPIJsonResponse
    {
        public string result;
        public TwixlAPIJsonIssue issue;
        public string error;
    }

    class TwixlAPIJsonIssue
    {
        public int content_size_android10;
        public int content_size_android7;
        public int content_size_ipad;
        public int content_size_ipad_retina;
        public int content_size_pdf;
        public bool default_issue;
        public string file_type;
        public bool is_in_subscription;
        public string issue_type;
        public string name;
        public string tagline;
        public string product_identifier;
        public int published_on; //Unix-Timestamp
        public string status;
        public string uuid;
        public string description;

    }
}
