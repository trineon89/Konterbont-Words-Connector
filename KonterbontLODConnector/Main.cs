using System;
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
using System.IO;
using System.Diagnostics;

namespace KonterbontLODConnector
{
    public partial class frmMain : Form
    {
        public string MagazinePath = "\\\\192.168.1.75\\Konterbont_Produktioun\\Magazines\\";
        public string ArticlePath = "\\\\192.168.1.75\\Konterbont_Produktioun\\Artikelen\\";
        public Ookii.Dialogs.WinForms.VistaFolderBrowserDialog folderBrowser;
        public Ookii.Dialogs.WinForms.ProgressDialog progressDialog;
        private INDesignPlugin iNDesignPlugin;
        public DataHandler globaldt = null;

        public frmMain()
        {
            InitializeComponent();
            folderBrowser = new Ookii.Dialogs.WinForms.VistaFolderBrowserDialog
            {
                SelectedPath = ArticlePath
            };
            iNDesignPlugin = new INDesignPlugin();
        }

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
            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            return responseBody;
        }
        private async Task<AutoComplete> GetFullTranslationsAsync(string searchstring, bool compare = true)
        {
            return await Task.Run(() => GetFullTranslations(searchstring, compare));
        }

        private async Task<AutoComplete> GetFullTranslations(string searchstring, bool compare)
        {
            Task<string> task = Task.Run(async () => await FetchXMLasync(searchstring));
            task.Wait();
            string fetchedXml = task.Result;

            AutoComplete acwuert = ParseXMLWords(fetchedXml, compare, searchstring);

            Task<AutoComplete> taskLU = Task.Run(async () => await FetchFullWordsAsync(acwuert, "LU", !compare));
            taskLU.Wait();
            acwuert = taskLU.Result;
            Task<AutoComplete> taskDE = Task.Run(async () => await FetchFullWordsAsync(acwuert, "DE", !compare));
            taskDE.Wait();
            acwuert = taskDE.Result;
            Task<AutoComplete> taskFR = Task.Run(async () => await FetchFullWordsAsync(acwuert, "FR", !compare));
            taskFR.Wait();
            acwuert = taskFR.Result;
            Task<AutoComplete> taskEN = Task.Run(async () => await FetchFullWordsAsync(acwuert, "EN", !compare));
            taskEN.Wait();
            acwuert = taskEN.Result;
            Task<AutoComplete> pttask = Task.Run(async () => await FetchFullWordsAsync(acwuert, "PT", !compare));
            pttask.Wait();
            AutoComplete res = pttask.Result;
            return res;
        }

        private async Task<string> FetchXMLasync(string Word)
        {
            return await Task.Run(() => FetchXML(Word));
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

            // await Task.WhenAny(_responseT);
            _responseT.Wait();
            var _response = _responseT.Result;
            _response.EnsureSuccessStatusCode();

            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            return responseBody;
        }

