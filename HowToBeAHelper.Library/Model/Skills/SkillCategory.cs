namespace HowToBeAHelper.Model.Skills
{
    /// <summary>
    /// The 3 main categories of the skill groups.
    /// </summary>
    public enum SkillCategory
    {
        /// <summary>
        /// Everything what a character does with the skill, counts as "Handeln"
        /// </summary>
        Handeln = 0, 
        /// <summary>
        /// Everything what a character knows with the skill, counts as "Wissen"
        /// </summary>
        Wissen = 1, 
        /// <summary>
        /// Every interaction of a character with the skill, counts as "Soziales"
        /// </summary>
        Soziales = 2
    }
}
