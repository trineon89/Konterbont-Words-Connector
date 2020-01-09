using HtmlAgilityPack;
using KonterbontLODConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.Implementation
{
    class LODConnector : Interfaces.ILODConnectors
    {
        public classes.WordOverview searchWord(string occ)
        {
            classes.WordOverview wo = new classes.WordOverview();
            wo.valid = true;
            return wo;
        }

        public async static Task<(List<classes.Meaning>, classes.WordForm_Overload)> FetchWordDetails(string xmlWord)
        {
            List<classes.Meaning> lmOut = new List<classes.Meaning>();

            string[] LangArray = { "LU", "DE", "FR", "EN", "PT" };
            classes.WordForm_Overload wordForm = new classes.WordForm_Overload();

            foreach (string currlang in LangArray)
            {
                EmptyClass em = new EmptyClass();
                var responseTask = em.HttpRequest(currlang, xmlWord);
                string responseBody = await responseTask;

                //build wordForm
                wordForm = getWordForm(wordForm, responseBody, currlang);

                //getMeanings

                lmOut = getMeanings(lmOut, responseBody, currlang);
            }



            return (lmOut, wordForm);
        }

        private static classes.WordForm_Overload getWordForm(classes.WordForm_Overload wf_ov, string responseBody, string currentLanguage)
        {

            HtmlAgilityPack.HtmlDocument LangDocument = new HtmlAgilityPack.HtmlDocument();
            LangDocument.LoadHtml(responseBody);
            string wuertForm = null;

            if (LangDocument.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                if (LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim() != "♦")
                {
                    wuertForm = LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim();
                }
                else
                {
                    wuertForm = LangDocument.DocumentNode.SelectSingleNode(".//span[@class='klass'][2]").InnerText.Trim();
                }
                //Set Translated WordForms
                wf_ov.SetWordForm(currentLanguage, wuertForm);

                //Set Base Values (Pluriel(s), Hellefsverb, Adverb), ONLY WHEN LU
                if (currentLanguage == "LU")
                {
                    /* Get <div class="artikel" ...... until <div class="uds_block" */

                    string trgrgrgr = LangDocument.DocumentNode.SelectSingleNode("//div[@class='artikel']").InnerHtml;
                    HtmlDocument articleDoc = new HtmlAgilityPack.HtmlDocument();
                    articleDoc.LoadHtml(trgrgrgr);
                    var divs = articleDoc.DocumentNode.SelectNodes("//div");
                    foreach (var tag in divs)
                    {
                        tag.Remove();
                    }
                    
                    //Gëtt schonn eischter agesaat
                    var Basiswuert = articleDoc.DocumentNode.SelectNodes("//span[@class='adress mentioun_adress']/text()");
                    /*BasisWuert(/Wieder) : //span[@class='adress mentioun_adress']/text()
                     */

                    var HV = articleDoc.DocumentNode.SelectSingleNode("//span[@class='text_gen']/span[@class='mentioun_adress']/text()");
                    if (HV != null)
                        wf_ov.WordFormHelperVerb = HV.InnerText.Trim();
                    /*HV(en) : //span[@class='text_gen']/span[@class='mentioun_adress']/text()
                     */

                    var PP = articleDoc.DocumentNode.SelectSingleNode("//span[contains(.,'Participe passé')]/following-sibling::span[@class='mentioun_adress']/text()");
                    if (PP != null)
                        wf_ov.pastParticiple = PP.InnerText.Trim();
                    /*PP : //div[@class='artikel']/span[@class='mentioun_adress']/text()
                     */

                    var Pluriel = articleDoc.DocumentNode.SelectNodes("//span[@class='mentioun_adress popup']/text()|//span[@class='mentioun_adress']/span[@class='mentioun_adress']/text()");
                    if (Pluriel != null)
                    {
                        if (wf_ov.WordFormPlurals == null) wf_ov.InitWordFormPlurals();
                        foreach (var pluriel in Pluriel)
                        {
                            wf_ov.WordFormPlurals.Add(pluriel.InnerText.Trim());
                        }
                    }
                    /* Pluriel(en): //span[@class='mentioun_adress popup']/text()|//span[@class='mentioun_adress']/span[@class='mentioun_adress']/text()
                     */

                    // substring-after(//span[@id='myPopup'][1]/text(),'ouni -n ')
                    var EiffelerTmp = articleDoc.DocumentNode.SelectSingleNode("//span[@id='myPopup'][1]/text()[1]");
                    if (EiffelerTmp != null)
                        wf_ov.eiffelerForm = EiffelerTmp.InnerText.Replace("ouni -n: ", "");
                    /* Eiffeler Reegel: substring-after(//span[@id='myPopup'][1]/text(),'ouni -n')
                     */

                    var Variant = articleDoc.DocumentNode.SelectNodes("//span[@class='lu_link']");
                    if (Variant != null)
                        wf_ov.isVariant = true;
                    else wf_ov.isVariant = false;
                    /* isVariant: //span[@class='lu_link']
                     */

                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            
            
            return wf_ov;
        }

        private static List<classes.Meaning> getMeanings(List<classes.Meaning> meanings, string responseBody, string currentLanguage)
        {
            HtmlAgilityPack.HtmlDocument LangDocument = new HtmlAgilityPack.HtmlDocument();
            LangDocument.LoadHtml(responseBody);
            if (LangDocument.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                //load cleaned Document
                string trgrgrgr = LangDocument.DocumentNode.SelectSingleNode("//div[@class='artikel']").InnerHtml;
                HtmlDocument articleDoc = new HtmlAgilityPack.HtmlDocument();
                articleDoc.LoadHtml(trgrgrgr);
                var meaningNodes = articleDoc.DocumentNode.SelectNodes("//div[@class='uds_block']");

                int i = 0;
                //run through different Meanings
                if (meaningNodes != null)
                    foreach (var meaningNode in meaningNodes)
                    {
                        classes.Meaning _meaning;
                        if (currentLanguage == "LU") _meaning = new classes.Meaning();
                        else _meaning = meanings[i];

                        //Default Translation, NOT LU
                        if (currentLanguage != "LU")
                        {
                            /*
                             * //div[@class='artikel']//div[@class='uds_block']
                             */


                            // Get All Examples to Cutout //div[@class='artikel']//div[@class='uds_block']/div[@class='bspsblock']
                        }
                    }

                if (currentLanguage == "LU")
                {
                    //get Examples
                }
            }









            return meanings;
        }
    }

    public class EmptyClass
    {
        public async Task<string> HttpRequest(string Lang, string XML)
        {
            string responseBody = null;
            int i = 0;

            HttpClient httpClient = new HttpClient();
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
                responseBody = await _response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            httpClient.Dispose();

            return responseBody;
        }
    }

    public class LodApiResults
    {
        public string searchString;
        public Dictionary <string, classes.WordBase> wordBase;

        public List<string> Results;


        public LodApiResults()
        {
            searchString = null;
            Results = new List<string>();
        }

        public LodApiResults(string strangeLodJson)
        {
            Results = new List<string>();

            if (strangeLodJson == null) return;
            strangeLodJson = DecodeEncodedNonAsciiCharacters(strangeLodJson);
            strangeLodJson = strangeLodJson.Replace(",", "");
            strangeLodJson = strangeLodJson.Replace("[", "");
            strangeLodJson = strangeLodJson.Replace("]", "");
            string[] splitted = @strangeLodJson.Split('"');
            List<string> clearedSplitted = new List<string>(); ;
            for (int i = 0; i < splitted.Length -1; i++)
            {
                if (splitted[i] != "") clearedSplitted.Add(splitted[i]);
            }

            searchString = clearedSplitted[0];
            if (clearedSplitted.Count>1)
            {
                //has results
                for (int i = 1; i< clearedSplitted.Count -1; i++)
                {
                    Results.Add(clearedSplitted[i]);
                }
            } 
        }

        public classes.WordOverview FillResults()
        {
            classes.WordOverview wo = new classes.WordOverview();

            foreach (string res in Results)
            {
                wo = FillResult(wo, res);
            }
            return wo;
        }

        public classes.WordOverview FillResults(classes.WordOverview wo)
        {
            foreach (string res in Results)
            {
                if (wo._wordPossibleMeanings.FirstOrDefault(x => x.occurence == res) != null )
                {
                    //word exists already
                } else
                {
                    wo = FillResult(wo, res);
                }
            }
            return wo;
        }

        public classes.WordOverview FillResult(classes.WordOverview wo, string res)
        {
            classes.Word word = new classes.Word(res);
            

            Task<string> task = Task.Run(async () => await GetXMLasync(res));
            task.Wait();
            string fetchedXml = task.Result;

            (List<string> xml, List<string> mp3, List<string> occ, Boolean valid) = ReturnBaseXmlData(fetchedXml);
            if (!valid)
            {
                return wo;
            }

            word.wordBasePointer = xml[0];

            Console.WriteLine("");

            insertBaseWords(xml, mp3, occ);

            Console.WriteLine("");

            wo._wordPossibleMeanings.Add(word);
            wo.valid = true;
            return wo;
        }

        private async Task<string> GetXMLasync(string Word)
        {
            return await Task.Run(() => GetXML(Word));
        }

        public async Task<string> GetXML(string Word)
        {
            string responseBody = null;
            HttpClient httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/lod-search.php?v=H&s=lu&w=" + Word.ToLower()), //Word has to be Lowercase
                Method = HttpMethod.Get,
                Headers =
                               {
                                  { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                                  { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                               }
            };
            try
            {
                var _response = await httpClient.SendAsync(httpContent);
                responseBody = await _response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            httpClient.Dispose();
            return responseBody;
        }

        public (List<string>, List<string>, List<string>, Boolean) ReturnBaseXmlData(string fetchedXml)
        {
            List<string> _xmlWords = new List<string>();
            List<string> _mp3s = new List<string>();
            List<string> _occ = new List<string>();

            HtmlAgilityPack.HtmlDocument XmlDocument = new HtmlAgilityPack.HtmlDocument();
            HtmlNode[] XmlNodes;
            XmlDocument.LoadHtml(fetchedXml);
            HtmlNodeCollection XmlTempNodes = XmlDocument.DocumentNode.SelectNodes("//a[@onclick]");
            if (XmlTempNodes == null)
            {
                //no parseable Item found
                return (_xmlWords, _mp3s, _occ, false);
            }
            XmlNodes = XmlTempNodes.ToArray();
            int _i = 1;
            string attrib = null;

            foreach (HtmlNode htmlNode in XmlNodes)
            {
                attrib = XmlDocument.DocumentNode.SelectNodes("//a[@onclick]")[_i - 1].GetAttributeValue("onclick", "default");
                string tmpxml = attrib.Remove(0, attrib.IndexOf("'") + 1); // result: PERSOUN1.xml','persoun1.mp3')
                string tmpmp3 = tmpxml.Remove(0, tmpxml.IndexOf("'") + 1); // result: ,'persoun1.mp3')
                tmpmp3 = tmpmp3.Remove(0, tmpmp3.IndexOf("'") + 1);

               string tmpWordForm = XmlDocument.DocumentNode.SelectSingleNode("//span[@class='s4']").InnerText.Trim();
               htmlNode.RemoveChild(XmlDocument.DocumentNode.SelectNodes("//span[@class='s4']").First());
                
                _i++;

                _occ.Add(htmlNode.InnerText.Trim());
                _xmlWords.Add(tmpxml.Substring(0, tmpxml.IndexOf("'")));
                _mp3s.Add(tmpmp3.Substring(0, tmpmp3.IndexOf("'")));
            }

                return (_xmlWords, _mp3s, _occ, true);
        }

        public async Task insertBaseWords(List<string> xml, List<string> mp3, List<string> occs)
        {
            int i = 0;
            foreach (string thisXml in xml)
            {
                classes.WordBase outer = new classes.WordBase();
                if (!wordBase.TryGetValue(thisXml, out outer))
                {
                    //doesn't exists, so at it
                    outer = new classes.WordBase(thisXml);
                    outer.baseMp3 = mp3[i];
                    outer.baseWordLu = occs[i];
                    wordBase.Add(thisXml, outer);

                    var taskFillWordBaseDetails = frmMainProgram.getInstance()._articleFile.article.fillWordBaseDetails(outer);
                    await taskFillWordBaseDetails;
                }
                i++;
            }
        }


        //  https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string/12563498
        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
