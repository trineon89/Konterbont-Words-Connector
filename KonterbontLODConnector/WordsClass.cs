using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;
using NIL = Newtonsoft.Json.DefaultValueHandling;

namespace KonterbontLODConnector
{
  
    public class AutoComplete : IEquatable<AutoComplete>
    {
        public string Occurence;
        public List<Wuert> Wierder;
        public int Selection;
        public int internalId; [JsonProperty("id")] public string id { get; set; }
        [JsonProperty("article_id")] public string ArticleId { get; set; }
        [JsonProperty("word_lb")]  public string WordLb { get; set; }
        [JsonProperty("scientific_name")] public string ScientificName { get; set; }
        [JsonProperty("pos")] public string Pos { get; set; }
        [JsonProperty("erroneous")] public bool Erroneous { get; set; }
        [JsonProperty("matches", NullValueHandling = NullValueHandling.Ignore)] public List<string> Matches { get; set; }

        public static AutoComplete FromJson(string json) => JsonConvert.DeserializeObject<AutoComplete>(json, KonterbontLODConnector.Converter.Settings);

        public AutoComplete()
        {
            Wierder = new List<Wuert>();
            Selection = 0;

        }

        public string GetWuertForm (dynamic LODString)
        {
            string convertedValue = Convert.ToString(LODString);
            switch (convertedValue)
            {
                case "SUBST": return "Substantiv";
                case "SUBST+F": return "weiblecht Substantiv";
                case "SUBST+M": return "männlecht Substantiv"; 
                case "SUBST+N": return "sächlecht Substantiv";
                case "SUBST+MF": return "männlecht/weiblecht Substantiv";
                case "VRB": return "Verb";
                case "ADV": return "Adverb";
                case "ADJ": return "Adjektiv";
                case "ADJ+INV": return "onverännerbaart Adjektiv";
                case "NP": return "Eegennumm";
                case "PART": return "Partikel";
                case "VRBPART": return "Verbpartikel";
                case "PREP": return "Prepositioun";
                case "PRON+INDEF": return "Indefinitpronomen";
                case "SUBST+MN": return "männlecht/sächlecht Substantiv";
                case "PRON+POSS": return "Possessivpronomen";
                case "PRON+PERS": return "Personalpronomen";
                case "CONJ": return "Konjunktioun";
                case "INTERJ": return "Interjektioun";
                case "NB+CARD": return "Kardinalzuel";
                case "ART+INDEF": return "onbestëmmten Artikel";
                case "SUBST+FN": return "weiblecht/sächlecht Substantiv";
                case "NB+ORD": return "Ordinalzuel";
                case "PRON+DEM": return "Demonstrativpronomen";
                case "PRON+REL": return "Relativpronomen";
                case "ART+DEF": return "bestëmmten Artikel";
                case "VRB+MOD": return "Modalverb";

                default: return convertedValue;
            }
        }

