using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace KonterbontLODConnector
{
    [System.Serializable]
    public class ArticleFile
    {
        public string ArticlePath { get; }
        public string ArticleFileName { get; }
        public string ArticleId { get; }
        public string ArticleName { get; }

        public Article article;

        public ArticleFile()
        {
            this.ArticlePath = null;
        }

        public static ArticleFile LoadFromFile(string filepath)
        {
            ArticleFile ar = new ArticleFile(filepath);

            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader sr = new StreamReader(ar.ArticlePath + @"\" + ar.ArticleFileName))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                ar = serializer.Deserialize<ArticleFile>(reader);
            }

            return ar;
        }

        public void SaveToFile()
        {
            JsonSerializer serializer = new JsonSerializer();
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
    }
}