        private async Task<AutoComplete> FetchFullWordsAsync(AutoComplete acwuert, string Lang, bool showselection = false)
        {
            AutoComplete reswuert = new AutoComplete();
            reswuert.Occurence = acwuert.Occurence;
            int _c = 1;
            int _MeaningsCount = 0;
            frmSelectMeaning frmSelectMeaning = new frmSelectMeaning();


            foreach (Wuert wuert in acwuert.Wierder)
            {
                string responseBody = await HttpRequest(Lang, wuert.XMLFile);

                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(responseBody);

                // Meanings START
                if (htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']") != null)
                {
                    int _i = 1;
                    string Selection = "";

                    if (Lang == "LU")
                    {
                        wuert.WuertForm.WuertForm = htmlDocument.DocumentNode.SelectSingleNode(".//span[@class='klass']").InnerText.Trim();
                    }

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
                            if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']") != null) //Failsafe pluriel
                            {
                                Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                                Pluriel = Pluriel.Replace("&lt;", "<");
                                Pluriel = Pluriel.Replace("&gt;", ">");
                            }
                            else
                                Pluriel = null;
                        }
                        else
                            Pluriel = null; // no base pluriel
                        wuert.WuertLuS = Pluriel;

                        Meaning meaning = new Meaning();

                        if (wuert.WuertForm.WuertForm == "Verb")
                        {
                            meaning.LU = wuert.WuertLu;
                            meaning.HV = Pluriel; // writes "Hëllefsverb" to class
                                                  // write PP to LUs variable
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

                            meaning.LUs = Pluriel;
                            wuert.WuertLuS = Pluriel;
                        }
                        else
                        {
                            if (Lang == "LU")
                            {
                                if (Meaning.SelectSingleNode(".//span[@class='text_gen']") != null)
                                { // Meaning 1 or 2 or 4
                                    meaning.LU = wuert.WuertLu;
                                    meaning.LUs = Pluriel;
                                    Console.WriteLine("Meaning 1, 2, 4");
                                    if (Meaning.SelectSingleNode(".//span[@class='text_gen'][1]").ChildNodes.Count() == 3)
                                    { // -> Meaning 1
                                        Console.WriteLine("Meaning 1");
                                        //meaning.LUs = null;
                                    }
                                    else
                                    {
                                        if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                                        { // Meaning 2 or 4
                                            Console.WriteLine("Meaning 2, 4");
                                            if (Meaning.SelectSingleNode(".//span[@class='info_flex']") != null)
                                            { // -> Meaning 2
                                                Console.WriteLine("Meaning 2");
                                                meaning.LUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
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
                                    if (Meaning.SelectSingleNode("span[@class='polylex']") != null)
                                    { // -> Meaning 3
                                        Console.WriteLine("Meaning 3");
                                        meaning.LU = Meaning.SelectSingleNode("span[@class='polylex']").InnerText;
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
                            MeaningText = System.Text.RegularExpressions.Regex.Replace(MeaningText, regex, "");

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
                                    /*string EGS = "";
                                    if (htmlexample.SelectSingleNode(".//span[@id='text_gen']") != null)
                                    {
                                        EGS = htmlexample.SelectSingleNode(".//span[@id='text_gen']").InnerText;
                                    }
                                    RemoveNode = htmlexample.SelectSingleNode(".//span[@id='text_gen']");
                                    if (RemoveNode != null)
                                    {
                                        htmlexample.RemoveChild(RemoveNode);
                                    }*/
                                    Example example = new Example(htmlexample.InnerText);
                                    
                                    //Console.WriteLine(EGS);
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
                        Selection = (_i + 1).ToString();

                        // Wuert wuert = acwuert.Wierder[acwuert.Selection - 1];
                        if (showselection && acwuert.Selection == _c)
                        {
                            string thename = _i.ToString();
                            RadioButton rb = new RadioButton
                            {
                                Name = thename,
                                Text = thename + " " + MeaningText + "" + MeaningTextAdd,

                                Location = new Point(10, _i * 30),
                                Width = 500
                            };
                            if (_i == 1)
                            {
                                rb.Checked = true;
                            }
                            frmSelectMeaning.gbMeanings.Text = "Bedeitung fir '" + wuert.WuertLu + "' auswielen:";
                            frmSelectMeaning.gbMeanings.Controls.Add(rb);
                            _MeaningsCount = htmlNodes.Count();
                        }

                        if (Lang == "LU")
                        { wuert.Meanings.Add(meaning); }
                        _i++;
                    }
                }
                if (_MeaningsCount > 1 && Lang == "DE")
                {
                    if (frmSelectMeaning.ShowDialog() == DialogResult.OK)
                    {
                        RadioButton selectedMeaning = frmSelectMeaning.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                        string Selection = selectedMeaning.Name;
                        wuert.Selection = Int32.Parse(Selection);
                        //TheResults.Wierder[TheResults.Selection].Selection = Int32.Parse(Selection);
                    }
                }

                reswuert.Wierder.Add(wuert);
                _c++;
            }
            reswuert.Selection = acwuert.Selection;
            return reswuert;
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

            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();

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
                        if (Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']") != null) //Failsafe pluriel
                        {
                            Pluriel = Meaning.SelectSingleNode("//div[@class='artikel']/span[@class='text_gen']/span[@class='mentioun_adress']").InnerText;
                            Pluriel = Pluriel.Replace("&lt;", "<");
                            Pluriel = Pluriel.Replace("&gt;", ">");
                        }
                        else
                            Pluriel = null;
                    }
                    else
                        Pluriel = null; // no base pluriel
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
                        }
                        else if (Meaning.SelectSingleNode(".//span[@class='mentioun_adress']") != null)
                        { // Meaning 2 or 4
                            Console.WriteLine("Meaning 2, 4");
                            if (Meaning.SelectSingleNode(".//span[@class='info_flex']") != null)
                            { // -> Meaning 2
                                Console.WriteLine("Meaning 2");
                                meaning.LUs = Meaning.SelectSingleNode(".//span[@class='mentioun_adress']").InnerText;
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
                    else
                    {
                        if (Meaning.SelectSingleNode("span[@class='polylex']") != null)
                        { // -> Meaning 3
                            Console.WriteLine("Meaning 3");
                            meaning.LU = Meaning.SelectSingleNode("span[@class='polylex']").InnerText;
                            meaning.LUs = wuert.WuertLuS;
                        }
                        else
                        { // -> Meaning 4 safe
                            Console.WriteLine("Meaning 4 (safe)");
                            meaning.LUs = wuert.WuertLuS;
                            meaning.LU = wuert.WuertLu;
                        }
                    }
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
                    }

                    switch (Lang)
                    {
                        case "LU":
                            meaning.MP3 = wuert.MP3;
                            HtmlNodeCollection htmlExamples = Meaning.SelectNodes(".//span[@class='beispill']");
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

                    Selection = (_i + 1).ToString();
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
                    frmSelectMeaning.gbMeanings.Text = "Bedeitung fir '" + wuert.WuertLu + "' auswielen:";
                    frmSelectMeaning.gbMeanings.Controls.Add(rb);

                    if (Lang == "LU")
                    { wuert.Meanings.Add(meaning); }
                    _i++;
                }
                if (htmlNodes.Count() > 1 && Lang == "DE")
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

            var _response = await httpClient.SendAsync(httpContent);
            _response.EnsureSuccessStatusCode();
            string responseBody = await _response.Content.ReadAsStringAsync();
            httpClient.Dispose();
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            // Meanings START
            if (Lang == "DE")
            {
                int _i = 1;
                string Selection = null;
                string MeaningsTT = null;
                HtmlNodeCollection MeaningsDE = htmlDocument.DocumentNode.SelectNodes("//div[@class='uds_block']");

                if (MeaningsDE == null) // keen normale Fall, Variant
                {
                    HtmlNode Node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='artikel']");
                    var RemoveNode = Node.SelectSingleNode("//span[@class='mentioun_adress']");
                    if (RemoveNode != null)
                        Node.RemoveChild(RemoveNode);
                    RemoveNode = Node.SelectSingleNode("//span[@class='klass']");
                    if (RemoveNode != null)
                        Node.RemoveChild(RemoveNode);
                    RemoveNode = Node.SelectSingleNode("//div[@class='lux.map']");
                    if (RemoveNode != null)
                        Node.RemoveChild(RemoveNode);

                    //Meaning.RemoveChild(RemoveNode);
                    MeaningsTT = Node.InnerText;
                    return MeaningsTT;
                }

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
                            if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null)
                            { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                            MeaningText = htmlNode.SelectSingleNode("span[@class='et']").InnerText;
                            MeaningTextAdd = htmlNode.SelectSingleNode("span[@class='text_gen']").InnerText;
                        }
                        else
                        {
                            if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null)
                            { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                            MeaningArray = htmlNode.SelectNodes(".//span[@class='et']").ToArray();

                            for (int _m = 0; _m < MeaningArray.Length; _m++)
                            {
                                MeaningText = MeaningText + MeaningArray[_m].InnerText;
                                if (_m < MeaningArray.Length - 1)
                                { MeaningText = MeaningText + ", "; }
                            }
                        }
                    }
                    else
                    {
                        if (htmlNode.SelectSingleNode("span[@class='uds_num']") != null)
                        { MeaningNr = htmlNode.SelectSingleNode("span[@class='uds_num']").InnerText; }
                        MeaningText = htmlNode.SelectSingleNode("span[@class='et']").InnerText;
                    }
                    Selection = _i.ToString();

                    MeaningsTT = MeaningsTT + " " + MeaningNr + " " + MeaningText + "" + MeaningTextAdd + Environment.NewLine;
                }
                return MeaningsTT;
                ;
            }
            return null;
        }

        private async void BtnFetch_ClickAsync(object sender, EventArgs e)
        {
            Task<string> task = Task.Run(async () => await FetchXMLasync(edtWord.Text));
            task.Wait();
            string fetchedXml = task.Result;

            AutoComplete acwuert = ParseXMLWords(fetchedXml);

            acwuert = await Task.Run(async () => await FetchWordsAsync(acwuert, "LU"));
            acwuert = await Task.Run(async () => await FetchWordsAsync(acwuert, "DE"));
            acwuert = await Task.Run(async () => await FetchWordsAsync(acwuert, "FR"));
            acwuert = await Task.Run(async () => await FetchWordsAsync(acwuert, "EN"));
            acwuert = await Task.Run(async () => await FetchWordsAsync(acwuert, "PT"));
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


        private AutoComplete ParseXMLWords(string XML, bool onlycompare = false, string occurence = null)
        {
            AutoComplete ac = new AutoComplete();
            ac.Occurence = occurence;

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
                    //WuertForm = new WordForm(tmpWordForm),
                    WuertForm = new WordForm(""),
                    Selection = 1,
                    XMLFile = tmpxml.Substring(0, tmpxml.IndexOf("'")),
                    MP3 = tmpmp3.Substring(0, tmpmp3.IndexOf("'"))
                };

                ac.Wierder.Add(wuert);

                RadioButton rb = new RadioButton
                {
                    Name = _i.ToString(),
                    //Text = wuert.WuertLu + " (" + wuert.WuertForm.WuertForm + ")",
                    Text = wuert.WuertLu + " (" + tmpWordForm + ")",
                    Location = new Point(10, _i * 30),
                    Width = 500
                };
                if (_i == 1)
                { rb.Checked = true; }

                Task<string> task = Task.Run(async () => await GetSelectionTooltipAsync(wuert.XMLFile));
                task.Wait();
                string tooltip = task.Result;

                frm.gbMeanings.Controls.Add(rb);
                frm.gbMeanings.Text = "Wuert auswielen:";
                frm.Text = "Wuert auswielen";
                frm.tpInfo.SetToolTip(rb, tooltip);

                _i++;
            }

            if (onlycompare)
                return ac;

            if (htmlNodes.Count() > 1)
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RadioButton radioButton = frm.gbMeanings.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    ac.Selection = Int32.Parse(radioButton.Name);
                }
            }
            else
            {
                ac.Selection = 1;
            }
            return ac;
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

