﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return true;
            }
            else return false;
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
