using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        public System.Collections.Generic.Dictionary<string, classes.WordBase> _WordBase;

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
            _WordBase = new Dictionary<string, classes.WordBase>();
        }

        public Article(string articlePath, string articleId)
        {
            RtfPath = new DirectoryInfo(articlePath).FullName + @"\" + articleId + @"_Text\Text.rtf";
            _Words = new System.Collections.Generic.Dictionary<string, classes.WordOverview>();
            _WordBase = new Dictionary<string, classes.WordBase>();
        }

        public async Task fillWordBaseDetails(WordBase wb)
        {
            classes.WordBase wbout = new classes.WordBase();
            if (_WordBase.TryGetValue(wb.baseWordXml, out wbout) != false )
            {
                Task<(List<Meaning>, classes.WordForm_Overload)> tm = Implementation.LODConnector.FetchWordDetails(wb.baseWordXml);
                (wb.meanings, wb.wordForm) = await tm;
            }
        }
    }
}