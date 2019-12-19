using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class Word
    {
        public string baseWordLu;
        public string occurence;
        public WordForm wordForm;

        public Word(string occ)
        {
            occurence = occ;
        }

    }

    
}
