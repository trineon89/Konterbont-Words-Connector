using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Independentsoft.Office.Odf;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;
using NIL = Newtonsoft.Json.DefaultValueHandling;

namespace KonterbontLODConnector
{
    public partial class DataHandler
    {
        private string lodmp3path = "https://www.lod.lu/audio/";
        private string MagazinePath = "\\\\cubecluster01\\Konterbont_Produktioun\\Magazines\\";
        private string CustomAudioPath = "\\\\cubecluster01\\Konterbont_Audio\\";
        public string DocPath = null;
        //public TextDocument Article = null;
        public List<string> StyleName = null;
        private frmMagazineSelector theform;
        [J("targetMag", NullValueHandling = N.Ignore)] public string targetMag { get; set; }
        private bool hasPopups = false;
        private bool isInMag = false;
        private bool isSaved = false;

        private string Temppath = Path.GetTempPath() + "_KBLODCONN\\";

        public string Filename { get; set; }
        [J("filepath", NullValueHandling = N.Ignore)] public string Filepath { get; set; }
        [J("wordlist", NullValueHandling = N.Ignore)] public List<AutoComplete> WordList { get; set; }
        [J("globrgb", NullValueHandling = N.Ignore)] public string Globrgb { get; set; }
        [DefaultValue(false)] [J("customcolor", DefaultValueHandling = NIL.Populate)] public Boolean CustomColor { get; set; }

        public DataHandler()
        {
            Filename = null;
            Filepath = null;
            WordList = new List<AutoComplete>();
            FrmMagazineSelectorInit();
        }

        public DataHandler(string _filename)
        {
            Filename = _filename;
            Filepath = null;
            WordList = new List<AutoComplete>();
            FrmMagazineSelectorInit();
        }

        public DataHandler(string _filename, string _filepath)
        {
            Filename = _filename;
            Filepath = _filepath;
            WordList = new List<AutoComplete>();
            FrmMagazineSelectorInit();
        }

        public DataHandler(string _filename, string _filepath, string _targetMag)
        {
            Filename = _filename;
            Filepath = _filepath;
            targetMag = _targetMag;
            WordList = new List<AutoComplete>();
            FrmMagazineSelectorInit();
        }

        public string getTemppath()
        {
            return Temppath;
        }

        public bool IsSaved()
        {
            return isSaved;
        }

        public void IsSaved(bool setter)
        {
            isSaved = setter;
        }

        public bool HasPopups()
        {
            return hasPopups;
        }

        public void HasPopups(bool setter)
        {
            hasPopups = setter;
        }

        public bool IsInMag()
        {
            return isInMag;
        }

        public void IsInMag(bool setter)
        {
            isInMag = setter;
        }

        public void FrmMagazineSelectorInit()
        {
            string[] _dirs = Directory.GetDirectories(MagazinePath);
            theform = new frmMagazineSelector();
            ComboBox theCombo = (ComboBox)theform.Controls.Find("cbMagazine", false)[0];
            foreach (string _dir in _dirs)
            {
                var dirName = new DirectoryInfo(_dir).Name;
                if (dirName != "Published")
                {
                    theCombo.Items.Add(dirName);
                }
            }

            if (targetMag != null)
            {
                theCombo.SelectedIndex = theCombo.FindString(targetMag);
            }
        }

        public void SetRGB(string input)
        {
            if (!CustomColor) Globrgb = input;
        }

        public bool InitParseMagazine()
        {
            if (targetMag == null)
                return false;
            return true;
        }

        public void ShowMagazineSelector()
        {
            theform.ShowDialog();
            ComboBox theCombo = (ComboBox)theform.Controls.Find("cbMagazine", false)[0];
            if (theCombo.SelectedIndex >= 0)
            {
                targetMag = theCombo.SelectedItem.ToString();
            }
        }

        public void AddWordToList(AutoComplete ac)
        {
            WordList.Add(ac);
        }

        public void ReplaceWordInList(AutoComplete ac, int internalId)
        {
            if (WordList.Count > internalId - 1)
            {
                WordList.RemoveAt(internalId - 1);
                WordList.Insert(internalId - 2, ac);
                for (int counter = 0; counter < WordList.Count; counter++)
                {
                    WordList[counter].internalId = counter + 1;
                }
            }
            else
            {
                AddWordToList(ac);
            }
        }

        public void SaveToFile(DataHandler dt)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = N.Ignore;

            using (StreamWriter sw = new StreamWriter(Filepath + Filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dt);
            }
        }

