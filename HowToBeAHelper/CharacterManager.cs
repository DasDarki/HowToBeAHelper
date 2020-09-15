using System.Collections.Generic;
using System.IO;
using HowToBeAHelper.Model.Characters;
using Newtonsoft.Json;

namespace HowToBeAHelper
{
    internal class CharacterManager
    {
        public List<Character> Characters { get; }

        public string FilePath { get; }

        internal CharacterManager()
        {
            FilePath = Path.Combine(Bootstrap.DataPath, "characters.toml");
            Characters = new List<Character>();
            if (File.Exists(FilePath))
            {
                try
                {
                    Characters = JsonConvert.DeserializeObject<List<Character>>(File.ReadAllText(FilePath));
                }
                catch
                {
                    //Ignore: needs handling
                }
            }
            else
            {
                Save();
            }
        }

        internal void Save()
        {
            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(Characters));
            }
            catch
            {
                //Ignore: needs handling
            }
        }
    }
}
