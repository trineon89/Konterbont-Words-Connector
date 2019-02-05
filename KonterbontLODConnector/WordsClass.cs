using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{

    public class AutoComplete
    {
        public int internalId;
        public List<Wuert> Wierder;
        public int Selection;

        public AutoComplete()
        {
            Wierder = new List<Wuert>();
            Selection = 0;
        }

    }

    [Serializable]
    public class Wuert
    {
        public string WuertLu;
        public string WuertLuS;
        public WordForm WuertForm;
        public List<Meaning> Meanings;
        public int Selection;
        public string MP3;
        public string XMLFile;

        public Wuert()
        {
            WuertLuS = null;
            Meanings = new List<Meaning>();
            WuertForm = new WordForm();
            Selection = 0;
            MP3 = null;
            XMLFile = null;
        }
    }

    public partial class WordForm
    {
        public string WuertForm;

        public WordForm() { WuertForm = null; }
        public WordForm(string Word)
        {
            WuertForm = Word;
        }
    }

    public partial class Example
    {
        public string ExampleText;
        public Example()
        {
            this.ExampleText = null;
        }
        public Example(string ExText)
        {
            ExampleText = ExText;
        }
    }

    public class Meaning
    {
        public string LU;
        public string LUs;
        public string DE;
        public string FR;
        public string EN;
        public string PT;
        public List<Example> Examples;
        public string MP3;

        public Meaning() // Constructor
        {
            LU = null;
            LUs = null;
            DE = null;
            FR = null;
            EN = null;
            PT = null;
            Examples = new List<Example>();
        }
    }
}
