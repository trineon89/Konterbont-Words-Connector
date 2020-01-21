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
        public string MP3;

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
            MP3 = null;
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
                case "MP3": MP3 = value; break;
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
                case "MP3": return MP3;
                default: return null;
            }
        }

        public Meaning CopyOver(Meaning _orig)
        {
            Meaning _res = new Meaning();

            _res.DE = _orig.DE;
            _res.EN = _orig.EN;
            _res.examples = _orig.examples;
            _res.examples_Extended = _orig.examples_Extended;
            _res.FR = _orig.FR;
            _res.LU = _orig.LU;
            _res.LUs = _orig.LUs;
            _res.MP3 = _orig.MP3;
            _res.NoPlural = _orig.NoPlural;
            _res.PT = _orig.PT;

            return _res;
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