        private async Task<Boolean> CheckIfWordHasChangedAsync(string searchstring, List<AutoComplete> ac)
        {
            return await Task.Run(() => CheckIfWordHasChanged(searchstring, ac));
        }

        private async Task<Boolean> CheckIfWordHasChanged(string searchstring, List<AutoComplete> ac)
        {
            AutoComplete acresults = await Task.Run(async () => await GetFullTranslationsAsync(searchstring));

            var tmp = ac.FirstOrDefault(acx => acx.Wierder.Any(x => acresults.Wierder.Any(b => (b.WuertLu == x.WuertLu) && (b.MP3 == x.MP3) && (b.WuertForm.WuertForm == x.WuertForm.WuertForm) && (b.XMLFile == x.XMLFile) && (acx.Occurence == searchstring))));

            if (tmp == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private async Task<AutoComplete> GetWordAsync(string searchstring)
        {
            return await Task.Run(() => GetWord(searchstring));
        }

        private async Task<AutoComplete> GetWord(string searchstring)
        {
            string fetchedXml = await Task.Run(async () => await FetchXMLasync(searchstring));

            AutoComplete acwuert = ParseXMLWords(fetchedXml, false, searchstring);
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

            return acwuert;
        }

        private async void ArtikelOpmaachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Add Progress Bar?
            //Menu -> Artikel Opmaachen
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources"))
                {
                    try
                    {
                        Directory.Delete(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources", true);
                    }
                    catch (Exception ea)
                    {
                        Debug.WriteLine("{0} Exception caught.", ea);
                    }
                }
                Directory.CreateDirectory(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources");
                string[] files = Directory.GetFiles(folderBrowser.SelectedPath, "*.words");
                foreach (var file in files)
                {
                    string tmpfilename = Path.GetFileNameWithoutExtension(file) + ".wordslist";

                    DataHandler dt = new DataHandler();
                    ;

                    if (File.Exists(folderBrowser.SelectedPath + "\\" + tmpfilename))
                    {
                        dt = dt.LoadFromFile(folderBrowser.SelectedPath, tmpfilename);
                    }
                    else
                    {
                        dt = new DataHandler(tmpfilename, folderBrowser.SelectedPath + "\\");
                    }

                    if (!(File.Exists(folderBrowser.SelectedPath + dt.QuickSelectFile)))
                    {
                        File.CreateText(folderBrowser.SelectedPath + dt.QuickSelectFile).Dispose();
                    }
                    else
                    {
                        string tmppath = folderBrowser.SelectedPath + dt.QuickSelectFile;
                        dt.LoadQuickSelect(File.ReadLines(tmppath));
                    }

                    string tfile = new StreamReader(file).ReadToEnd();
                    string[] lines = tfile.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    int countlines = lines.GetLength(0);
                    int c = 0;
                    Pietschsoft.NativeProgressDialog progressDialog = new Pietschsoft.NativeProgressDialog(this.Handle)
                    {
                        Title = "Sichen Wierder um LOD...",
                        CancelMessage = "Eieiei... Da wart elo...",
                        Maximum = 100,
                        Value = 0,
                        Line3 = "Calculating Time Remaining..."
                    };
                    progressDialog.ShowDialog(Pietschsoft.NativeProgressDialog.PROGDLG.Modal, Pietschsoft.NativeProgressDialog.PROGDLG.AutoTime, Pietschsoft.NativeProgressDialog.PROGDLG.NoMinimize);
                    dt.PrepareOutputFolder();
                    foreach (string line in lines)
                    {
                        progressDialog.Line1 = "Siche nom Wuert: " + line;

                        if (dt.WordList.Count == 0)
                        {
                            AutoComplete acword = await Task.Run(async () => await GetFullTranslationsAsync(line, false));
                            dt.AddWordToList(acword);
                        }
                        else
                        {
                            bool HasChanged = await Task.Run(async () => await CheckIfWordHasChangedAsync(line, dt.WordList));
                            if (HasChanged)
                            {
                                AutoComplete acword = await Task.Run(async () => await GetFullTranslationsAsync(line, false));
                                dt.AddWordToList(acword);
                            }
                        }
                        double dbl = 100d / countlines * c;
                        uint _currprog = Convert.ToUInt32(Math.Round(dbl));
                        progressDialog.Line2 = "Oofgeschloss zu " + _currprog.ToString() + "%";
                        progressDialog.Value = _currprog;
                        c++;
                        //dt.OutputPopup(dt.WordList[c-1].Wierder[dt.WordList[c-1].Selection -1], line, "240,120,84");
                    }
                    progressDialog.CloseDialog();

                    dt.SaveToFile(dt);

                    lbWords.Items.Clear();
                    foreach (AutoComplete ac in dt.WordList) // Adds Words to lbWords on Main Form
                    {
                        //lbWords.Items.Add(ac.Wierder[ac.Selection - 1].WuertLu);
                        lbWords.Items.Add(ac.Occurence);
                    }
                    globaldt = dt;
                    lbWords.SelectedIndex = 0;
                }
            }
        }

        private void LbWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ✓ 
            var _i = 0;
            lbSelectWord.Items.Clear();

            foreach (Wuert SelWuert in globaldt.WordList[lbWords.SelectedIndex].Wierder)
            {
                if (globaldt.WordList[lbWords.SelectedIndex].Selection - 1 == _i)
                //if (SelWuert.Selection - 1 == _i)
                {
                    lbSelectWord.Items.Add(SelWuert.WuertLu + " ✓");
                    lbSelectWord.SelectedIndex = _i;

                }
                else
                {
                    lbSelectWord.Items.Add(SelWuert.WuertLu);
                }
                _i++;
            }
        }

        private void LbSelectWord_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ✓ 
            var _i = 0;
            lbSelectMeaning.Items.Clear();

            foreach (Meaning SelMeaning in globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings)
            {
                if (globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Selection - 1 == _i)
                {
                    lbSelectMeaning.Items.Add(SelMeaning.DE.Trim() + " ✓");
                    lbSelectMeaning.SelectedIndex = _i;
                }
                else
                {
                    lbSelectMeaning.Items.Add(SelMeaning.DE.Trim());
                }
                _i++;
            }
        }

