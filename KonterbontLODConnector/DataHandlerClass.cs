using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

namespace KonterbontLODConnector
{
    public partial class DataHandler
    {
        private string lodmp3path = "https://www.lod.lu/audio/";
        private string MagazinePath = "\\\\192.168.1.75\\Konterbont_Produktioun\\Magazines\\";
        private frmMagazineSelector theform;
        private string targetMag;

        public string Filename { get; set; }
        [J("filepath", NullValueHandling = N.Ignore)] public string Filepath { get; set; }
        [J("wordlist", NullValueHandling = N.Ignore)] public List<AutoComplete> WordList { get; set; }
        [J("globrgb", NullValueHandling = N.Ignore)] public string globrgb { get; set; }

        public DataHandler()
        {
            Filename = null;
            Filepath = null;
            WordList = new List<AutoComplete>();
            frmMagazineSelectorInit();
        }

        public DataHandler(string _filename)
        {
            Filename = _filename;
            Filepath = null;
            WordList = new List<AutoComplete>();
            frmMagazineSelectorInit();
        }

        public DataHandler(string _filename, string _filepath)
        {
            Filename = _filename;
            Filepath = _filepath;
            WordList = new List<AutoComplete>();
            frmMagazineSelectorInit();
        }

        public void frmMagazineSelectorInit()
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
        }

        public void SetRGB(string input)
        {
            globrgb = input;
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

        public void GetMp3(string mp3filename)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(new Uri(lodmp3path + mp3filename), Filepath + "WebResources\\popupbase-web-resources\\audio\\" + mp3filename);
            }
        }

        public bool OutputPopups()
        {
            foreach (AutoComplete ac in WordList)
            {
                OutputPopup(ac.Wierder[ac.Selection - 1], ac.Occurence, globrgb);
            }
            return true;
        }

        public void OutputPopup(Wuert wuert, string occurence, string rgbvalue)
        {
            string _tmpfilecontent = Properties.Resources.popup;
            _tmpfilecontent = _tmpfilecontent.Replace("leColorCSScolor", rgbvalue);
            _tmpfilecontent = _tmpfilecontent.Replace("aarbecht1.mp3", wuert.MP3);
            _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORD_", wuert.Meanings[wuert.Selection - 1].LU);

            if (wuert.Meanings[wuert.Selection - 1].LUs == null)
            {
                _tmpfilecontent = _tmpfilecontent.Replace("_PLURALBEGIN_", "");
                _tmpfilecontent = _tmpfilecontent.Replace("_PLURALEND_", "");
            }
            else if (wuert.WuertForm.WuertForm == "Verb")
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
            GetMp3(wuert.MP3);
            occurence = DeUmlaut(occurence);
            File.WriteAllText(Filepath + "WebResources\\popupbase-web-resources\\" + Path.GetFileNameWithoutExtension(Filename) + "popup_" +occurence + ".html", _tmpfilecontent);
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
            return Result;
        }

        public void PrepareOutputFolder(bool reset = true)
        {
            if (Directory.Exists(Filepath + "WebResources\\popupbase-web-resources"))
            {
                try
                {
                    Directory.Move(Filepath + "WebResources", Filepath + "WebResources_x");
                    Directory.Delete(Filepath + "WebResources_x\\popupbase-web-resources", true);
                    Directory.Move(Filepath + "WebResources_x", Filepath + "WebResources");
                }
                catch (Exception ea)
                {
                    Console.WriteLine("{0} Exception caught.", ea);
                }
            }

            Directory.CreateDirectory(Filepath + "WebResources\\popupbase-web-resources");
            Directory.CreateDirectory(Filepath + "WebResources\\popupbase-web-resources\\audio");
            File.WriteAllBytes(Filepath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro-BookItalic.ttf", Properties.Resources.FreightSansCmpPro_BookItalic);
            File.WriteAllBytes(Filepath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro-Med.ttf", Properties.Resources.FreightSansCmpPro_Med);
            File.WriteAllBytes(Filepath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro-Semi.ttf", Properties.Resources.FreightSansCmpPro_Semi);
            File.WriteAllText(Filepath + "WebResources\\popupbase-web-resources\\popupstyle.css", Properties.Resources.popupstyle);
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