        public bool CompareAutoComplete(AutoComplete first, AutoComplete second, DataHandler dataHandler, int index)
        {
            bool result = false;
            List<string> results = new List<string>();
            //Set Selections
            first.Selection = second.Selection;
            for (int i = 0; i < first.Wierder.Count; i++)
                if (i < second.Wierder.Count) first.Wierder[i].Selection = second.Wierder[i].Selection; else first.Wierder[i].Selection = 1;
                
            //Check Counts
            if (first.Wierder.Count != second.Wierder.Count) { result = true; }
            for (int i = 0; i < first.Wierder.Count; i++) if (first.Wierder[i].Meanings.Count != second.Wierder[i].Meanings.Count) 
                { 
                    result = true; 
                }

            for (int i = 0; i < first.Wierder.Count; i++)
                for (int j = 0; j < first.Wierder[i].Meanings.Count; j++)
                    if (second.Wierder[i].Meanings[j].hasCustomAudio == false)
                    {
                        first.Wierder[i].Meanings[j].hasCustomAudio = true;
                        first.Wierder[i].Meanings[j].MP3 = second.Wierder[i].Meanings[j].MP3;
                    }

                if (result)
            {
                dataHandler.WordList[index] = first;
                return result;
            } else
            {
                for (int i = 0; i < first.Wierder.Count; i++)
                {
                    if (first.Wierder[i].WuertLu != second.Wierder[i].WuertLu) { result = true; results.Add(second.Wierder[i].WuertLu); }
                    if (first.Wierder[i].WuertForm.ToString() != second.Wierder[i].WuertForm.ToString()) 
                    { result = true; results.Add(second.Wierder[i].WuertForm.ToString()); }
                    if (first.Wierder[i].WuertLuS != second.Wierder[i].WuertLuS) { result = true; results.Add(second.Wierder[i].WuertLuS); }
                    if (first.Wierder[i].M4A != second.Wierder[i].M4A) { result = true; results.Add(second.Wierder[i].M4A); }
                    if (first.Wierder[i].MP3 != second.Wierder[i].MP3) { result = true; results.Add(second.Wierder[i].MP3); }
                    for (int j = 0; j< first.Wierder[i].Meanings.Count; j++)
                    {
                        if (first.Wierder[i].Meanings[j].DE != second.Wierder[i].Meanings[j].DE) { result = true; results.Add(second.Wierder[i].Meanings[j].DE); }
                        if (first.Wierder[i].Meanings[j].EN != second.Wierder[i].Meanings[j].EN) { result = true; results.Add(second.Wierder[i].Meanings[j].EN); }
                        if (first.Wierder[i].Meanings[j].FR != second.Wierder[i].Meanings[j].FR) { result = true; results.Add(second.Wierder[i].Meanings[j].FR); }
                        if (first.Wierder[i].Meanings[j].PT != second.Wierder[i].Meanings[j].PT) { result = true; results.Add(second.Wierder[i].Meanings[j].PT); }
                        if (first.Wierder[i].Meanings[j].HV != second.Wierder[i].Meanings[j].HV) { result = true; results.Add(second.Wierder[i].Meanings[j].HV); }
                        if (first.Wierder[i].Meanings[j].LU != second.Wierder[i].Meanings[j].LU) { result = true; results.Add(second.Wierder[i].Meanings[j].LU); }
                        if (first.Wierder[i].Meanings[j].LUs != second.Wierder[i].Meanings[j].LUs) { result = true; results.Add(second.Wierder[i].Meanings[j].LUs); }
                        if (first.Wierder[i].Meanings[j].M4A != second.Wierder[i].Meanings[j].M4A) { result = true; results.Add(second.Wierder[i].Meanings[j].M4A); }
                        if (first.Wierder[i].Meanings[j].MP3 != second.Wierder[i].Meanings[j].MP3 && second.Wierder[i].Meanings[j].hasCustomAudio != false) { result = true; results.Add(second.Wierder[i].Meanings[j].MP3); }
                        if (first.Wierder[i].Meanings[j].Examples.ToString() != second.Wierder[i].Meanings[j].Examples.ToString()) 
                        { result = true; results.Add(second.Wierder[i].Meanings[j].Examples.ToString()); }
                    }
                }

            }

            dataHandler.WordList[index] = first;
            return result;
        }

