using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class Example
    {
        public string exampleText { get; set; }
    }

    public class Example_Extended : Example
    {
        public string exampleText { get; set; }
        public enunciation enunciation { get; set; }
        public string enunciationText { get; set; }
    }

    public enum enunciation
    {
        none, ëmgangssproochlech, colloquial
    }
}
