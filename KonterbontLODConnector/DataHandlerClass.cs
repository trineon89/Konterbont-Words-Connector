using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    class DataHandler
    {
        public string Filename;
        public string Filepath;
        public string QuickSelectFile;
        //public Dictionary<short, string> QuickSelect = new Dictionary<short, string>();
        public List<AutoComplete> WordList = new List<AutoComplete>();

        public DataHandler()
        {
            Filename = null;
            Filepath = null;
            QuickSelectFile = null;
        }

        public DataHandler(string _filename)
        {
            Filename = _filename;
            Filepath = null;
            QuickSelectFile = null;
        }

        public DataHandler(string _filename, string _filepath)
        {
            Filename = _filename;
            Filepath = _filepath;
            QuickSelectFile = null;
        }

        public void AddWordToList(AutoComplete ac)
        {
            WordList.Add(ac);
        }

    }
}
