using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Konterbont_Words_Connector
{
    public partial class frmMain : Form
    {
        public static readonly HttpClient client = new HttpClient();
        public HttpResponseMessage response;
        public string responseString;
        public string filename;
        public string SourcePath = "\\\\192.168.1.75\\Infomedia\\_KB_SCRIPT\\COPYCONTENT\\";
        public string targetMagazine = "";
        public string MagazinePath = "\\\\192.168.1.75\\Konterbont_Produktioun\\Magazines\\";
        public string ArticlePath = "\\\\192.168.1.75\\Konterbont_Produktioun\\Artikelen\\";
        public string QuickSelectFile = "meanings.quickselect";
        public Dictionary<short, String> QuickSelect = new Dictionary<short, string>();
        public string CurrentPath;
        public string charcolor;
        public FileIniDataParser IniParser;
        public string IniFileName = "Config.ini";
        public frmMagazineSelector theForm;
        public frmCloseDialog theCloseDialog;
        public Ookii.Dialogs.VistaFolderBrowserDialog folderBrowser;
        public bool CopyClicked = true;

        public string TargetMagazine { get => targetMagazine; set => targetMagazine = value; }

        public frmMain()
        {
            InitializeComponent();
            lbNotFound.Visible = false;
            IniParser = new FileIniDataParser();
            folderBrowser = new Ookii.Dialogs.VistaFolderBrowserDialog();

            if (File.Exists(IniFileName))
            {
                IniData data = IniParser.ReadFile(IniFileName);
                /*
                 var _magazineValue = data["MAGAZINE"].GetKeyData("mag").Value;
                if (_magazineValue != "")
                {
                    theCombo.SelectedIndex = theCombo.FindStringExact(_magazineValue);
                }
                */
            }


            //folderBrowserDialog1.SelectedPath = ArticlePath;
            folderBrowser.SelectedPath = ArticlePath;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            //
            
           // if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
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

                string[] files = Directory.GetFiles(folderBrowser.SelectedPath, "*.txt");
                foreach (var file in files)
                {
                    CopyClicked = false;
                    QuickSelect.Clear();
                    Debug.WriteLine("okay clicked");
                    //
                    CurrentPath = folderBrowser.SelectedPath + "\\";

                    filename = Path.GetFileNameWithoutExtension(file).Substring(0, 7) + "_";

                    QuickSelectFile = filename + "qs.quickselect";

                    if (!(File.Exists(folderBrowser.SelectedPath + "\\" + QuickSelectFile)))
                    {
                        File.CreateText(folderBrowser.SelectedPath + "\\" + QuickSelectFile).Dispose();
                    }
                    else
                    {
                        QuickSelect = File.ReadLines(folderBrowser.SelectedPath + "\\" + QuickSelectFile)
                            .Select(sline => sline.Split('	'))
                            .ToDictionary(split => Int16.Parse(split[0]), split => split[1]);
                    }

                    lbOccurences.Items.Clear();
                    lbValues.Items.Clear();
                    lbNotFound.Items.Clear();

                    lblArticle.Text = filename.Substring(0,4);
                    StreamReader reader = File.OpenText(folderBrowser.SelectedPath + "\\" + filename + ".txt");
                
                    Debug.WriteLine("read start---");
                    string line;
                    var values = new Dictionary<string, string>();
                    int i = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                    
                        if (line!= "_wierder_	")
                        {
                            if (i == 0)
                            {
                                charcolor = line;
                            }
                            else
                            {
                                Debug.WriteLine("L: " + line);
                                values.Add(i.ToString(), line);
                                lbValues.Items.Add(line);
                            }
                            i++;
                        }
                    }
                    reader.Close();
                    lblValues.Text = lbValues.Items.Count.ToString();
                    var content = new FormUrlEncodedContent(values);

                    //PostAsync(content);

                    response = await client.PostAsync("https://www.autisme.lu/kbentitlement/inddjsx/getTheWords.json.php", content);

                    responseString = await response.Content.ReadAsStringAsync();

                    List<Occurence> Occurence = JsonConvert.DeserializeObject<List<Occurence>>(responseString);

                    Debug.WriteLine("read end---");
                    if (Occurence != null)
                    {

                        // CHECK IF NUMS
                        bool error = false;
                        foreach (var theOccurence in Occurence)
                        {
                            if (theOccurence.Translations.Count==0)
                            {
                                error = true;
                                Debug.Print("Error");
                                lbNotFound.Items.Add(theOccurence.WordOccurence.ToString());
                            } else
                            {
                                lbOccurences.Items.Add(theOccurence.WordOccurence.ToString());
                            }
                        }
                        if (error)
                        {
                            lbNotFound.Visible = true;
                            string message = "Ups!\r\nFolgend Wierder sinn net an der Datebank fonnt ginn!\r\nSoubal dei dra stinn kanns de et naemol probéieren";
                            string caption = "Fehlend Wierder an der Datebank!";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            MessageBox.Show(message, caption, buttons);

                            return;
                        }
                        if (lbValues.Items.Count > lbOccurences.Items.Count)
                        {
                            Debug.WriteLine("SWAG!");

                            //Find not found Words
                            List<string> _tmpList = new List<string>();
                            List<string> _sList = new List<string>();
                            List<string> _tList = new List<string>();

                            _sList = lbValues.Items.Cast<string>().ToList();
                            _tList = lbOccurences.Items.Cast<string>().ToList();

                            _tmpList = _sList.Except(_tList).ToList();

                            lbNotFound.Items.AddRange(_tmpList.ToArray());
                            lbNotFound.Visible = true;

                            string message = "Ups!\r\nFolgend Wierder sinn net an der Datebank fonnt ginn!\r\nSoubal dei dra stinn kanns de et naemol probéieren";
                            string caption ="Fehlend Wierder an der Datebank!";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            MessageBox.Show(message, caption, buttons);

                            return;
                        }
                        lbOccurences.Items.Clear();

                            //Now Create all of the directories
                            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(SourcePath, folderBrowser.SelectedPath + "\\WebResources\\"));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(SourcePath, folderBrowser.SelectedPath + "\\WebResources\\"), true);

                        foreach (var theOccurence in Occurence)
                        {
                            string _filename = filename + "popup_" + DeUmlaut(theOccurence.WordOccurence.ToString()) + ".html";
                            string _filenameH = filename + "popup_" + DeUmlaut(theOccurence.WordOccurence.ToString()) + "H.html";
                            lbOccurences.Items.Add(theOccurence.WordOccurence.ToString()+ " " + _filename);
                            File.Copy(SourcePath+ "popupbase-web-resources\\popupbase.html", folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\" + _filename,true);
                            File.Copy(SourcePath + "popupbase-web-resources\\popupbaseH.html", folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\" + _filenameH,true);

                            // if (theOccurence.Translations.Count > 0)
                            // {
                            int _sel = 0;

                            string  _luxword = null, _luxwordplural = null, _luxwordform = null, _frword = null, _deword = null, _enword = null, _ptword = null;
                            string _mp3 = null;
                            string _pluralbegin = "(Pluriel: ";
                            string _pluralend = ")";

                            if (theOccurence.Translations.Count > 1)
                            {
                                var myKey = QuickSelect.FirstOrDefault(x => x.Value == theOccurence.WordOccurence).Key;
                                if (myKey == 0)
                                { 
                                    dlgSwitch dialog = new dlgSwitch();
                                    GroupBox _groupBox = (GroupBox)dialog.Controls[1];
                                    int _thecount = 0;
                                    foreach (var theTranslations in theOccurence.Translations)
                                    {
                                        RadioButton _rb = new RadioButton();

                                        _rb.Top = 20 + (32 * _thecount);
                                        _rb.Left = 10;
                                        string _example = "";
                                        if (theTranslations.Examples.Count > 0)
                                        {
                                            _example = theTranslations.Examples[0].ExampleText;
                                        }
                                        _rb.Text = theTranslations.Id + " " + theTranslations.Lu + " (" + theTranslations.WordForm +
                                            ") : " + "[" + theTranslations.De + " ¦ " + theTranslations.Fr + "]" + _example;
                                        _rb.AutoSize = true;
                                        _groupBox.Controls.Add(_rb);
                                        _thecount++;
                                    }
                                    dialog.ShowDialog(this);
                                    _thecount = 0;
                                    foreach (RadioButton item in _groupBox.Controls)
                                    {
                                        if (item.Checked)
                                        {
                                            _sel = _thecount;
                                        }
                                        _thecount++;
                                    }
                                    QuickSelect.Add(Int16.Parse(theOccurence.Translations[_sel].Id), theOccurence.WordOccurence.ToString());
                                    //using (StreamWriter _File = new StreamWriter(QuickSelectFile))
                                      //  _File.WriteLine("[{0} {1}]", Int16.Parse(theOccurence.Translations[_sel].Id, theOccurence.WordOccurence.ToString());

                                    File.WriteAllLines(folderBrowser.SelectedPath + "\\" + QuickSelectFile, QuickSelect.Select(kv => String.Join("	", kv.Key, kv.Value)));

                                    _mp3 = theOccurence.Translations[_sel].Audio.ToString() + ".mp3";
                                    _luxword = theOccurence.Translations[_sel].Lu.ToString();
                                    _luxwordplural = theOccurence.Translations[_sel].LuPlural.ToString();
                                    _luxwordform = theOccurence.Translations[_sel].WordForm.ToString();
                                    _frword = theOccurence.Translations[_sel].Fr.ToString();
                                    _deword = theOccurence.Translations[_sel].De.ToString();
                                    _enword = theOccurence.Translations[_sel].En.ToString();
                                    _ptword = theOccurence.Translations[_sel].Pt.ToString();

                                    
                                    if (_luxwordplural.Length == 0)
                                    {
                                        _pluralbegin = "";
                                        _pluralend = "";
                                    }
                                    if (_luxwordform == "Verb")
                                    {
                                        _pluralbegin = "(Participe passé: ";
                                    }
                                } else
                                {
                                    // OldKey Found!
                                    
                                    var match = theOccurence.Translations.FirstOrDefault(x => x.Id == myKey.ToString());
                                    Console.Write("yolo");
                                    //theOccurence.Translations
                                    _mp3 = match.Audio.ToString() + ".mp3";
                                    _luxword = match.Lu.ToString();
                                    _luxwordplural = match.LuPlural.ToString();
                                    _luxwordform = match.WordForm.ToString();
                                    _frword = match.Fr.ToString();
                                    _deword = match.De.ToString();
                                    _enword = match.En.ToString();
                                    _ptword = match.Pt.ToString();

                                    
                                    if (_luxwordplural.Length == 0)
                                    {
                                        _pluralbegin = "";
                                        _pluralend = "";
                                    }
                                    if (_luxwordform == "Verb")
                                    {
                                        _pluralbegin = "(Participe passé: ";
                                    }
                                }
                            }else
                            {
                                _mp3 = theOccurence.Translations[_sel].Audio.ToString() + ".mp3";
                                _luxword = theOccurence.Translations[_sel].Lu.ToString();
                                _luxwordplural = theOccurence.Translations[_sel].LuPlural.ToString();
                                _luxwordform = theOccurence.Translations[_sel].WordForm.ToString();
                                _frword = theOccurence.Translations[_sel].Fr.ToString();
                                _deword = theOccurence.Translations[_sel].De.ToString();
                                _enword = theOccurence.Translations[_sel].En.ToString();
                                _ptword = theOccurence.Translations[_sel].Pt.ToString();


                                if (_luxwordplural.Length == 0)
                                {
                                    _pluralbegin = "";
                                    _pluralend = "";
                                }
                                if (_luxwordform == "Verb")
                                {
                                    _pluralbegin = "(Participe passé: ";
                                }

                            }


                            string text = File.ReadAllText(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\" + _filename);
                            text = text.Replace("leColorCSScolor", charcolor);
                            text = text.Replace("aarbecht1.mp3", _mp3);
                            text = text.Replace("_LUXWORD_", _luxword);
                            text = text.Replace("_PLURALBEGIN_", _pluralbegin);
                            text = text.Replace("_LUXWORDPLURAL_", _luxwordplural);
                            text = text.Replace("_PLURALEND_", _pluralend);
                            text = text.Replace("_LUXWORDFORM_", _luxwordform);
                            text = text.Replace("_FRWORD_", _frword);
                            text = text.Replace("_FRWORD2_", "");
                            text = text.Replace("_DEWORD_", _deword);
                            text = text.Replace("_DEWORD2_", "");
                            text = text.Replace("_ENWORD_", _enword);
                            text = text.Replace("_ENWORD2_", "");
                            text = text.Replace("_PTWORD_", _ptword);
                            text = text.Replace("_PTWORD2_", "");
                            File.WriteAllText(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\" + _filename, text);

                            text = File.ReadAllText(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\" + _filenameH);
                            text = text.Replace("leColorCSScolor", charcolor);
                            text = text.Replace("aarbecht1.mp3", _mp3);
                            text = text.Replace("_LUXWORD_", _luxword);
                            text = text.Replace("_PLURALBEGIN_", _pluralbegin);
                            text = text.Replace("_LUXWORDPLURAL_", _luxwordplural);
                            text = text.Replace("_PLURALEND_", _pluralend);
                            text = text.Replace("_LUXWORDFORM_", _luxwordform);
                            text = text.Replace("_FRWORD_", _frword);
                            text = text.Replace("_FRWORD2_", "");
                            text = text.Replace("_DEWORD_", _deword);
                            text = text.Replace("_DEWORD2_", "");
                            text = text.Replace("_ENWORD_", _enword);
                            text = text.Replace("_ENWORD2_", "");
                            text = text.Replace("_PTWORD_", _ptword);
                            text = text.Replace("_PTWORD2_", "");
                            File.WriteAllText(folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\" + _filenameH, text);

                            // Copy MP3
                            File.Copy("\\\\192.168.1.75\\Konterbont_Produktioun\\Audio\\"+ _mp3, folderBrowser.SelectedPath + "\\WebResources\\popupbase-web-resources\\audio\\" + _mp3,true);
                            //  }
                        }
                    }
                    lblOccurences.Text = lbOccurences.Items.Count.ToString();
                }
            }
        }

        public String DeUmlaut(string value)
        {
            string Result;
            Result = value;
            Result = Result.ToLower();
            Result = Regex.Replace(Result, "ä", "ae", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ö", "oe", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ü", "ue", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ß", "ss", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "é", "e", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "è", "e", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ä", "a", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ë", "e", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "û", "u", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "Û", "u", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ê", "e", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "â", "a", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, " ", "_", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "'", "", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "’", "", RegexOptions.IgnoreCase);
            return Result;
        }

        private void MagazineAuswielenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //load Form Select Magazine
            //frmMagazineSelector theForm = new frmMagazineSelector();
            theForm.ShowDialog();
            IniParser = new FileIniDataParser();
            IniData data = IniParser.ReadFile(IniFileName);
            ComboBox theCombo = (ComboBox)theForm.Controls.Find("cbMagazine", false)[0];
            if (theCombo.SelectedIndex >= 0)
            {
                data["MAGAZINE"].GetKeyData("mag").Value = theCombo.SelectedItem.ToString();
                TargetMagazine = theCombo.SelectedItem.ToString();
                IniParser.WriteFile(IniFileName, data);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // LoadIni

            string[] _directories = Directory.GetDirectories(MagazinePath);
            theForm = new frmMagazineSelector(this);
            theCloseDialog = new frmCloseDialog();
            ComboBox theCombo = (ComboBox)theForm.Controls.Find("cbMagazine", false)[0];

            foreach (string _directory in _directories)
            {
                var dirName = new DirectoryInfo(_directory).Name;
                if (dirName != "Published")
                {
                    theCombo.Items.Add(dirName);
                }
            }

            if (File.Exists(IniFileName))
            {
                IniData data = IniParser.ReadFile(IniFileName);
                var _magazineValue = data["MAGAZINE"].GetKeyData("mag").Value;
                if (_magazineValue != "")
                {
                    theCombo.SelectedIndex = theCombo.FindStringExact(_magazineValue);
                }
            } else
            {
                using (StreamWriter theFile = new StreamWriter(IniFileName))
                {
                    string bline ="[MAGAZINE]"+ Environment.NewLine +"mag=";
                    theFile.WriteLine(bline);
                }
            }
            
        }

        private void BtnCopyToMag_Click(object sender, EventArgs e)
        {
            // copy generated Content to selected Magazine

            //if check if targetMagazine = empty
            if (targetMagazine == "")
            {
                string message = "Ups!\r\nEt ass nach keen Magazin ausgewielt!\r\nWiel fir 1. een aus.";
                string caption = "Kee Magazinn ausgewielt!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
            else
            {
                StartCopyToMagazine();
            }
        }

       private void StartCopy()
        {

        }

        private void StartCopyToMagazine()
        {
            CopyToMagazine();
            UpdatebtnCopyToMag(false);
        }

        public void UpdatebtnCopyToMag(Boolean _bool)
        {
            if (ControlInvokeRequired(btnCopyToMag, () => UpdatebtnCopyToMag(_bool))) return;
            btnCopyToMag.Enabled = _bool;
        }

        public void UpdatebtnCopyAndLoad(Boolean _bool)
        {
            if (ControlInvokeRequired(btnLoadAndGo, () => UpdatebtnCopyAndLoad(_bool))) return;
            btnLoadAndGo.Enabled = _bool;
        }

        async void CopyToMagazine()
        {
            await Task.Run(() =>
            {
                
                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(CurrentPath + "WebResources\\", "*",
                        SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(CurrentPath + "WebResources\\", MagazinePath + targetMagazine + "\\WebResources\\"));

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(CurrentPath + "WebResources\\", "*.*",
                        SearchOption.AllDirectories))
                    {
                        var _newFilePath = newPath.Replace(CurrentPath + "WebResources\\", MagazinePath + targetMagazine + "\\WebResources\\");
                    if (File.Exists(_newFilePath))
                    {
                        File.Delete(_newFilePath);
                    }
                        //
                        File.Copy(newPath, _newFilePath, true);
                    }
                    Debug.WriteLine(MagazinePath + targetMagazine);
                
                    UpdatebtnCopyToMag(true);
                CopyClicked = true;
                
            });
            
        }

        async void Copy()
        {
            await Task.Run(() =>
            {
                try
                {
                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(CurrentPath + "WebResources\\", "*",
                        SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(CurrentPath + "WebResources\\", MagazinePath + targetMagazine + "\\WebResources\\"));

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(CurrentPath + "WebResources\\", "*.*",
                        SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(CurrentPath + "WebResources\\", MagazinePath + targetMagazine + "\\WebResources\\"), true);
                    }

                    Debug.WriteLine(MagazinePath + targetMagazine);
                }
                finally
                {
                    UpdatebtnCopyToMag(true);
                }
            });
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ZoumaachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CopyClicked == false)
            {
                DialogResult click = theCloseDialog.ShowDialog();
                if (click==DialogResult.Cancel)
                { e.Cancel = true; }
                else if (click == DialogResult.OK)
                { e.Cancel = false; }
            }
        }
    }

    public class Occurence
    {
        [JsonProperty("WordOccurence")]
        public string WordOccurence { get; set; }
        [JsonProperty("Translations")]
        public List<Translation> Translations { get; set; }
    }

    public class Translation
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Lu")]
        public string Lu { get; set; }
        [JsonProperty("LuPlural")]
        public string LuPlural { get; set; }
        [JsonProperty("WordForm")]
        public string WordForm { get; set; }
        [JsonProperty("De")]
        public string De { get; set; }
        [JsonProperty("Fr")]
        public string Fr { get; set; }
        [JsonProperty("En")]
        public string En { get; set; }
        [JsonProperty("Pt")]
        public string Pt { get; set; }
        [JsonProperty("Audio")]
        public string Audio { get; set; }
        [JsonProperty("Examples")]
        public List<Example> Examples { get; set; }
    }

    public class Example
    {
        [JsonProperty("ExampleText")]
        public string ExampleText { get; set; }
    }

}
