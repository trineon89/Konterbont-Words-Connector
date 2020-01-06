using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class Meaning
    {
        protected string LU;
        protected string LUs;
        protected string DE;
        protected string FR;
        protected string EN;
        protected string PT;

        public List<Example> examples;

        public enunciation enunciation;

        public Meaning()
        {
            LU = null;
            LUs = null;
            DE = null;
            FR = null;
            EN = null;
            PT = null;
            examples = new List<Example>();
            enunciation = enunciation.none;
        }
    }
}
