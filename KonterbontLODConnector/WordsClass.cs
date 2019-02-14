using System;
using System.Collections.Generic;
using System.Linq;

namespace KonterbontLODConnector
{

    public class AutoComplete : IEquatable<AutoComplete>
    {
        public int internalId;
        public string Occurence;
        public List<Wuert> Wierder;
        public int Selection;

        public AutoComplete()
        {
            Wierder = new List<Wuert>();
            Selection = 0;
        }

        public bool Equals(AutoComplete other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            if (Wierder.Count == other.Wierder.Count)
            {
                foreach(Wuert wuert in Wierder)
                {
                    foreach (Wuert otherWuert in other.Wierder)
                    {
                        if (wuert.Equals(otherWuert)) return true;
                    }
                }
                return false;
            }
            else return false;
        }

        public bool DeepCheck(AutoComplete ac)
        {
            foreach (Wuert word in ac.Wierder)
            {
                foreach (Meaning mean in word.Meanings)
                {
                    var tmp = ac.Wierder.FirstOrDefault(acx => acx.Meanings.Any(x => (x.LU == mean.LU) && (x.LUs == mean.LUs)
                    && (x.DE == mean.DE) && (x.FR == mean.FR) && (x.EN == mean.EN) && (x.PT == mean.PT) ));
                    if (tmp == null) return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class Wuert : IEquatable<Wuert>
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

        public bool Equals(Wuert other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            var tmp=WuertLu.Equals(other.WuertLu) && WuertForm.WuertForm.Equals(other.WuertForm.WuertForm) && MP3.Equals(other.MP3) && XMLFile.Equals(other.XMLFile);
            return tmp;
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
        public string EGS;
        public Example()
        {
            this.ExampleText = null;
        }
        public Example(string ExText,string EGSText)
        {
            ExampleText = ExText;
            EGS = EGSText;
        }
    }

    public class Meaning
    {
        public string LU;
        public string LUs;
        public string HV;
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
            HV = null;
            DE = null;
            FR = null;
            EN = null;
            PT = null;
            Examples = new List<Example>();
        }
    }
}
