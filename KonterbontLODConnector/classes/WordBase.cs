using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class WordBase
    {
        public string baseWordLu;
        public string baseWordXml;
        public WordForm wordForm;
        public string baseMp3;
        public List<Meaning> meanings;

        public WordBase()
        {

        }

        public WordBase(string baseWord)
        {
            baseWordXml = baseWord;
        }

    }
}
