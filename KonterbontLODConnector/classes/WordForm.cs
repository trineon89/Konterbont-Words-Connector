using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public abstract class WordForm
    {
        protected abstract string WordFormStringLu { get; set; }
        protected abstract string WordFormStringDe { get; set; }
        protected abstract string WordFormStringFr { get; set; }
        protected abstract string WordFormStringEn { get; set; }
        protected abstract string WordFormStringPt { get; set; }

        protected abstract List<string> WordFormPlurals { get; }
    }

    public class WordForm_Default : WordForm
    {
        protected override string WordFormStringLu { get; set; } = "wordForm";
        protected override string WordFormStringDe { get; set; } = "wordForm";
        protected override string WordFormStringFr { get; set; } = "wordForm";
        protected override string WordFormStringEn { get; set; } = "wordForm";
        protected override string WordFormStringPt { get; set; } = "wordForm";

        protected override List<string> WordFormPlurals { get; }

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
        protected override string WordFormStringLu { get; set; } = "Adjektiv";
        protected override string WordFormStringDe { get; set; } = "Adjektiv";
        protected override string WordFormStringFr { get; set; } = "adjectif";
        protected override string WordFormStringEn { get; set; } = "adjective";
        protected override string WordFormStringPt { get; set; } = "adjetivo";

        protected new List<string> WordFormPlurals = null;
    }

    class WordForm_Verb : WordForm_Default
    {
        protected override string WordFormStringLu { get; set; } = "Verb";
        protected override string WordFormStringDe { get; set; } = "Verb";
        protected override string WordFormStringFr { get; set; } = "verbe";
        protected override string WordFormStringEn { get; set; } = "verb";
        protected override string WordFormStringPt { get; set; } = "verbo";

        protected new List<string> WordFormPlurals = null;

        protected WordBase WordFormHelperVerb { get; set; }

        protected string pastParticiple { get; set; }
    }

    class WordForm_ModalVerb : WordForm_Default
    {
        protected override string WordFormStringLu { get; set; } = "Modalverb";
        protected override string WordFormStringDe { get; set; } = "Modalverb";
        protected override string WordFormStringFr { get; set; } = "verbe de modalité";
        protected override string WordFormStringEn { get; set; } = "modal verb";
        protected override string WordFormStringPt { get; set; } = "verbo modal";

        protected new List<string> WordFormPlurals = null;

        protected WordBase WordFormHelperVerb { get; set; }

        protected string pastParticiple { get; set; }
    }

    class WordForm_ProperNoun : WordForm_Default
    {
        protected override string WordFormStringLu { get; set; } = "Eegennumm";
        protected override string WordFormStringDe { get; set; } = "Eigenname";
        protected override string WordFormStringFr { get; set; } = "nom propre";
        protected override string WordFormStringEn { get; set; } = "proper noun";
        protected override string WordFormStringPt { get; set; } = "nome própio";

        protected new List<string> WordFormPlurals = null;
    }
}
