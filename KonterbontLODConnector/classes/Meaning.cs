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
        public bool NoPlural = false;
        public string DE;
        public string FR;
        public string EN;
        public string PT;

        public List<Example> examples;
        public List<Example_Extended> examples_Extended;

        public Meaning()
        {
            LU = null;
            LUs = null;
            DE = null;
            FR = null;
            EN = null;
            PT = null;
        }

        public void SetValue(string selector, string value)
        {
            switch (selector)
            {
                case "LU": LU = value; break;
                case "DE": DE = value; break;
                case "LUs": LUs = value; break;
                case "FR": FR = value; break;
                case "EN": EN = value; break;
                case "PT": PT = value; break;
                default: return;
            }
        }

        public string GetValue(string selector)
        {
            switch (selector)
            {
                case "LU": return LU;
                case "DE": return DE;
                case "LUs": return LUs;
                case "FR": return FR;
                case "EN": return EN;
                case "PT": return PT;
                default: return null;
            }
        }



        public enunciation GetEnunciation(string enunText)
        {
            enunciation enun;
            switch (enunText)
            {
                case "ëmgangssproochlech": enun = enunciation.ëmgangssproochlech; break;
                default: enun = enunciation.none; break;
            }
            return enun;
        }

    }
}
