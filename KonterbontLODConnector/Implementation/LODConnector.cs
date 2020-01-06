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

            insertBaseWords(xml, mp3);

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

        private void insertBaseWords(List<string> xml, List<string> mp3)
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
                    wordBase.Add(thisXml, outer);
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