        public async Task<AutoComplete> GetAutoComplete2022(string searchstring, LodSearch fetchedSearch, LogClass log)
        {
            this.Occurence = searchstring;

            int _i = 1;
            string attrib = null;

            AutoComplete actemp = new AutoComplete();

            foreach (Result _result in fetchedSearch.Results)
            {
                Wuert wuert = new Wuert
                {
                    WuertLu = _result.WordLb,
                    WuertForm = new WordForm(null),
                    Selection = 1,
                    XMLFile = _result.ArticleId,
                    MP3 = null, //not accessesible yet
                    M4A = null
                };

                //ac.Wierder.Add(wuert);

                actemp.Wierder.Add(wuert);
                _i++;
            }

            this.Selection = 1;

            int _Total = 0;

            foreach (Wuert wuert in actemp.Wierder)
            {
                dynamic article = await RequestArticle(wuert.XMLFile);
                if (IsContainsKey(article.entry, "partOfSpeechLabel")) wuert.WuertForm.WuertForm = GetWuertForm(article.entry.partOfSpeechLabel);
                else wuert.WuertForm.WuertForm = GetWuertForm(article.entry.partOfSpeech);
                Console.WriteLine(article.entry);
                if (IsContainsKey(article.entry,"audioFiles"))
                {
                    string mp3url = article.entry.audioFiles.aac;
                    wuert.M4A = mp3url;
                    string[] urlpaths = mp3url.Split('/');
                    string[] filename = urlpaths.Last().Split('.');
                    wuert.MP3 = filename[0] + ".mp3";
                }
                
                if (IsContainsKey(article.entry, "microStructures"))
                {
                    List<MicroStructure> lms = article.entry.microStructures.ToObject<List<MicroStructure>>();
                    
                    foreach (MicroStructure ms in lms)
                    {
                        //Meanings
                        Meaning meaning = new Meaning();


                        if (ms.Inflection != null)
                        {
                            foreach (LODForm _form in ms.Inflection.Forms)
                            {
                                if (meaning.LUs != null) { meaning.LUs += "/"; }
                                meaning.LUs += _form.Content;
                            }
                        }

                        if (ms.PastParticiple != null)
                        {
                            foreach (string _pp in ms.PastParticiple)
                            {
                                if (meaning.LUs != null) { meaning.LUs += "/"; }
                                meaning.LUs += _pp;
                            }
                        }

                        if (ms.AuxiliaryVerb != null)
                        {
                            //Hellefs VERB
                            meaning.HV = ms.AuxiliaryVerb;
                        }

                        if (ms.GrammaticalUnits != null)
                        {
                            foreach (var _a in ms.GrammaticalUnits)
                            {
                                foreach (LODMeaning _meaning in _a.Meanings)
                                {
                                    Meaning meaningInside = new Meaning() { LUs = meaning.LUs };
                                    if (meaning.HV != null) meaningInside.HV = meaning.HV;

                                    if (_meaning.SecondaryHeadword != null) meaningInside.LU = _meaning.SecondaryHeadword;
                                    else meaningInside.LU = wuert.WuertLu;

                                    if (_meaning.Inflection != null)
                                    {
                                        if (_meaning.Inflection.Forms != null) {
                                            foreach (LODForm _form in _meaning.Inflection.Forms)
                                            {
                                                if (meaningInside.LUs != null) { meaningInside.LUs += "/"; }
                                                meaningInside.LUs += _form.Content;
                                            }
                                        }

                                        if (_meaning.Inflection.DeclensionInfo != null)
                                        {
                                            string tmp = meaningInside.LUs;
                                            meaningInside.LUs = "(" + _meaning.Inflection.DeclensionInfo + ")";

                                            if (_meaning.Inflection.DeclensionInfo == "kee Singulier")
                                                meaningInside.LU = tmp;
                                            //if (_meaning.Inflection.DeclensionInfo == "kee Pluriel")
                                                    
                                        }
                                    }

                                    if (_meaning.DeclensionInfo != null)
                                    {
                                        if (_meaning.DeclensionInfo == "kee Pluriel") meaningInside.LUs = "(" + _meaning.DeclensionInfo + ")";
                                    }

                                    meaningInside.MP3 = wuert.MP3;
                                    meaningInside.M4A = wuert.M4A;

                                    meaningInside.DE = returnBuildedLanguage(_meaning.TargetLanguages.De.Parts);
                                    meaningInside.FR = returnBuildedLanguage(_meaning.TargetLanguages.Fr.Parts);
                                    meaningInside.EN = returnBuildedLanguage(_meaning.TargetLanguages.En.Parts);
                                    meaningInside.PT = returnBuildedLanguage(_meaning.TargetLanguages.Pt.Parts);

                                    foreach (LODExample example in _meaning.LODExamples)
                                    {
                                        Example exampletmp = new Example();
                                        foreach (ExamplePart part in example.Parts)
                                        {
                                            //if type: "gloss" <- still same example
                                            if (part.Type == FluffyType.Text)
                                            {
                                                string concat = "";
                                                foreach (PartPart _subpart in part.Parts)
                                                {
                                                    if (_subpart.Type == PurpleType.Attribute
                                                        && _subpart.Content == "EGS") continue;
                                                    if (concat.Length>0) { concat += " "; }
                                                    concat += _subpart.Content;
                                                }
                                                exampletmp.ExampleText = concat;
                                            }
                                            if (part.Type == FluffyType.Gloss)
                                            {
                                                string concat = "";
                                                foreach (PartPart _subpart in part.Parts)
                                                {
                                                    if (concat.Length > 0) { concat += " "; }
                                                    concat += _subpart.Content;
                                                }
                                                exampletmp.EGS = " [" + concat + "]";
                                            }
                                        }

                                        meaningInside.Examples.Add(exampletmp);
                                    }
                                    wuert.Meanings.Add(meaningInside);
                                }
                                
                            }
                        }
                        

                        //wuert.Meanings.Add(meaning);
                    }
                } else
                {
                    Console.WriteLine("No microStructure");
                }
               
                int _j = 1;
                string Selection = "";


                //wuert.WuertForm.WuertForm = 
                
                this.Wierder.Add(wuert);
            }

            return this;
        }

