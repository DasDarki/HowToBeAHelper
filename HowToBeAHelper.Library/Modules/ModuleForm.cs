using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HowToBeAHelper.Modules
{
    [Serializable]
    public class ModuleForm
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }

        [Serializable]
        [XmlRoot("row")]
        public class Row
        {
            [XmlElement("column")]
            public List<Column> Columns { get; set; }
        }

        [Serializable]
        [XmlRoot("column")]
        public class Column
        {
            [XmlElement("input")]
            public List<Input> Inputs { get; set; }

            [XmlElement("select")]
            public List<Select> Selects { get; set; }
        }

        [Serializable]
        [XmlRoot("select")]
        public class Select
        {
            [XmlAttribute("key")]
            public string Key { get; set; }

            [XmlAttribute("label")]
            public string Label { get; set; }

            [XmlElement("option")] 
            public List<string> Options { get; set; } = new List<string>();
        }

        [Serializable]
        [XmlRoot("input")]
        public class Input
        {
            [XmlAttribute("type")]
            public string Type { get; set; }

            [XmlAttribute("key")]
            public string Key { get; set; }

            [XmlAttribute("label")]
            public string Label { get; set; }

            [XmlAttribute("placeholder")]
            public string Placeholder { get; set; }

            [XmlAttribute("dblaction")]
            public string DblClickAction { get; set; }
        }
    }
}
