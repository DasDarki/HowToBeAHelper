using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HowToBeAHelper
{
    internal class Settings
    {
        public bool AutoStart { get; set; } = false;

        public bool StartMinimize { get; set; } = false;

        public bool MinimizeToTray { get; set; } = false;

        internal void Save()
        {
            try
            {
                File.WriteAllText(GetPath(), JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch
            {
                //Ignore: Needs handling
            }
        }

        internal static Settings Load()
        {
            try
            {
                string path = GetPath();
                if (File.Exists(path))
                {
                    return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
                }
                else
                {
                    Settings settings = new Settings();
                    settings.Save();
                    return settings;
                }
            }
            catch
            {
                //Ignore: Needs handling
                return new Settings();
            }
        }

        private static string GetPath()
        {
            return Path.Combine(Bootstrap.DataPath, "settings.json");
        }
    }
}