        string returnBuildedLanguage(List<DePart> parts)
        {
            string tmp = "";
            foreach (DePart part in parts)
            {
                if (part.Type == "translation")
                {
                    if (tmp.Length > 0) { tmp += ", "; }
                    tmp += part.Content;
                }
                if (part.Type == "semanticClarifier") { tmp += " [" + part.Content + "]"; }
            }
            return tmp;
        }

        bool IsContainsKey(dynamic newtonsoftDynamic, string propertyName)
        {
            return (newtonsoftDynamic as Newtonsoft.Json.Linq.JObject).ContainsKey(propertyName);
        }

        public async Task<AutoComplete> GetFullAutoComplete(string searchstring, string fetchedXml, LogClass Log)
        {
            this.Occurence = searchstring;

            HtmlAgilityPack.HtmlDocument XmlDocument = new HtmlAgilityPack.HtmlDocument();
            HtmlNode[] XmlNodes;

            XmlDocument.LoadHtml(fetchedXml);
            XmlNodes = XmlDocument.DocumentNode.SelectNodes("//a[@onclick]").ToArray();

            int _i = 1;
            string attrib = null;

            AutoComplete actemp = new AutoComplete();

            foreach (HtmlNode htmlNode in XmlNodes)
            {
                attrib = XmlDocument.DocumentNode.SelectNodes("//a[@onclick]")[_i - 1].GetAttributeValue("onclick", "default");
                string tmpxml = attrib.Remove(0, attrib.IndexOf("'") + 1); // result: PERSOUN1.xml','persoun1.mp3')
                string tmpmp3 = tmpxml.Remove(0, tmpxml.IndexOf("'") + 1); // result: ,'persoun1.mp3')
                tmpmp3 = tmpmp3.Remove(0, tmpmp3.IndexOf("'") + 1);

                string tmpWordForm = XmlDocument.DocumentNode.SelectSingleNode("//span[@class='s4']").InnerText.Trim();
                htmlNode.RemoveChild(XmlDocument.DocumentNode.SelectNodes("//span[@class='s4']").First());

                Wuert wuert = new Wuert
                {
                    WuertLu = htmlNode.InnerText.Trim(),
                    WuertForm = new WordForm(null),
                    Selection = 1,
                    XMLFile = tmpxml.Substring(0, tmpxml.IndexOf("'")),
                    MP3 = tmpmp3.Substring(0, tmpmp3.IndexOf("'"))
                };

                //ac.Wierder.Add(wuert);

                actemp.Wierder.Add(wuert);
                _i++;
            }

            this.Selection = 1;
            //----------------------------------------------------------------------------------------------------
            int _Total = 0;
            string[] LangArray = { "LU", "DE", "FR", "EN", "PT" };

            foreach (Wuert wuert in actemp.Wierder)
            {
                foreach (string Lang in LangArray)
                {
                    string responseBody = await HttpRequest(Lang, wuert.XMLFile);

                    HtmlAgilityPack.HtmlDocument LangDocument = new HtmlAgilityPack.HtmlDocument();
                    LangDocument.LoadHtml(responseBody);

                    // Meanings START
                    if (LangDocument.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
                    {
                        int _j = 1;
                        string Selection = "";

                        if (Lang == "LU")
                        {
                            if (LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                            {
                                wuert.WuertForm.WuertForm = LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim();
                            }
                            else
                            {
                                wuert.WuertForm.WuertForm = LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass'][2]").InnerText.Trim();
                            }
                        }

                        HtmlNodeCollection htmlNodes = LangDocument.DocumentNode.SelectNodes("//div[@class='uds_block']");
                        foreach (HtmlNode Meaning in htmlNodes)
                        {
                            string MeaningText = "";
                            string MeaningTextAdd = "";
                            string Pluriel;

                            HtmlNode[] LUsArray = LangDocument.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();

                            if (LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                            {
                                //HtmlNode[] MeaningArray;
                                if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']") != null) //has base pluriel
                                {
                                    if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']") != null) //Failsafe pluriel
                                    {
                                        Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                                        //trim ouni -n
                                        int posi = Pluriel.IndexOf("ouni -n");
                                        if (posi > 0) { Pluriel = Pluriel.Remove(posi, Pluriel.Length - posi); }
                                        posi = Pluriel.IndexOf("sem -n");
                                        if (posi > 0) { Pluriel = Pluriel.Remove(posi, Pluriel.Length - posi); }
                                        Pluriel = Pluriel.Replace("&lt;", "<");
                                        Pluriel = Pluriel.Replace("&gt;", ">");
                                    }
                                    else
                                        Pluriel = null;
                                }
                                else
                                    Pluriel = null; // no base pluriel
                            }
                            else
                            {
                                if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='text_gen']") != null) //has base pluriel
                                {
                                    if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='text_gen']/span[@class='mentioun_adress']") != null) //Failsafe pluriel
                                    {
                                        Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                                        Pluriel = Pluriel.Replace("&lt;", "<");
                                        Pluriel = Pluriel.Replace("&gt;", ">");
                                    }
                                    else
                                        Pluriel = null;
                                }
                                else
                                    Pluriel = null; // no base pluriel                       
                            }
                            wuert.WuertLuS = Pluriel;

                            Meaning meaning = new Meaning();

                            if (wuert.WuertForm.WuertForm == "Verb" || wuert.WuertForm.WuertForm == "Modalverb")
                            {
                                meaning.LU = wuert.WuertLu;
                                meaning.HV = Pluriel; // writes "Hëllefsverb" to class
                                                      // write PP to LUs variable
                                if (LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                                {
                                    if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress']") != null)
                                    {
                                        Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress']").InnerText;
                                        if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress'][2]") != null)
                                        {
                                            string Pluriel2 = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='mentioun_adress'][2]").InnerText;
                                            Pluriel = Pluriel + " / " + Pluriel2;
                                        }
                                        Pluriel = Pluriel.Replace("&lt;", "<");
                                        Pluriel = Pluriel.Replace("&gt;", ">");
                                    }
                                    else
                                        Pluriel = null;

                                }
                                else
                                {
                                    if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress']") != null)
                                    {
                                        Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress']").InnerText;
                                        if (Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress'][2]") != null)
                                        {
                                            string Pluriel2 = Meaning.SelectSingleNode("//div[@class='artikel']/div[@class='s20'][" + _i + "]/span[@class='mentioun_adress'][2]").InnerText;
                                            Pluriel = Pluriel + " / " + Pluriel2;
                                        }
                                        Pluriel = Pluriel.Replace("&lt;", "<");
                                        Pluriel = Pluriel.Replace("&gt;", ">");
                                    }
                                    else
                                        Pluriel = null;
                                }
                                meaning.LUs = Pluriel;
                                wuert.WuertLuS = Pluriel;
                            }
                            else
                            {
                                if (Lang == "LU")
                                {

                                    /*
                                     * 1 Meaning (kee Pluriel) ::html:: <span class=text_gen> ( <span class=info_plex>kee Pluriel</span>  ) </span>
                                     * 
                                     * 2 Meaning (Pluriel PlurielWuert) ::html:: <span class=text_gen> ( <span class=info_plex>Pluriel <span class=mentioun_adress> 
                                     *      <span class=mentioun_adress> PlurielWuert </span> </span> </span>  ) </span>            
                                     * 
                                     * 3 Meaning SpecialWuert ::html:: <span class=polylex> SpecialWuert </span>
                                     * 
                                     * 4 Meaning DE Wuert ::html:: <span class=intro_et> ..... </span>
                                     * 
                                     */

                                    if (Meaning.SelectSingleNode(".//span[@class='text_gen']") != null)
                                    { // Meaning 1 or 2 or 4
                                        meaning.LU = wuert.WuertLu;
                                        meaning.LUs = Pluriel;
                                        Console.WriteLine("Meaning 1, 2, 4");
                                        if (Meaning.SelectSingleNode(".//span[@class='text_gen'][1]").ChildNodes.Count() == 3)
                                        { // -> Meaning 1
                                            Console.WriteLine("Meaning 1");
                                            if (Meaning.SelectSingleNode(".//span[@class='polylex']") != null)
                                            {
                                                meaning.LU = Meaning.SelectSingleNode(".//span[@class='polylex']").InnerText;
                                            }
                                            //meaning.LUs = null;
                                        }
                                        else
                                        {
                                            if (Meaning.SelectSingleNode(".//span[@class='polylex']") != null)
                                            { // -> Meaning 3
                                                Console.WriteLine("Meaning 3");
                                                meaning.LU = Meaning.SelectSingleNode(".//span[@class='polylex']").InnerText;
                                                meaning.LUs = wuert.WuertLuS;
                                            }
                                            else if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                                            { // Meaning 2 or 4
                                                Console.WriteLine("Meaning 2, 4");
                                                if (Meaning.SelectSingleNode(".//span[@class='info_flex']") != null)
                                                { // -> Meaning 2
                                                    Console.WriteLine("Meaning 2");
                                                    meaning.LUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
                                                    //trim ouni -n
                                                    int posi = meaning.LUs.IndexOf("ouni -n");
                                                    if (posi > 0) { meaning.LUs = meaning.LUs.Remove(posi, meaning.LUs.Length - posi); }

                                                }
                                                else
                                                { // -> Meaning 4
                                                    Console.WriteLine("Meaning 4");
                                                }
                                            }
                                            else
                                            { // -> Meaning 1
                                                Console.WriteLine("Meaning 1");
                                                meaning.LUs = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Meaning.SelectSingleNode(".//span[@class='polylex']") != null)
                                        { // -> Meaning 3
                                            Console.WriteLine("Meaning 3");
                                            meaning.LU = Meaning.SelectSingleNode(".//span[@class='polylex']").InnerText;
                                            meaning.LUs = wuert.WuertLuS;
                                        }
                                        else
                                        { // -> Meaning 4 safe
                                            Console.WriteLine("Meaning 4 (safe)");
                                            meaning.LUs = wuert.WuertLuS;
                                            meaning.LU = wuert.WuertLu;
                                        }
                                    }
                                }
                            }

                            // Source: https://stackoverflow.com/questions/541954/how-would-you-count-occurrences-of-a-string-actually-a-char-within-a-string
                            // check expression
                            if (meaning.LU != null)
                            {
                                int count = meaning.LU.Count(f => f == ' ');
                                if (count > 0)
                                {
                                    meaning.LUs = null;
                                }
                            }
                            //
                            if (Lang != "LU")
                            {
                                // var ModMean = Meaning;
                                int i = 0;
                                while (Meaning.InnerHtml.Contains("intro_et"))
                                {
                                    Meaning.ChildNodes[i].Remove();
                                }

                                var RemoveNode = Meaning.SelectSingleNode("./div[@class='bspsblock']");
                                if (RemoveNode != null)
                                    Meaning.RemoveChild(RemoveNode);
                                RemoveNode = Meaning.SelectSingleNode("./div[@class='syn_block']");
                                if (RemoveNode != null)
                                    Meaning.RemoveChild(RemoveNode);

                                Console.Write(Meaning.InnerText);

                                MeaningText = Meaning.InnerText;

                                string regex = "(\\&lt;.*\\&gt;)";
                                MeaningText = Regex.Replace(MeaningText, regex, "");

                                if (MeaningText.Contains("--- coming soon ---") || MeaningText.Contains("--- disponível em breve ---"))
                                {
                                    MeaningText = null;
                                }
                            }

                            switch (Lang)
                            {
                                case "LU":
                                    meaning.MP3 = wuert.MP3;
                                    HtmlNodeCollection htmlExamples = Meaning.SelectNodes(".//span[@class='beispill']");
                                    foreach (HtmlNode htmlexample in htmlExamples)
                                    {
                                        var RemoveNode = htmlexample.SelectSingleNode("span[@id='sprangop']");
                                        if (RemoveNode != null)
                                        {
                                            htmlexample.RemoveChild(RemoveNode);
                                        }

                                        string EGS = "";
                                        if (htmlexample.SelectSingleNode(".//span[@class='text_gen']") != null)
                                        {
                                            EGS = htmlexample.SelectSingleNode(".//span[@class='text_gen']").InnerText;
                                        }

                                        RemoveNode = htmlexample.SelectSingleNode(".//span[@class='text_gen']");
                                        if (RemoveNode != null)
                                        {
                                            htmlexample.RemoveChild(RemoveNode);
                                        }
                                        Example example = new Example(htmlexample.InnerText, EGS.Trim());

                                        meaning.Examples.Add(example);
                                    }
                                    break;
                                case "DE":
                                    wuert.Meanings[_j - 1].DE = MeaningText + MeaningTextAdd;
                                    break;
                                case "FR":
                                    wuert.Meanings[_j - 1].FR = MeaningText + MeaningTextAdd;
                                    break;
                                case "EN":
                                    wuert.Meanings[_j - 1].EN = MeaningText + MeaningTextAdd;
                                    break;
                                case "PT":
                                    wuert.Meanings[_j - 1].PT = MeaningText + MeaningTextAdd;
                                    break;
                            }
                            Selection = (_j + 1).ToString();

                            // Wuert wuert = acwuert.Wierder[acwuert.Selection - 1];

                            if (Lang == "LU")
                            { wuert.Meanings.Add(meaning); }
                            _j++;
                            _Total = _j;
                        }
                    }
                }
                this.Wierder.Add(wuert);
            }
            this.Selection = 1;
            return this;
        }

        private async Task<dynamic> RequestArticle(string id_Article)
        {
            bool blocked = true;
            string responseBody = null;
            int i = 0;
            dynamic d = null;

            while (blocked)
            {
                HttpClient httpClient;
                //if more than 30 connections => Exception
                if (i >= 30) { throw new Exception("More than 30 connections, proxy not working!"); }
                if (_Globals.useProxy && i > 0)
                {
                    //get random Proxy & setup Proxy
                    string _proxy = GetProxie();
                    string proxyHost = _proxy.Substring(0, _proxy.IndexOf(":"));
                    string proxyPort = _proxy.Substring(_proxy.IndexOf(":") + 1);
                    var proxy = new WebProxy
                    {
                        Address = new Uri($"http://{proxyHost}:{proxyPort}"),
                        UseDefaultCredentials = true
                    };

                    HttpClientHandler handler = new HttpClientHandler() { Proxy = proxy };
                    handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    httpClient = new HttpClient(handler);
                } else
                {
                    httpClient = new HttpClient();
                }

                var httpContent = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://lod.lu/api/lb/entry/" + id_Article),
                    Method = HttpMethod.Get
                };
                try
                {
                    var _responseT = httpClient.SendAsync(httpContent, new HttpCompletionOption());
                    _responseT.Wait();
                    var _response = _responseT.Result;
                    blocked = !_response.IsSuccessStatusCode;
                    responseBody = await _response.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                //article = LodArticle.FromJson(responseBody);
                d = JsonConvert.DeserializeObject(responseBody);

                httpClient.Dispose();
                i++;
            }
            return d;
        }

        private async Task<string> HttpRequest(string Lang, string XML)
        {
            bool blocked = true;
            string responseBody = null;
            int i = 0;
            while (blocked)
            {
                HttpClient httpClient;
                //if more than 30 connections => Exception
                if (i >= 30) { throw new Exception("More than 30 connections, proxy not working!"); }
                if (_Globals.useProxy && i>0)
                {
                    //get random Proxy & setup Proxy
                    string _proxy = GetProxie();
                    string proxyHost = _proxy.Substring(0, _proxy.IndexOf(":"));
                    string proxyPort = _proxy.Substring(_proxy.IndexOf(":") + 1);
                    var proxy = new WebProxy
                    {
                        Address = new Uri($"http://{proxyHost}:{proxyPort}"),
                        UseDefaultCredentials = true
                    };

                    HttpClientHandler handler = new HttpClientHandler() { Proxy = proxy };
                    handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    httpClient = new HttpClient(handler);
                } else
                {
                    httpClient = new HttpClient();
                }
                string LangURL;

                if (Lang == "LU") { LangURL = ""; }
                else
                { LangURL = Lang.ToLower(); }
                var httpContent = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://www.lod.lu/php/getart" + LangURL + ".php?artid=" + XML),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                        { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                    }
                };
                try
                {
                    var _responseT = httpClient.SendAsync(httpContent, new HttpCompletionOption());
                    _responseT.Wait();
                    var _response = _responseT.Result;
                    blocked = !_response.IsSuccessStatusCode;
                    responseBody = await _response.Content.ReadAsStringAsync();
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                httpClient.Dispose();
                i++;
            }
            return responseBody;
        }

        private List<string> GetProxies()
        {
            List<string> _proxies = new List<string>();
            _proxies.Add("87.249.205.157:8080");
            _proxies.Add("159.224.73.208:23500");
            _proxies.Add("195.9.188.78:53281");
            _proxies.Add("110.232.74.233:30739");
            _proxies.Add("211.252.169.8:80");
            _proxies.Add("158.69.138.15:8080");
            _proxies.Add("3.8.134.151:80");
            _proxies.Add("138.197.157.32:8080");
            _proxies.Add("212.34.254.34:48477");
            _proxies.Add("37.77.135.126:44497");
            _proxies.Add("54.38.110.35:47640");
            _proxies.Add("36.66.151.29:60279");
            _proxies.Add("118.172.51.110:36552");
            _proxies.Add("185.57.228.61:53281");
            _proxies.Add("36.89.194.113:47254");
            _proxies.Add("103.107.133.25:43683");
            /*_proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");
            _proxies.Add("222.331.444.555:80");*/
            return _proxies;
        }

        public string GetProxie()
        {
            List<string> _proxies = GetProxies();
            string proxy = _proxies[new Random().Next(_proxies.Count)];
            return proxy;
        }

        public bool Equals(AutoComplete other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            if (Wierder.Count == other.Wierder.Count)
            {
                foreach (Wuert wuert in Wierder)
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

        public bool NotSoDeepCheck(AutoComplete ac)
        {
            foreach (Wuert word in ac.Wierder)
            {
                foreach (Meaning mean in word.Meanings)
                {
                    var tmp = ac.Wierder.FirstOrDefault(acx => acx.Meanings.Any(x => (x.LUs == mean.LUs)));
                    if (tmp == null) return true;
                }
            }
            return false;
        }

        public bool DeepCheck(AutoComplete acresults, AutoComplete ac)
        {
            foreach (Wuert word in acresults.Wierder)
            {
                foreach (Meaning mean in word.Meanings)
                {
                    //acresults.Wierder
                    // var tmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.WuertLu == x.WuertLu)

                    var tmp = ac.Wierder.FirstOrDefault(acx => acx.Meanings.Any(x => (x.LU == mean.LU) && (x.LUs == mean.LUs)
                    && (x.DE == mean.DE) && (x.FR == mean.FR) && (x.EN == mean.EN) && (x.PT == mean.PT)));
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
        public string M4A;
        public string XMLFile;
        public bool IsVariant;


        public Wuert()
        {
            WuertLuS = null;
            Meanings = new List<Meaning>();
            WuertForm = new WordForm();
            Selection = 0;
            MP3 = null;
            M4A = null;
            XMLFile = null;
            IsVariant = false;
        }

        public bool Equals(Wuert other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            var tmp = WuertLu.Equals(other.WuertLu) && WuertForm.WuertForm.Equals(other.WuertForm.WuertForm) && MP3.Equals(other.MP3) && XMLFile.Equals(other.XMLFile);
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
            this.EGS = "";
        }
        public Example(string ExText, string EGSText)
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
        public string M4A;
        [J("hasCustomAudio", NullValueHandling = N.Ignore)] public bool hasCustomAudio;
        [DefaultValue(false)] [J("hasCustomEN", NullValueHandling = N.Ignore, DefaultValueHandling = NIL.Populate)] public bool hasCustomEN;
        [DefaultValue(false)] [J("hasCustomPT", NullValueHandling = N.Ignore, DefaultValueHandling = NIL.Populate)] public bool hasCustomPT;

        public Meaning() // Constructor
        {
            LU = null;
            LUs = null;
            HV = null;
            DE = null;
            FR = null;
            EN = null;
            PT = null;
            hasCustomAudio = false;
            hasCustomEN = false;
            hasCustomPT = false;
            Examples = new List<Example>();
        }
    }
}
