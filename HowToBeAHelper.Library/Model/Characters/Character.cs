using System;
using HowToBeAHelper.Model.Skills;
using Newtonsoft.Json;

namespace HowToBeAHelper.Model.Characters
{
    /// <summary>
    /// The data model for the character.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// The name of the character.
        /// </summary>
        [JsonProperty("uuid")]
        public string ID { get; set; }

        /// <summary>
        /// The name of the character.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The age of the character.
        /// </summary>
        [JsonProperty("age")]
        public int Age { get; set; } = 0;

        /// <summary>
        /// The xp of the character.
        /// </summary>
        [JsonProperty("xp")]
        public int XP { get; set; } = 0;

        /// <summary>
        /// The gender of the character.
        /// </summary>
        [JsonProperty("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// The health points of the character.
        /// </summary>
        [JsonProperty("health")]
        public int Health { get; set; } = 100;

        /// <summary>
        /// The build of the character.
        /// </summary>
        [JsonProperty("stature")]
        public string Stature { get; set; } = "";

        /// <summary>
        /// The religion of the character.
        /// </summary>
        [JsonProperty("religion")]
        public string Religion { get; set; } = "";

        /// <summary>
        /// The job of the character.
        /// </summary>
        [JsonProperty("job")]
        public string Job { get; set; } = "";

        /// <summary>
        /// The martial status of the character.
        /// </summary>
        [JsonProperty("martialStatus")]
        public string MartialStatus { get; set; } = "";

        /// <summary>
        /// The inventory of the character.
        /// </summary>
        [JsonProperty("inventory")]
        public string Inventory { get; set; } = "";

        /// <summary>
        /// The notes of the character.
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; } = "";

        /// <summary>
        /// An 10-length array containing skills for the category <see cref="SkillCategory.Handeln"/>.
        /// </summary>
        [JsonProperty("actSkills")]
        public Skill[] ActSkills { get; set; } = GenerateSkills(SkillCategory.Handeln);

        /// <summary>
        /// An 10-length array containing skills for the category <see cref="SkillCategory.Wissen"/>.
        /// </summary>
        [JsonProperty("knowledgeSkills")]
        public Skill[] KnowledgeSkills { get; set; } = GenerateSkills(SkillCategory.Wissen);

        /// <summary>
        /// An 10-length array containing skills for the category <see cref="SkillCategory.Soziales"/>.
        /// </summary>
        [JsonProperty("socialSkills")]
        public Skill[] SocialSkills { get; set; } = GenerateSkills(SkillCategory.Soziales);

        /// <summary>
        /// The points the character has left for skills.
        /// </summary>
        [JsonProperty("pointsLeft")] 
        public int PointsLeft { get; set; } = 400;

        /// <summary>
        /// Calculates the bonus for the given skills.
        /// </summary>
        /// <param name="skills">The skills group</param>
        /// <returns>The bonus for the given skills</returns>
        public int GetSkillsBonus(Skill[] skills)
        {
            int bonus = 0;
            foreach (var skill in skills)
            {
                bonus += skill.Value;
            }

            return (int) Math.Round(bonus / 10d);
        }

        /// <summary>
        /// Generates a default skills array with no entered skills. The skill names are empty so that the skills will be ignored.
        /// </summary>
        private static Skill[] GenerateSkills(SkillCategory category)
        {
            Skill[] skills = new Skill[10];
            for (int i = 0; i < 10; i++)
            {
                skills[i] = new Skill(category, "");
            }

            return skills;
        }
    }
}
