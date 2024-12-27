using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Asv.Gnss
{
    public class LogMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ComNavPortEnum Direction { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ComNavMessageEnum Message { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ComNavFormat Format { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ComNavTriggerEnum Trigger { get; set; }
        public double Period { get; set; }
        public double Offset { get; set; }
    }
}
