using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HowToBeAHelper.Modules
{
    [Serializable]
    public class ModuleSkills
    {
        [XmlArray("acting")]
        [XmlArrayItem("skill", typeof(string))]
        public List<string> Acting { get; set; } = new List<string>();

        [XmlArray("knowledge")]
        [XmlArrayItem("skill", typeof(string))]
        public List<string> Knowledge { get; set; } = new List<string>();

        [XmlArray("social")]
        [XmlArrayItem("skill", typeof(string))]
        public List<string> Social { get; set; } = new List<string>();
    }
}
