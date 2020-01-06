using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class Word
    {
        public string occurence;
        public string wordBasePointer;
        public int meaningPointer;
        public Meaning customMeaning;

        public Word()
        {
            occurence = null;
        }

        public Word(string occ)
        {
            occurence = occ;
        }

        public Word(string occ, string wordBaseP)
        {
            occurence = occ;
            wordBasePointer = wordBaseP;
        }
    }

    
}
