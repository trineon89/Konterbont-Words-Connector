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
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;
using NIL = Newtonsoft.Json.DefaultValueHandling;

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

        private async Task<string> HttpRequest(string Lang, string XML)
        {
            HttpClient httpClient = new HttpClient();
            string LangURL;

            if (Lang == "LU")
            {
                LangURL = "";
            }
            else
            {
                LangURL = Lang.ToLower();
            }
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


            var _responseT = httpClient.SendAsync(httpContent, new HttpCompletionOption());
            _responseT.Wait();
            var _response = _responseT.Result;
            _response.EnsureSuccessStatusCode();

            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            return responseBody;
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
        public string XMLFile;
        public bool IsVariant;


        public Wuert()
        {
            WuertLuS = null;
            Meanings = new List<Meaning>();
            WuertForm = new WordForm();
            Selection = 0;
            MP3 = null;
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
