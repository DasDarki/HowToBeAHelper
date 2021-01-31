using System;
using System.IO;
using System.Xml.Serialization;

namespace HowToBeAHelper.Modules
{
    [Serializable]
    [XmlRoot("module")]
    public class Module
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Module));

        [XmlElement("meta")]
        public ModuleMeta Meta { get; set; }

        [XmlElement("skills")] 
        public ModuleSkills Skills { get; set; } = new ModuleSkills();

        [XmlElement("form")]
        public ModuleForm Form { get; set; }

        public string Ruleset { get; set; }

        public static Module TryLoad(string xml)
        {
            try
            {
                using StringReader reader = new StringReader(xml);
                return Serializer.Deserialize(reader) as Module;
            }
            catch
            {
                return null;
            }
        } 
    }
}