        private void LbSelectMeaning_SelectedIndexChanged(object sender, EventArgs e)
        {
            rtbDetails.Clear();
            Wuert SelWord = globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex];
            Meaning SelMeaning = globaldt.WordList[lbWords.SelectedIndex].Wierder[lbSelectWord.SelectedIndex].Meanings[lbSelectMeaning.SelectedIndex];

            Font Bold = new Font(rtbDetails.SelectionFont, FontStyle.Bold);
            Font Normal = new Font(rtbDetails.SelectionFont, FontStyle.Regular);
            Font Italic = new Font(rtbDetails.SelectionFont, FontStyle.Italic);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText("LU: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.LU);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "Wuertform: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelWord.WuertForm.WuertForm);

            rtbDetails.SelectionFont = Bold;
            if (SelWord.WuertForm.WuertForm == "Verb")
            {
                rtbDetails.AppendText(Environment.NewLine + "participe passé: ");

                if (SelMeaning.LUs != null)
                {
                    rtbDetails.SelectionFont = Normal;
                    string tmpPluriel = SelMeaning.LUs;
                    tmpPluriel = tmpPluriel.Replace("&lt;", "<");
                    tmpPluriel = tmpPluriel.Replace("&gt;", ">");
                    rtbDetails.AppendText(tmpPluriel);
                }
                else
                {
                    rtbDetails.SelectionFont = Italic;
                    rtbDetails.AppendText("(Kee Pluriel)");
                }
                rtbDetails.SelectionFont = Bold;
                rtbDetails.AppendText(Environment.NewLine + "Hëllefsverb: ");
                rtbDetails.SelectionFont = Normal;
                rtbDetails.AppendText(SelMeaning.HV);

            }
            else
            {
                if (SelWord.WuertForm.WuertForm.Contains("Substantiv"))
                {
                    rtbDetails.AppendText(Environment.NewLine + "Pluriel: ");

                    if (SelMeaning.LUs != null)
                    {
                        rtbDetails.SelectionFont = Normal;
                        string tmpPluriel = SelMeaning.LUs;
                        tmpPluriel = tmpPluriel.Replace("&lt;", "<");
                        tmpPluriel = tmpPluriel.Replace("&gt;", ">");
                        rtbDetails.AppendText(tmpPluriel);
                    }
                    else
                    {
                        rtbDetails.SelectionFont = Italic;
                        rtbDetails.AppendText("(Kee Pluriel)");
                    }
                }
            }



            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "DE: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.DE);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "FR: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.FR);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "EN: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.EN);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "PT: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.PT);

            rtbDetails.SelectionFont = Bold;
            rtbDetails.AppendText(Environment.NewLine + "MP3: ");
            rtbDetails.SelectionFont = Normal;
            rtbDetails.AppendText(SelMeaning.MP3);


            var Ex = 1;
            foreach (Example SelExample in SelMeaning.Examples)
            {
                rtbDetails.SelectionFont = Bold;
                rtbDetails.AppendText(Environment.NewLine + "Beispill " + Ex.ToString() + ": ");
                rtbDetails.SelectionFont = Normal;
                rtbDetails.AppendText(SelExample.ExampleText);
                Ex++;
            }

        }
    }
}
