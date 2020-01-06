using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class Example
    {
        protected string exampleText { get; set; }
    }

    class Example_Extended : Example
    {
        protected enunciation enunciation { get; set; }
        protected string enunciationText { get; set; }
    }

    public enum enunciation
    {
        none, ëmgangssproochlech, colloquial
    }
}
