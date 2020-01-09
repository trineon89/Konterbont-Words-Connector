using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class Meaning
    {
        public string LU;
        public string LUs;
        public string DE;
        public string FR;
        public string EN;
        public string PT;

        public List<Example> examples;
        public List<Example_Extended> examples_Extended;

        public enunciation enunciation;

        public Meaning()
        {
            LU = null;
            //LUs = null;
            DE = null;
            FR = null;
            EN = null;
            PT = null;
            examples = new List<Example>();
            examples_Extended = new List<Example_Extended>();
            enunciation = enunciation.none;
        }

        public void SetValue(string selector, string value)
        {
            switch (selector)
            {
                case "LU": LU = value; break;
                case "DE": DE = value; break;
                //case "LUs": LUs = value; break;
                case "FR": FR = value; break;
                case "EN": EN = value; break;
                case "PT": PT = value; break;
                default: return;
            }
        }

    }
}
