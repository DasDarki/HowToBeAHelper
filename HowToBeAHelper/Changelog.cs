using System.Collections.Generic;
using System.Windows.Forms;

namespace HowToBeAHelper
{
    internal class Changelog
    {
        public string Summary { get; }

        public string Content { get; }

        public string Author { get; }
        
        public string Date { get; }

        public string Version { get; }

        private Changelog(Dictionary<string, string> dict)
        {
            Summary = dict["Summary"];
            Content = dict["Content"];
            Author = dict["Author"];
            Date = dict["Date"];
            Version = Updater.CurrentVersion;
        }

        internal static Changelog Parse(string data)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"Summary", ""},
                {"Content", ""},
                {"Author", ""},
                {"Date", ""}
            };
            string type = null;
            foreach (string line in data.Split('\n'))
            {
                string formatted = line.Trim();
                if (string.IsNullOrEmpty(formatted)) continue;
                if (formatted.StartsWith("!"))
                {
                    type = formatted.Substring(1);
                }
                else if(type != null)
                {
                    dict[type] += formatted + "\n";
                }
            }

            return new Changelog(dict);
        }
    }
}
