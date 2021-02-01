using System;
using System.Collections.Generic;
using HowToBeAHelper.Model;
using HowToBeAHelper.Model.Characters;
using HowToBeAHelper.Model.Skills;
using HowToBeAHelper.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HowToBeAHelper.Scripting
{
    internal class ScriptingSystem : ISystem
    {
        public string User { get; internal set; }

        public bool IsLocalCharacter { get; set; } = false;

        public bool IsLoggedIn => !string.IsNullOrEmpty(User);

        public bool IsSessionView { get; set; } = false;

        public Session CurrentSession { get; set; } = null;

        public bool IsSessionSelected => CurrentSession != null;

        public List<Character> LocalCharacters => Bootstrap.CharacterManager.Characters;

        public IReadOnlyList<Character> RemoteCharacters => Characters.AsReadOnly();

        public INetwork Network { get; set; }

        internal List<Character> Characters { get; set; } = new List<Character>();

        public void SaveLocalCharacters()
        {
            Bootstrap.CharacterManager.Save();
        }

        public Character GetCharacter(string charId, bool isLocal)
        {
            return (isLocal ? LocalCharacters : Characters).SelectFirst(character => character.ID == charId);
        }

        public event Action LoggedIn;
        public event Action LoggedOut;
        public event Action<Character> CharacterUpdate;
        public event Action<Character> CharacterCreate;
        public event Action<Character> CharacterDelete;
        public event Action<Character> CharacterLoad;

        internal void TriggerCharacterLoad(Character character)
        {
            CharacterLoad?.Invoke(character);
        }

        internal void TriggerCharacterCreate(Character character)
        {
            CharacterCreate?.Invoke(character);
        }

        internal void TriggerCharacterUpdate(Character character)
        {
            CharacterUpdate?.Invoke(character);
        }

        internal void TriggerCharacterDelete(Character character)
        {
            CharacterDelete?.Invoke(character);
        }

        internal void TriggerLoggedIn()
        {
            LoggedIn?.Invoke();
        }

        internal void TriggerLoggedOut()
        {
            LoggedOut?.Invoke();
        }

        internal void OnDeleteChar(string charId)
        {
            Character character = Characters.SelectFirst(o => o.ID == charId);
            if (character == null) return;
            Characters.Remove(character);
            TriggerCharacterDelete(character);
        }

        public void OnCharCreate(string json)
        {
            try
            {
                Character character = JsonConvert.DeserializeObject<Character>(json);
                if (character != null)
                {
                    Characters.Add(character);
                    TriggerCharacterCreate(character);
                }
            }
            catch
            {
                // Ignore; needs handling
            }
        }

        internal void SetCharSkills(string charId, string jsonData)
        {
            try
            {
                Character character = Characters.SelectFirst(o => o.ID == charId);
                if (character == null) return;
                JArray array = JArray.Parse(jsonData);
                foreach (var entry in array)
                {
                    if (entry is JObject data)
                    {
                        string type = data["type"]?.ToObject<string>();
                        string name = data["name"]?.ToObject<string>();
                        int idx = data["idx"]?.ToObject<int>() ?? -1;
                        int val = data["val"]?.ToObject<int>() ?? -1;
                        if (type == null || name == null || idx == -1 || val == -1) continue;
                        Skill[] skills = GetSkillArrayForUpdate(character, type);
                        Skill skill = skills[idx];
                        skill.Name = name;
                        skill.Value = val;
                    }
                }

                TriggerCharacterUpdate(character);
            }
            catch
            {
                // Ignore; needs handling
            }
        }

        private Skill[] GetSkillArrayForUpdate(Character character, string type)
        {
            switch (type)
            {
                case "actSkills":
                    return character.ActSkills;
                case "knowledgeSkills":
                    return character.KnowledgeSkills;
                case "socialSkills":
                    return character.SocialSkills;
                default:
                    return new Skill[0];
            }
        }

        internal void SetCharModData(string charId, string key, object val)
        {
            try
            {
                Character character = Characters.SelectFirst(o => o.ID == charId);
                if (character == null) return;
                if (character.ModulesData == null)
                    character.ModulesData = new Dictionary<string, object>();
                if (character.ModulesData.ContainsKey(key))
                    character.ModulesData.Remove(key);
                character.ModulesData.Add(key, val);
                TriggerCharacterUpdate(character);
            }
            catch
            {
                // Ignore; needs handling
            }
        }

        internal void SetCharData(string charId, string key, string val)
        {
            try
            {
                Character character = Characters.SelectFirst(o => o.ID == charId);
                if (character == null) return;
                switch (key)
                {
                    case "xp":
                        character.XP = JsonConvert.DeserializeObject<int>(val);
                        break;
                    case "health":
                        character.Health = JsonConvert.DeserializeObject<int>(val);
                        break;
                    case "name":
                        character.Name = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "gender":
                        character.Gender = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "age":
                        character.Age = JsonConvert.DeserializeObject<int>(val);
                        break;
                    case "stature":
                        character.Stature = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "religion":
                        character.Religion = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "job":
                        character.Job = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "martialStatus":
                        character.MartialStatus = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "inventory":
                        character.Inventory = JsonConvert.DeserializeObject<string>(val);
                        break;
                    case "notes":
                        character.Notes = JsonConvert.DeserializeObject<string>(val);
                        break;
                }
                
                TriggerCharacterUpdate(character);
            }
            catch
            {
                // Ignore; needs handling
            }
        }
    }
}
