using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public abstract class WordForm
    {
        public abstract string WordFormStringLu { get; set; }
        public abstract string WordFormStringDe { get; set; }
        public abstract string WordFormStringFr { get; set; }
        public abstract string WordFormStringEn { get; set; }
        public abstract string WordFormStringPt { get; set; }

        public abstract List<string> WordFormPlurals { get; }
    }

    public class WordForm_Default : WordForm
    {
        public override string WordFormStringLu { get; set; } = "wordForm";
        public override string WordFormStringDe { get; set; } = "wordForm";
        public override string WordFormStringFr { get; set; } = "wordForm";
        public override string WordFormStringEn { get; set; } = "wordForm";
        public override string WordFormStringPt { get; set; } = "wordForm";

        public override List<string> WordFormPlurals { get; }

        public WordForm_Default()
        {
            WordFormPlurals = new List<string>();
        }

        public virtual String GetWordForm()
        {
            return GetWordForm("LU");
        }
        public virtual String GetWordForm(string languageCode)
        {
            switch (languageCode)
            {
                case "DE": return WordFormStringDe;
                case "FR": return WordFormStringFr;
                case "EN": return WordFormStringEn;
                case "PT": return WordFormStringPt;
                case "LU":
                default: return WordFormStringLu;
            }
        }

        public virtual void SetWordForm(string languageCode, string newWordString)
        {
            switch (languageCode)
            {
                case "DE": WordFormStringDe = newWordString; break;
                case "FR": WordFormStringFr = newWordString; break;
                case "EN": WordFormStringEn = newWordString; break;
                case "PT": WordFormStringPt = newWordString; break;
                case "LU": WordFormStringLu = newWordString; break;
                default: return;
            }
        }

        public virtual void SetWordForm(string LU, string DE, string FR, string EN, string PT)
        {
            SetWordForm("LU", LU);
            SetWordForm("DE", DE);
            SetWordForm("FR", FR);
            SetWordForm("EN", EN);
            SetWordForm("PT", PT);
        }
    }

    class WordForm_Adjectiv : WordForm_Default
    {
        public override string WordFormStringLu { get; set; } = "Adjektiv";
        public override string WordFormStringDe { get; set; } = "Adjektiv";
        public override string WordFormStringFr { get; set; } = "adjectif";
        public override string WordFormStringEn { get; set; } = "adjective";
        public override string WordFormStringPt { get; set; } = "adjetivo";

        public new List<string> WordFormPlurals = null;
    }

    class WordForm_Verb : WordForm_Default
    {
        public override string WordFormStringLu { get; set; } = "Verb";
        public override string WordFormStringDe { get; set; } = "Verb";
        public override string WordFormStringFr { get; set; } = "verbe";
        public override string WordFormStringEn { get; set; } = "verb";
        public override string WordFormStringPt { get; set; } = "verbo";

        public new List<string> WordFormPlurals = null;

        public string WordFormHelperVerb { get; set; }

        public string pastParticiple { get; set; }
    }

    class WordForm_ModalVerb : WordForm_Default
    {
        public override string WordFormStringLu { get; set; } = "Modalverb";
        public override string WordFormStringDe { get; set; } = "Modalverb";
        public override string WordFormStringFr { get; set; } = "verbe de modalité";
        public override string WordFormStringEn { get; set; } = "modal verb";
        public override string WordFormStringPt { get; set; } = "verbo modal";

        public new List<string> WordFormPlurals = null;

        public string WordFormHelperVerb { get; set; }

        public string pastParticiple { get; set; }
    }

    class WordForm_ProperNoun : WordForm_Default
    {
        public override string WordFormStringLu { get; set; } = "Eegennumm";
        public override string WordFormStringDe { get; set; } = "Eigenname";
        public override string WordFormStringFr { get; set; } = "nom propre";
        public override string WordFormStringEn { get; set; } = "proper noun";
        public override string WordFormStringPt { get; set; } = "nome própio";

        public new List<string> WordFormPlurals = null;
    }

    public class WordForm_Overload
    {
        public string WordFormStringLu { get; set; } = "wordForm";
        public string WordFormStringDe { get; set; } = "wordForm";
        public string WordFormStringFr { get; set; } = "wordForm";
        public string WordFormStringEn { get; set; } = "wordForm";
        public string WordFormStringPt { get; set; } = "wordForm";

        public List<string> WordFormPlurals { get; set; }
        public string WordFormHelperVerb { get; set; }
        public string pastParticiple { get; set; }
        public string eiffelerForm { get; set; }
        public bool isVariant { get; set; } = false;

        public void SetWordForm(string languageCode, string newWordString)
        {
            switch (languageCode)
            {
                case "DE": WordFormStringDe = newWordString; break;
                case "FR": WordFormStringFr = newWordString; break;
                case "EN": WordFormStringEn = newWordString; break;
                case "PT": WordFormStringPt = newWordString; break;
                case "LU": WordFormStringLu = newWordString; break;
                default: return;
            }
        }

        public void InitWordFormPlurals()
        {
            if (WordFormPlurals == null)
                WordFormPlurals = new List<string>();
        }
    }
}
