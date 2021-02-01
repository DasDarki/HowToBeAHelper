using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace HowToBeAHelper.Modules
{
    [Serializable]
    public class ModuleMeta
    {
        [XmlElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [XmlElement("version")]
        [JsonProperty("version")]
        public string Version { get; set; }

        [XmlElement("author")]
        [JsonProperty("author")]
        public string Author { get; set; }

        [XmlElement("icon")]
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
