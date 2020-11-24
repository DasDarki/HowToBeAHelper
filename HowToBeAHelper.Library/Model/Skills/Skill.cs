using Newtonsoft.Json;

namespace HowToBeAHelper.Model.Skills
{
    /// <summary>
    /// A skill describes the things a character can do.
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// The category of this skill.
        /// </summary>
        [JsonProperty("category")]
        public SkillCategory Category { get; }

        /// <summary>
        /// The name of the skill.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the skill.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// The base constructor which offers all accessible variable initialization.
        /// </summary>
        /// <param name="category">The category is required in order to initialize a skill</param>
        /// <param name="name">The name is required, too</param>
        /// <param name="value">The value is optional and is 0 by default</param>
        public Skill(SkillCategory category, string name, int value = 0)
        {
            Category = category;
            Name = name;
            Value = value;
        }
    }
}
