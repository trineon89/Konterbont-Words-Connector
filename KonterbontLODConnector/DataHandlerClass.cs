using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        [J("filename", NullValueHandling = N.Ignore)] public string Filename { get; set; }
        [J("filepath", NullValueHandling = N.Ignore)] public string Filepath { get; set; }
        [J("quickselectfile", NullValueHandling = N.Ignore)] public string QuickSelectFile { get; set; }
        [J("qsindex", NullValueHandling = N.Ignore)] private int QSindex { get; set; }
        [J("quickselect", NullValueHandling = N.Ignore)] public Dictionary<int, int> QuickSelect { get; set; }
        [J("wordlist", NullValueHandling = N.Ignore)] public List<AutoComplete> WordList { get; set; }
        public DataHandler()
        {
            QSindex = 0;
            Filename = null;
            Filepath = null;
            QuickSelectFile = null;
            QuickSelect = new Dictionary<int, int>();
            WordList = new List<AutoComplete>();
            frmMagazineSelectorInit();
        }

        public DataHandler(string _filename)
        {
            QSindex = 0;
            Filename = _filename;
            Filepath = null;
            QuickSelectFile = Path.GetFileNameWithoutExtension(_filename) + ".selections"; ;
            QuickSelect = new Dictionary<int, int>();
            WordList = new List<AutoComplete>();
            frmMagazineSelectorInit();
        }

        public DataHandler(string _filename, string _filepath)
        {
            QSindex = 0;
            Filename = _filename;
            Filepath = _filepath;
            QuickSelectFile = Path.GetFileNameWithoutExtension(_filename) + ".selections"; ;
            QuickSelect = new Dictionary<int, int>();
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
            QSindex++;
            QuickSelect.Add(QSindex, WordList.Count() - 1);
        }

        public void SaveToFile(DataHandler dt)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = N.Ignore;

            using (StreamWriter sw = new StreamWriter(Filepath+Filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dt);
            }
            File.WriteAllLines(Filepath + QuickSelectFile, QuickSelect.Select(kv => String.Join("	", kv.Key, kv.Value)));
        }

        public DataHandler LoadFromFile(string _filepath, string _filename)
        {
            DataHandler dt = new DataHandler(_filename, _filepath);
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(_filepath+"\\"+_filename))
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

        public void OutputPopup(Wuert wuert, string occurence, string rgbvalue)
        {
            string _tmpfilecontent = Properties.Resources.popup;
            _tmpfilecontent = _tmpfilecontent.Replace("leColorCSScolor", rgbvalue);
            _tmpfilecontent = _tmpfilecontent.Replace("aarbecht1.mp3", wuert.MP3);
            _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORD_", wuert.Meanings[wuert.Selection-1].LU);
            _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORDPLURAL_", wuert.Meanings[wuert.Selection - 1].LUs);
            _tmpfilecontent = _tmpfilecontent.Replace("_LUXWORDFORM_", wuert.WuertForm.WuertForm);
            _tmpfilecontent = _tmpfilecontent.Replace("_FRWORD_", wuert.Meanings[wuert.Selection - 1].FR);
            _tmpfilecontent = _tmpfilecontent.Replace("_DEWORD_", wuert.Meanings[wuert.Selection - 1].DE);
            _tmpfilecontent = _tmpfilecontent.Replace("_ENWORD_", wuert.Meanings[wuert.Selection - 1].EN);
            _tmpfilecontent = _tmpfilecontent.Replace("_PTWORD_", wuert.Meanings[wuert.Selection - 1].PT);
            GetMp3(wuert.MP3);
            File.WriteAllText(Filepath + "WebResources\\popupbase-web-resources\\" + occurence + ".html", _tmpfilecontent);
        }

        public void PrepareOutputFolder()
        {
            if (Directory.Exists(Filepath + "WebResources\\popupbase-web-resources"))
            {
                try
                {
                    Directory.Delete(Filepath + "WebResources\\popupbase-web-resources", true);
                }
                catch (Exception ea)
                {
                    Console.WriteLine("{0} Exception caught.", ea);
                }
            }
            Directory.CreateDirectory(Filepath + "WebResources\\popupbase-web-resources");
            Directory.CreateDirectory(Filepath + "WebResources\\popupbase-web-resources\\audio");
            File.WriteAllBytes(Filepath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro_BookItalic.ttf", Properties.Resources.FreightSansCmpPro_BookItalic);
            File.WriteAllBytes(Filepath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro_Med.ttf", Properties.Resources.FreightSansCmpPro_Med);
            File.WriteAllBytes(Filepath + "WebResources\\popupbase-web-resources\\FreightSansCmpPro_Semi.ttf", Properties.Resources.FreightSansCmpPro_Semi);
        }

        /// <summary>
        /// QuickSelect Datei Lueden an als Lëscht oofspäicheren
        /// </summary>
        public void LoadQuickSelect(IEnumerable<string> filepaths)
        {
            QuickSelect = filepaths.Select(sline => sline.Split('	')).ToDictionary(split => Convert.ToInt32(split[0]), split => Convert.ToInt32(split[1]));
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
