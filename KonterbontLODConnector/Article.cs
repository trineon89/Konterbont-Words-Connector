using System.IO;

namespace KonterbontLODConnector
{
    internal class Article
    {
        private string articlePath;
        private string rtfPath;

        public string ArticlePath { get => articlePath; set => articlePath = value; }
        public string RtfPath { get => rtfPath; set => rtfPath = value; }

        public Article(string _path)
        {
            articlePath = _path;
            string articleid = new DirectoryInfo(_path).Parent.Name.Substring(0,4);
            rtfPath = new DirectoryInfo(_path).Parent.Parent.FullName + @"\" + articleid + @"_Text\Text.rtf";
        }
    }
}