using System.IO;

namespace KonterbontLODConnector.classes
{
    [System.Serializable]
    public class Article
    {
        public string RtfPath { get; set; }

        /// <summary>
        /// Key is occurence
        /// </summary>
        public System.Collections.Generic.Dictionary<string, classes.WordOverview> _Words;

        public Article() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_path">The ArticlePath is the .article file (located in the Root Folder of the articles)</param>
        public Article(string _path)
        {
            string articleid = new DirectoryInfo(_path).Parent.Name.Substring(0,4);
            RtfPath = new DirectoryInfo(_path).Parent.Parent.FullName + @"\" + articleid + @"_Text\Text.rtf";
            _Words = new System.Collections.Generic.Dictionary<string, classes.WordOverview>();
        }

        public Article(string articlePath, string articleId)
        {
            RtfPath = new DirectoryInfo(articlePath).FullName + @"\" + articleId + @"_Text\Text.rtf";
            _Words = new System.Collections.Generic.Dictionary<string, classes.WordOverview>();
        }
    }
}