        public DataHandler LoadFromFile(string _filepath, string _filename)
        {
            DataHandler dt = new DataHandler(_filename, _filepath);
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(_filepath + "\\" + _filename))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                dt = serializer.Deserialize<DataHandler>(reader);
            }
            return dt;
        }

        /// <summary>
        /// Downloads MP3-File from LOD.lu and copy to Tempfolder.
        /// If "hasCustomAudio" is set to true, copys the file from LocalServer to the Tempfolder
        /// </summary>
        /// <param name="mp3filename"></param>
        /// <param name="hasCustomAudio"></param>
        public void GetMp3(string mp3filename, bool hasCustomAudio)
        {
            if (hasCustomAudio)
            {
                File.Copy(CustomAudioPath + mp3filename, Temppath + "WebResources\\popupbase-web-resources\\audio\\" + mp3filename, true);
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(new Uri(lodmp3path + mp3filename), Temppath + "WebResources\\popupbase-web-resources\\audio\\" + mp3filename);
                }
            }
        }

        /// <summary>
        /// Generate Popups (HTML Files) for all Items in the Wordlist
        /// </summary>
        /// <returns></returns>
        public bool OutputPopups()
        {
            foreach (AutoComplete ac in WordList)
            {
                OutputPopup(ac.Wierder[ac.Selection - 1], ac.Occurence, Globrgb);
            }
            return true;
        }

        public void OutputPopup(Wuert wuert, string occurence, string rgbvalue)
        {
            if (wuert.Selection != 0)
            {
                string _tmpfilecontent = Properties.Resources.popup;
                _tmpfilecontent = _tmpfilecontent.Replace("leColorCSScolor", rgbvalue);
                _tmpfilecontent = _tmpfilecontent.Replace("aarbecht1.mp3", wuert.Meanings[wuert.Selection - 1].MP3);
                _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORD_", wuert.Meanings[wuert.Selection - 1].LU);

                if (wuert.Meanings[wuert.Selection - 1].LUs == null)
                {
                    _tmpfilecontent = _tmpfilecontent.Replace("_PLURALBEGIN_", "");
                    _tmpfilecontent = _tmpfilecontent.Replace("_PLURALEND_", "");
                }
                else if (wuert.WuertForm.WuertForm == "Verb" || wuert.WuertForm.WuertForm == "Modalverb")
                {
                    _tmpfilecontent = _tmpfilecontent.Replace("_PLURALBEGIN_", "(Participe Passé: ");
                    _tmpfilecontent = _tmpfilecontent.Replace("_PLURALEND_", ")");
                }
                else
                {
                    _tmpfilecontent = _tmpfilecontent.Replace("_PLURALBEGIN_", "(Pluriel: ");
                    _tmpfilecontent = _tmpfilecontent.Replace("_PLURALEND_", ")");
                }

                _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORDPLURAL_", wuert.Meanings[wuert.Selection - 1].LUs);

                _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORDFORM_", wuert.WuertForm.WuertForm);
                _tmpfilecontent = _tmpfilecontent.Replace("_FRWORD_", wuert.Meanings[wuert.Selection - 1].FR);
                _tmpfilecontent = _tmpfilecontent.Replace("_DEWORD_", wuert.Meanings[wuert.Selection - 1].DE);
                _tmpfilecontent = _tmpfilecontent.Replace("_ENWORD_", wuert.Meanings[wuert.Selection - 1].EN);
                _tmpfilecontent = _tmpfilecontent.Replace("_PTWORD_", wuert.Meanings[wuert.Selection - 1].PT);
                occurence = DeUmlaut(occurence);
                GetMp3(wuert.Meanings[wuert.Selection - 1].MP3, wuert.Meanings[wuert.Selection - 1].hasCustomAudio);
                File.WriteAllText(Temppath + "WebResources\\popupbase-web-resources\\" + Path.GetFileNameWithoutExtension(Filename) + "popup_" + occurence + ".html", _tmpfilecontent);
            }
        }

        public void CopyTmpToArt()
        {
            PrepareOutputFolder();
            // https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(Temppath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(Temppath, Filepath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(Temppath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(Temppath, Filepath), true);
        }

        public string DeUmlaut(string inp)
        {
            string Result;
            Result = inp;
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
            Result = Regex.Replace(Result, "\\s", "_", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "'", "", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "’", "", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "û", "u", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "â", "a", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ô", "o", RegexOptions.IgnoreCase);
            Result = Regex.Replace(Result, "ç", "c", RegexOptions.IgnoreCase);
            return Result;
        }

        public void PrepareOutputFolder(bool reset = true)
        {
            if (Directory.Exists(Filepath + "WebResources\\popupbase-web-resources"))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(Filepath + "WebResources\\popupbase-web-resources");
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
            }


        }

        /// <summary>
        /// Erstellt den TempFolder an preparéiert d'Basisfichieren
        /// </summary>
        public void PrepareOutputTmpFolder()
        {
            if (!Directory.Exists(Temppath))
            {
                Directory.CreateDirectory(Temppath);
            }
            else
            {

                //Cleanup
                //https://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory

                System.IO.DirectoryInfo di = new DirectoryInfo(Temppath);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
            }
            Directory.CreateDirectory(Temppath + "WebResources\\popupbase-web-resources");
            Directory.CreateDirectory(Temppath + "WebResources\\popupbase-web-resources\\audio");
            File.WriteAllBytes(Temppath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro-BookItalic.ttf", Properties.Resources.FreightSansCmpPro_BookItalic);
            File.WriteAllBytes(Temppath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro-Med.ttf", Properties.Resources.FreightSansCmpPro_Med);
            File.WriteAllBytes(Temppath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro-Semi.ttf", Properties.Resources.FreightSansCmpPro_Semi);
            File.WriteAllText(Temppath + "WebResources\\popupbase-web-resources\\popupstyle.css", Properties.Resources.popupstyle);
        }

        public string[] InitCopyToMag()
        {
            if (targetMag == null)
                return null;
            string SourcePath = Filepath + "WebResources\\";
            var MagFiles = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
            Console.WriteLine("Count: " + MagFiles.Length);
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, MagazinePath + targetMag + "\\WebResources\\"));

            return MagFiles;
        }

        public async void CopyToMag(string _file)
        {
            await Task.Run(() =>
            {
                var _newFilePath = _file.Replace(Filepath + "WebResources\\", MagazinePath + targetMag + "\\WebResources\\");
                if (File.Exists(_newFilePath))
                {
                    File.Delete(_newFilePath);
                }
                //
                File.Copy(_file, _newFilePath, true);
            });
        }
    }

    public partial class DataHandler
    {
        public static DataHandler FromJson(string json) => JsonConvert.DeserializeObject<DataHandler>(json, KonterbontLODConnector.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this DataHandler self) => JsonConvert.SerializeObject(self, KonterbontLODConnector.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}