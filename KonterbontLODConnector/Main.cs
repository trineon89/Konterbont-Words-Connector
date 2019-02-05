﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using HtmlAgilityPack;

namespace KonterbontLODConnector
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }


        /* Print Class https://stackoverflow.com/questions/19823726/c-sharp-how-to-output-all-the-items-in-a-classstruct  */
        private void PrintProperties(Meaning myObj)
        {
            foreach (var prop in myObj.GetType().GetProperties())
            {
                Console.WriteLine(prop.Name + ": " + prop.GetValue(myObj, null));
            }

            foreach (var field in myObj.GetType().GetFields())
            {
                Console.WriteLine(field.Name + ": " + field.GetValue(myObj));
            }
        }
        /* Print Class End */

        private async Task<string> FetchXML(string Word)
        {
            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.lod.lu/php/lod-search.php?v=H&s=lu&w=" + Word),
                Method = HttpMethod.Get,
                Headers =
                           {
                              { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                              { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                           }
            };
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            return responseBody;
        }

        private async Task<string> FetchXMLasync(string Word)
        {
            return await Task.Run(() => FetchXML(Word));
        }

        private async Task<AutoComplete> FetchWordsAsync(AutoComplete acwuert, string Lang)
        {
            Wuert wuert = acwuert.Wierder[acwuert.Selection - 1];
            var httpClient = new HttpClient();
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
                RequestUri = new Uri("https://www.lod.lu/php/getart" + LangURL + ".php?artid=" + wuert.XMLFile),
                Method = HttpMethod.Get,
                Headers =
                           {
                              { HttpRequestHeader.Host.ToString(), "www.lod.lu" },
                              { HttpRequestHeader.Referer.ToString(), "https://www.lod.lu/" }
                           }
            };

            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            // Meanings START
            if (htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
            {
                int _i = 1;
                string Selection = "";
                string MeaningNr = "";
                frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();

                HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']");
                foreach (HtmlNode Meaning in htmlNodes)
                {
                    string MeaningText = "";
                    string MeaningTextAdd = "";
                    string Pluriel;

                    //HtmlNode[] MeaningArray;
                    HtmlNode[] LUsArray = htmlDocument.DocumentNode.SelectNodes("//span[@class='mentioun_adress']").ToArray();

                    if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']") != null) //has base pluriel
                    {
                        Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                    } else Pluriel = null; // no base pluriel
                    wuert.WuertLuS = Pluriel;

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

                    Meaning meaning = new Meaning();

                    if (Meaning.SelectSingleNode(".//span[@class='text_gen']") != null)
                    { // Meaning 1 or 2 or 4
                        meaning.LU = wuert.WuertLu;
                        meaning.LUs = Pluriel;
                        Console.WriteLine("Meaning 1, 2, 4");
                        if (Meaning.SelectSingleNode(".//span[@class='text_gen'][1]").ChildNodes.Count() == 3)
                        { // -> Meaning 1
                            Console.WriteLine("Meaning 1");
                            meaning.LUs = null;
                        } else if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                        { // Meaning 2 or 4
                            Console.WriteLine("Meaning 2, 4");
                            if (Meaning.SelectSingleNode(".//span[@class='info_flex']") != null)
                            { // -> Meaning 2
                                Console.WriteLine("Meaning 2");
                                meaning.LUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
                            } else
                            { // -> Meaning 4
                                Console.WriteLine("Meaning 4");
                            }
                        } else
                        { // -> Meaning 1
                            Console.WriteLine("Meaning 1");
                            meaning.LUs = null;
                        }
                    } else
                    {
                        if (Meaning.SelectSingleNode("span[@class='polylex']") != null)
                        { // -> Meaning 3
                            Console.WriteLine("Meaning 3");
                            meaning.LU = Meaning.SelectSingleNode("span[@class='polylex']").InnerText;
                            meaning.LUs = wuert.WuertLuS;
                        } else
                        { // -> Meaning 4 safe
                            Console.WriteLine("Meaning 4 (safe)");
                            meaning.LUs = wuert.WuertLuS;
                            meaning.LU = wuert.WuertLu;
                        }
                    }
                    if (Lang != "LU")
                    {
                        if (Meaning.SelectSingleNode(".//span[@class='et']") != null)
                        {
                            MeaningText = Meaning.SelectSingleNode(".//span[@class='et']").InnerText;
                        }
                        if (Meaning.SelectSingleNode(".//span[@class='text_gen']") != null)
                        {
                            if (Meaning.SelectSingleNode(".//span[@class='text_gen']").InnerText.Contains("["))
                            {
                                var tmp = Meaning.SelectSingleNode(".//span[@class='text_gen']").InnerText;
                                MeaningText = MeaningText + tmp;                             }
                            //MeaningText;
                        }
                    }

                    switch (Lang)
                    {
                        case "LU":
                            meaning.MP3 = wuert.MP3;
                            HtmlNodeCollection htmlExamples = htmlDocument.DocumentNode.SelectNodes("//span[@class='beispill']");
                            foreach (HtmlNode htmlexample in htmlExamples)
                            {
                                Example example = new Example(htmlexample.InnerText);
                                meaning.Examples.Add(example);
                            }
                            break;
                        case "DE":
                            wuert.Meanings[_i - 1].DE = MeaningText + MeaningTextAdd;
                            break;
                        case "FR":
                            wuert.Meanings[_i - 1].FR = MeaningText + MeaningTextAdd;
                            break;
                        case "EN":
                            wuert.Meanings[_i - 1].EN = MeaningText + MeaningTextAdd;
                            break;
                        case "PT":
                            wuert.Meanings[_i - 1].PT = MeaningText + MeaningTextAdd;
                            break;
                    }

                    Selection = _i.ToString();
                    RadioButton rb = new RadioButton
                    {
                        Name = _i.ToString(),
                        Text = MeaningNr + " " + MeaningText + "" + MeaningTextAdd,
                        Location = new Point(10, _i * 30),
                        Width = 500
                    };
                    if (_i == 1)
                    {
                        rb.Checked = true;
                    }
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);

                    if (Lang == "LU") { wuert.Meanings.Add(meaning); }
                    _i++;
                }
                if (htmlNodes.Count() > 1 && Lang =="PT")
                {
                    if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                    {
                        RadioButton selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                        Selection = selectedMeaning.Name;
                        wuert.Selection = Int32.Parse(Selection);
                        //TheResults.Wierder[TheResults.Selection].Selection = Int32.Parse(Selection);
                    }
                }
            }
            acwuert.Wierder[acwuert.Selection - 1] = wuert;
            return acwuert;
            //return wuert;
        }

        private async Task<string> FetchWordsTT(string XML, string Lang)
        {
            var httpClient = new HttpClient();
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

            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            // Meanings START
            if (Lang == "DE")
            {
                int _i = 1;
                string Selection = null;
                string MeaningsTT = null;
                HtmlNodeCollection MeaningsDE = htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']");
                
                foreach (HtmlNode htmlNode in MeaningsDE)
                {
                    string MeaningNr = null;
                    string MeaningText = null;
                    string MeaningTextAdd = null;
                    HtmlNode[] MeaningArray;

                    if (htmlNode.SelectSingleNode("span[@class='text_gen']") != null)
                    {
                        if (htmlNode.SelectSingleNode("span[@class='text_gen']").InnerText.Contains("["))
                        {
                            if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null) { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                            MeaningText = htmlNode.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = htmlNode.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null) { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                            MeaningArray = htmlNode.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1) { MeaningText = MeaningText + ", "; }
                            }
                        }
                    } else
                    {
                        if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null) { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                        MeaningText = htmlNode.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();

                    MeaningsTT = MeaningsTT + " " + MeaningNr + " " + MeaningText + "" + MeaningTextAdd + Environment.NewLine;
                }
                return MeaningsTT;
            }
            return null;
        }

        private void btnFetch_ClickAsync(object sender, EventArgs e)
        {
            Task<string> task = Task.Run(async () => await FetchXMLasync(edtWord.Text));
            task.Wait();
            string fetchedXml = task.Result;

            AutoComplete acwuert = ParseXMLWords(fetchedXml);

            Task<AutoComplete> taskLU = Task.Run(async () => await FetchWordsAsync(acwuert, "LU"));
            taskLU.Wait();
            acwuert = taskLU.Result;
            Task<AutoComplete> taskDE = Task.Run(async () => await FetchWordsAsync(acwuert, "DE"));
            taskDE.Wait();
            acwuert = taskDE.Result;
            Task<AutoComplete> taskFR = Task.Run(async () => await FetchWordsAsync(acwuert, "FR"));
            taskFR.Wait();
            acwuert = taskFR.Result;
            Task<AutoComplete> taskEN = Task.Run(async () => await FetchWordsAsync(acwuert, "EN"));
            taskEN.Wait();
            acwuert = taskEN.Result;
            Task<AutoComplete> taskPT = Task.Run(async () => await FetchWordsAsync(acwuert, "PT"));
            taskPT.Wait();
            acwuert = taskPT.Result;

            DataHandler dt = new DataHandler("test.qs", "K:\\Artikelen\\");
            dt.AddWordToList(acwuert);
            dt.SaveToFile(dt);$

            /*
            GetMeanings(TheResults.Selection);
            */
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired)
                c.Invoke(new MethodInvoker(delegate
                { a(); }));
            else
                return false;

            return true;
        }
        

        private AutoComplete ParseXMLWords(string XML)
        {
            AutoComplete ac = new AutoComplete();

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            HtmlNode[] htmlNodes;

            htmlDocument.LoadHtml(XML);
            htmlNodes = htmlDocument.DocumentNode.SelectNodes("//a[@onclick]").ToArray();

            frmSelectMeaning frm = new frmSelectMeaning();

            int _i = 1;
            string attrib = null;

            foreach (HtmlNode htmlNode in htmlNodes)
            {
                attrib = htmlDocument.DocumentNode.SelectNodes("//a[@onclick]")[_i - 1].GetAttributeValue("onclick", "default");
                string tmpxml = attrib.Remove(0, attrib.IndexOf("'") + 1); // result: PERSOUN1.xml','persoun1.mp3')
                string tmpmp3 = tmpxml.Remove(0, tmpxml.IndexOf("'") + 1); // result: ,'persoun1.mp3')
                tmpmp3 = tmpmp3.Remove(0, tmpmp3.IndexOf("'") + 1);

                string tmpWordForm = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='s4']").InnerText.Trim();
                htmlNode.RemoveChild(htmlDocument.DocumentNode.SelectNodes("//span[@class='s4']").First());

                Wuert wuert = new Wuert
                {
                    WuertLu = htmlNode.InnerText.Trim(),
                    WuertForm = new WordForm(tmpWordForm),
                    Selection = 0,
                    XMLFile = tmpxml.Substring(0, tmpxml.IndexOf("'")),
                    MP3 = tmpmp3.Substring(0, tmpmp3.IndexOf("'"))
                };

                ac.Wierder.Add(wuert);

                RadioButton rb = new RadioButton
                {
                    Name = _i.ToString(),
                    Text = wuert.WuertLu + " (" + wuert.WuertForm.WuertForm + ")",
                    Location = new Point(10, _i * 30),
                    Width = 500
                };
                if (_i == 1) { rb.Checked = true; }
              
                Task<string> task = Task.Run(async () => await GetSelectionTooltipAsync(wuert.XMLFile));
                task.Wait();
                string tooltip = task.Result;

                frm.gbMeanings.Controls.Add(rb);
                frm.gbMeanings.Text = "Wuert auswielen:";
                frm.Text = "Wuert auswielen";
                frm.tpInfo.SetToolTip(rb, tooltip);

                _i++;
            }

           if (htmlNodes.Count() > 1)
           {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RadioButton radioButton = frm.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    ac.Selection = Int32.Parse(radioButton.Name);
                }
           } else {
                ac.Selection = 1;
           }
            return ac;
            //return ac.Wierder[ac.Selection - 1];
        }
       

        private async Task<string> GetSelectionTooltipAsync(string XMLTT)
        {
            return await Task.Run(() => GetSelectionTooltip(XMLTT));
        }

        private async Task<string> GetSelectionTooltip(string XMLTT)
        {
            string tooltip = null;

            string tooltipLU = await Task.Run(() => FetchWordsTT(XMLTT, "LU"));
            string tooltipDE = await Task.Run(() => FetchWordsTT(XMLTT, "DE"));
            string tooltipFR = await Task.Run(() => FetchWordsTT(XMLTT, "FR"));

            tooltip = tooltipLU + tooltipDE + tooltipFR;

            return tooltip;
        }
        /*
            private void GetXMLTT(HtmlNode XMLTT)
        {
            // string onClickVal = "";
            var a = new HtmlAgilityPack.HtmlDocument();

            a.LoadHtml(XMLTT.OuterHtml);

            string onClickVal = a.DocumentNode.SelectSingleNode("//a[@onclick]").GetAttributeValue("onclick", "default"); //Bsp: getart('PERSOUN1.xml','persoun1.mp3')

            // get XML and MP3 from Word search
            var QuotePos = onClickVal.IndexOf("'");
            onClickVal = onClickVal.Remove(0, QuotePos + 1); // result: PERSOUN1.xml','persoun1.mp3')

            QuotePos = onClickVal.IndexOf("'");
            SearchXMLTT = onClickVal.Substring(0, QuotePos);   // result: PERSOUN1.xml

            Task.WaitAll(Task.Run(async () => await FetchWordsTT(SearchXMLTT, "LU")));
            Task.WaitAll(Task.Run(async () => await FetchWordsTT(SearchXMLTT, "DE")));
            Task.WaitAll(Task.Run(async () => await FetchWordsTT(SearchXMLTT, "FR")));
            GetMeaningsTT();
        }
        */
    }
}
