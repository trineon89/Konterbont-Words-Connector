using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    class Exporter
    {
        private string magazinePath = @"\\cubecluster01\Konterbont_Produktioun\Magazines\";
        private string customAudioPath = @"\\cubecluster01\Konterbont_Audio\";
        public List<ExporterClassObject> wordlist = new List<ExporterClassObject>();
        public string globrgb = "";

        public async Task<bool> Init()
        {

            return true;
        }

        public async Task DoExport(classes.WordOverview _wo)
        {

        }
    }

    class ExporterClassObject
    {
        public string Occurence = "";
        public string WuertForm = "";
        public string LU = "";
        public string LUs = "";
        public string DE = "";
        public string FR = "";
        public string EN = "";
        public string PT = "";
        public string MP3 = "";
    }
}
