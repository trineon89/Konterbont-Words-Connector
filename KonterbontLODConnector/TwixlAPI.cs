using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    public class TwixlAPI
    {
        private string KB_API_KEY = "1fe3c54f57f1abe4e1851dafbf5b1dd4";
        private string KB_APP_KEY_SANDBOX = "6f257e03bf92e48472a6a01bc80defec";

        public TwixlAPIAppData _twixlAPIAppData;
        public TwixlCategories _twixlCategories;

        private static readonly HttpClient client = new HttpClient();

        public TwixlAPI()
        {
            this.Init();
        }

        public void Init()
        {
            this.getAppData();
            this.getCategories();
        }

        public bool IsInCategory(int _issueid, int _catid)
        {
            if (this._twixlCategories.IsIssueInCategory(_issueid, _catid)) return true;
            else return false;
        }

        private TwixlAPIJsonCategorie[] GetAllCategories()
        {
            var client = new RestClient("https://platform.twixlmedia.com/admin-api/1/categories");
            var request = new RestRequest("all", Method.POST);
            request.AddParameter("admin_api_key", KB_API_KEY);
            request.AddParameter("app_key", KB_APP_KEY_SANDBOX);

            IRestResponse resp = client.Execute(request);
            JsonTextReader _reader = new JsonTextReader(new StringReader(resp.Content));
            JsonSerializer serializer = new JsonSerializer();
            TwixlAPIJsonCategorie[] res = serializer.Deserialize<TwixlAPIJsonCategorie[]>(_reader);

            return res;
        }

        public async Task<TwixlAPIJsonResponse> uploadIssue(string filepath)
        {
            filepath = ZipContent(filepath);

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var client = new RestClient("https://platform.twixlmedia.com/admin-api/1");
            var request = new RestRequest("upload", Method.POST);

            request.AddParameter("admin_api_key", KB_API_KEY);
            request.AddParameter("app_key", KB_APP_KEY_SANDBOX);
            string IssueIdent = SanitizeIdentifier(Path.GetFileNameWithoutExtension(filepath));
            IssueIdent = "com.konterbont.sandboxmagazine." + IssueIdent;
            request.AddParameter("issue_identifier", IssueIdent);
            request.AddParameter("issue_publish_on", unixTimestamp);

            request.AddFile("issue_file", filepath);

            IRestResponse resp = client.Execute(request);
            JsonTextReader _reader = new JsonTextReader(new StringReader(resp.Content));
            JsonSerializer serializer = new JsonSerializer();
            TwixlAPIJsonResponse res = serializer.Deserialize<TwixlAPIJsonResponse>(_reader);

            return res;
        }

        public void pushIssueToCategory(int issueId, int[] selectedCats)
        {
            foreach (int a in selectedCats)
            {
                _twixlCategories.AddIssueToCategory(issueId, _twixlCategories.categories[a].id);

                var client = new RestClient("https://platform.twixlmedia.com/admin-api/1/categories");
                var request = new RestRequest("update", Method.POST);
                request.AddParameter("admin_api_key", KB_API_KEY);
                request.AddParameter("app_key", KB_APP_KEY_SANDBOX);
                request.AddParameter("category_id", _twixlCategories.categories[a].id);
                request.AddParameter("category_name", _twixlCategories.categories[a].name);
                //request.AddParameter("category_issue_ids", _twixlCategories.categories[a].issue_ids.Cast<int>().ToArray());
                //request.AddParameter("category_issue_ids", Newtonsoft.Json.JsonConvert.SerializeObject( _twixlCategories.categories[a].issue_ids.ToArray<int>()));

                var arr = _twixlCategories.categories[a].issue_ids.ToArray<int>();
                string issue_ids = string.Join(",", arr);
                request.AddParameter("category_issue_ids", issue_ids);

                IRestResponse resp = client.Execute(request);

                JsonTextReader _reader = new JsonTextReader(new StringReader(resp.Content));
                JsonSerializer serializer = new JsonSerializer();
                TwixlAPIJsonResponse res = serializer.Deserialize<TwixlAPIJsonResponse>(_reader);

                var l = res;
            }

        }

        private string SanitizeIdentifier(string inp)
        {
            string _out = "";
            _out = inp.ToLower();
            _out = _out.Replace(" ", "_");
            _out = _out.Replace(".article", "");
            _out = Regex.Replace(_out, "[^a-z0-9_]+", "", RegexOptions.Compiled);
            return _out;
        }

        private void getCategories()
        {
            TwixlCategories _categories = new TwixlCategories(GetAllCategories());

            foreach (TwixlAPIJsonIssue _issue in _twixlAPIAppData.issues)
            {
                foreach (TwixlAPIJsonCategorie _cat in _issue.categories)
                {
                    _categories.AddCategory(_cat.name, _cat.id);
                    _categories.AddIssueToCategory(_issue.id, _cat.id);
                }
                
            }
            _twixlCategories = _categories;
        }

        private async Task<TwixlAPIAppData> getAppDataAsync()
        {
            var client = new RestClient("https://platform.twixlmedia.com/admin-api/1");
            var request = new RestRequest("app", Method.POST);
            request.AddParameter("admin_api_key", KB_API_KEY);
            request.AddParameter("app_key", KB_APP_KEY_SANDBOX);

            IRestResponse resp = client.Execute(request);
            JsonTextReader _reader = new JsonTextReader(new StringReader(resp.Content));
            JsonSerializer serializer = new JsonSerializer();
            TwixlAPIAppData res = serializer.Deserialize<TwixlAPIAppData>(_reader);

            return res;
        }

       /* public async Task<TwixlAPIJsonIssue[]> getIssues()
        {
            TwixlAPIAppData tmp = getAppData();
            return tmp.issues;
        }*/

        private string ZipContent(string filepath)
        {
            string newfilename = Path.GetFullPath(filepath) + ".zip";
            if (File.Exists(newfilename)) File.Delete(newfilename);
            ZipFile.CreateFromDirectory(filepath, newfilename, CompressionLevel.Optimal, true);
            return newfilename;
        }

        public void getAppData()
        {
            var appdata = getAppDataAsync();
            appdata.Wait();
            _twixlAPIAppData=appdata.Result;
        }

    }

    public class TwixlAPIRequest
    {
        public string admin_api_key { get; set; }
        public string app_key { get; set; }
        public string issue_identifier { get; set; }
        public string issue_file; //Should be a file, can't declare static <File>
    }

    public partial class TwixlAPIJsonResponse
    {
        public string result;
        public TwixlAPIJsonIssue issue;
        public TwixlAPIJsonIssue[] issues;
        public string error;
    }
  
    public class TwixlAPIJsonCategorie
    {
        public int id;
        public string name;
        public int sort_order;
        public List<TwixlAPIJsonIssue> issues;
    }

    public class TwixlAPIJsonIssue
    {
        public int id;
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
        public TwixlAPIJsonCategorie[] categories;
    }

    public class TwixlAPIAppData
    {
        public int id;
        public string mode;
        public string name;
        public string[] issue_identifiers;
        public TwixlAPIJsonIssue[] issues;
    }

    public class TwixlCategories
    {
        public List<TwixlCategory> categories;

        public TwixlCategories()
        {
            this.InitCategories();
        }

        public TwixlCategories(TwixlAPIJsonCategorie[] _twixlAPIJsonCategories)
        {
            this.InitCategories();
            foreach (TwixlAPIJsonCategorie categorie in _twixlAPIJsonCategories)
            {
                this.AddCategory(categorie.name, categorie.id);
            }
        }

        public void AddCategory(string _name, int _id)
        {
            if (!this.HasCategory(_id))
            { this.categories.Add(new TwixlCategory() { name = _name, id = _id }); }
        }

        private bool HasCategory(int _id)
        {
            if (this.categories.FirstOrDefault(x => x.id == _id) != null) return true;
            else return false;
        }

        public void AddIssueToCategory(int _issueid, int _catid)
        {
            if (!this.IsIssueInCategory(_issueid, _catid))
            { this.categories.First(cat => cat.id == _catid).issue_ids.Add(_issueid); }
           
        }

        public bool IsIssueInCategory(int _issueid, int _catid)
        {
            try
            {
                if (this.categories.FirstOrDefault(cat => cat.id == _catid && cat.issue_ids.FirstOrDefault(issue => issue == _issueid) > 0) != null)
                { return true; }
                else return false;
            } catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void InitCategories()
        {
            this.categories = new List<TwixlCategory>();

        }
    }

    public class TwixlCategory
    {
        public string name;
        public int id;
        public List<int> issue_ids;

        public TwixlCategory()
        {
            this.issue_ids = new List<int>();
        }
    }
}
