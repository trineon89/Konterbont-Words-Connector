using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class MicroStructures
    {
        [JsonProperty("microStructures")]
        public List<MicroStructure> MicroStructuresMicroStructures { get; set; }
    }

    public partial class MicroStructure
    {
        [JsonProperty("auxiliaryVerb", NullValueHandling = NullValueHandling.Ignore)]
        public string AuxiliaryVerb { get; set; }

        [JsonProperty("pastParticiple", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> PastParticiple { get; set; }
    }


    public partial class GrammaticalUnit
    {

    }

    public partial class LODMeaning
    {
        [JsonProperty("secondaryHeadword", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryHeadword { get; set; }
    }

    public partial class LODExample
    {

    }

    public partial class ExamplePart
    {

    }

    public partial class PartPart
    {

    }

    public partial class TargetLanguages
    {

    }

    public partial class De
    {

    }

    public partial class DePart
    {

    }

    public partial class Inflection
    {
        
    }

}
