using System;
using System.IO;
using HowToBeAHelper.Model.Characters;
using HowToBeAHelper.Model.Skills;
using HowToBeAHelper.Properties;
using iText.Forms;
using iText.Kernel.Pdf;

namespace HowToBeAHelper.BuiltIn
{
    /// <summary>
    /// This is the helper class for the built in character generation.
    /// </summary>
    internal static class CharacterGenerator
    {
        /// <summary>
        /// Generates a pdf file from the given character. The character data is automatically getting filled into the default character sheet
        /// of "How to be a Hero".
        /// </summary>
        /// <param name="character">The wanted character object</param>
        /// <param name="outputPath">The output path of the pdf file</param>
        internal static void GeneratePdf(Character character, string outputPath)
        {
            PdfDocument document = new PdfDocument(new PdfReader(new MemoryStream(Resources.CharacterTemplate)),
                new PdfWriter(outputPath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(document, false);
            if (form == null) return;
            form.GetField("Portrait_af_image");//TODO
            form.GetField("Name").SetValue(character.Name);
            form.PartialFormFlattening("Name");
            form.GetField("Geschlecht").SetValue(character.Gender);
            form.PartialFormFlattening("Geschlecht");
            form.GetField("Alter").SetValue(character.Age.ToString());
            form.GetField("Lebenspunkte").SetValue(character.Health.ToString());
            form.GetField("Statur").SetValue(character.Stature);
            form.PartialFormFlattening("Statur");
            form.GetField("Religion").SetValue(character.Religion);
            form.PartialFormFlattening("Religion");
            form.GetField("Beruf").SetValue(character.Job);
            form.PartialFormFlattening("Beruf");
            form.GetField("Familienstand").SetValue(character.MartialStatus);
            form.PartialFormFlattening("Familienstand");
            form.GetField("Inventar").SetValue(character.Inventory);
            form.GetField("Anmerkungen").SetValue(character.Notes);
            form.GetField("PunkteGesamt").SetValue("0");
            form.PartialFormFlattening("PunkteGesamt");
            form.GetField("PunkteRest").SetValue("0");
            form.PartialFormFlattening("PunkteRest");
            FillSkills(form, "Handeln", "H", "Geistesblitzpunkte_Handeln", character, character.ActSkills);
            FillSkills(form, "Wissen", "W", "Geistesblitzpunkte_Wissen", character, character.KnowledgeSkills, "G", "", "1.");
            FillSkills(form, "Interagieren", "I", "GBPI", character, character.SocialSkills, "F", "N");
            form.FlattenFields();
            document.Close();
        }

        private static void FillSkills(PdfAcroForm form, string fullname, string name, string brainstormName, Character character, Skill[] skills, string extra1 = "G", 
            string extra2 = "", string extra3 = "")
        {
            int bonus = character.GetSkillsBonus(skills);
            int brainstorm = (int)Math.Round(bonus / 10d);
            for (int i = 0; i < skills.Length; i++)
            {
                Skill skill = skills[i];
                if (string.IsNullOrEmpty(skill.Name)) continue;
                form.GetField(name + extra1 + "B." + (i + 1)).SetValue(name + "HGB." + (i + 1)); //HGB.x
                form.PartialFormFlattening(name + extra1 + "B." + (i + 1));
                form.GetField(name + extra1 + "G" + (i + 1)).SetValue((skill.Value + bonus).ToString()); //HGGx
                form.PartialFormFlattening(name + extra1 + "G" + (i + 1));
                form.GetField(name + "Talent." + extra3 + (i + 1)).SetValue(skill.Name); //HTalent.x = Skillname
                form.PartialFormFlattening(name + "Talent." + extra3 + (i + 1));
                form.GetField(name + extra2 + extra1 + (i + 1)).SetValue(skill.Value.ToString()); //HGx = Skillvalue
                form.PartialFormFlattening(name + extra2 + extra1 + (i + 1));
            }

            form.GetField(fullname).SetValue(bonus.ToString());
            form.PartialFormFlattening(fullname);
            form.GetField(brainstormName).SetValue(brainstorm.ToString());
            form.PartialFormFlattening(brainstormName);
        }
    }
}
