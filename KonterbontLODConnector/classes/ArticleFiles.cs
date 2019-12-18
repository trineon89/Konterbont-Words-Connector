using System.IO;

namespace KonterbontLODConnector
{
    public class ArticleFile
    {
        public string ArticlePath { get; }
        public string ArticleFileName { get; }
        public string ArticleId { get; }
        public string ArticleName { get; }

        public ArticleFile()
        {
            this.ArticlePath = null;
        }

        public ArticleFile(string articlePath)
        {
            ArticlePath = articlePath;
            string tmpstring = articlePath.Substring(articlePath.LastIndexOf("\\") + 1);
            ArticleId = tmpstring.Substring(0, 4);
            ArticleName = tmpstring.Substring(5, tmpstring.Length-5);
            ArticleFileName = ArticleId + @"_Artikel\" + ArticleId + ".article";
        }
    }
}