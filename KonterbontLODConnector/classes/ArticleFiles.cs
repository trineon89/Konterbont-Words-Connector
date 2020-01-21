using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace KonterbontLODConnector.classes
{
    [System.Serializable]
    public class ArticleFile
    {
        public string ArticlePath { get; set; }
        public string ArticleFileName { get; set; }
        public string ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string Magazine { get; set; }
        public string MagazinePath { get; set; }
        public string globalrgb { get; set; }
        public Article article;

        public ArticleFile()
        {
            this.ArticlePath = null;
            Magazine = null;
            globalrgb = null;
        }

        public static ArticleFile LoadFromFile(string filepath)
        {
            ArticleFile ar = new ArticleFile(filepath);
            JsonSerializerSettings jss = new JsonSerializerSettings();
            //jss.TypeNameHandling = TypeNameHandling.All;
            JsonSerializer serializer =JsonSerializer.Create(jss);

            using (StreamReader sr = new StreamReader(ar.ArticlePath + @"\" + ar.ArticleFileName))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                ar = serializer.Deserialize<ArticleFile>(reader);
            }

            if (ar == null)
            {
                ar = new ArticleFile(new System.IO.DirectoryInfo(filepath).Parent.FullName);
                ar.SaveToFile();
            }


            if (ar.article._WordBase == null)
            {
                ar.article._WordBase = new System.Collections.Generic.Dictionary<string, WordBase>();
            }

            return ar;
        }

        private void CleanUp()
        {
            int i;
            foreach (WordOverview _w in article._Words.Values)
            {
                i = 1;
                if (_w._wordPossibleMeanings.Count == 1)
                {
                    _w.WordPointer = 1;
                } else
                {
                    int j = _w.WordPointer; // Values 0 :-> not set, 1, 2, 3 -> specific word

                    foreach (var _pm in _w._wordPossibleMeanings)
                    {
                        if (_pm.meaningPointer > 0)
                        {
                            if (j != i)
                            {
                                _pm.meaningPointer = 0;
                            }
                        }

                        i++;
                    }
                }
            }
                
        }

        public void SaveToFile()
        {
            CleanUp();
            JsonSerializerSettings jss = new JsonSerializerSettings();
            //jss.TypeNameHandling = TypeNameHandling.All;
            JsonSerializer serializer = JsonSerializer.Create(jss);
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(ArticlePath + @"\" + ArticleFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        public ArticleFile(string articlePath)
        {
            ArticlePath = articlePath;
            string tmpstring = articlePath.Substring(articlePath.LastIndexOf("\\") + 1);
            ArticleId = tmpstring.Substring(0, 4);
            ArticleName = tmpstring.Substring(5, tmpstring.Length-5);
            //ArticleFileName = ArticleId + @"_Artikel\" + ArticleId + ".article";
            ArticleFileName = ArticleId + ".article";
            article = new Article(ArticlePath, ArticleId);
        }

        public ArticleFile(string articlePath, bool isMag)
        {
            ArticlePath = articlePath;
            string tmpstring = articlePath.Substring(articlePath.LastIndexOf("\\") + 1);
            ArticleId = tmpstring;
            ArticleFileName = ArticleId;
        }
    }
}