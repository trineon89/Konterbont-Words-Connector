using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

namespace KonterbontLODConnector
{
    public partial class DataHandler
    {
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
        }

        public DataHandler(string _filename)
        {
            QSindex = 0;
            Filename = _filename;
            Filepath = null;
            QuickSelectFile = null;
            QuickSelect = new Dictionary<int, int>();
            WordList = new List<AutoComplete>();
        }

        public DataHandler(string _filename, string _filepath)
        {
            QSindex = 0;
            Filename = _filename;
            Filepath = _filepath;
            QuickSelectFile = null;
            QuickSelect = new Dictionary<int, int>();
            WordList = new List<AutoComplete>();
        }

        public void AddWordToList(AutoComplete ac)
        {
            WordList.Add(ac);
            QSindex++;
            QuickSelect.Add(QSindex, WordList.Count() - 1);
        }

        public void SaveToFile(DataHandler dt)
        {
            //string json = dt.ToJson();
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = N.Ignore;

            using (StreamWriter sw = new StreamWriter(Filepath+Filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dt);
            }
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
