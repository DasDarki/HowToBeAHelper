using System;
using System.IO;
using System.Windows.Forms;
using HowToBeAHelper.Modules;

namespace HowToBeAHelper.Packer
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string xmlFile = GetXML(args);
                if (string.IsNullOrEmpty(xmlFile) || !File.Exists(xmlFile))
                {
                    MessageBox.Show("Ein Modul brauch eine XML-Datei, welche als Modul-Meta dient!", "Fehler!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string xml = File.ReadAllText(xmlFile);
                Module module = Module.TryLoad(xml);
                if (module?.Meta == null || string.IsNullOrEmpty(module.Meta.Name))
                {
                    MessageBox.Show("Die Modul-Meta ist ungültig! Bitte überprüfe den Syntax dieser!", "Fehler!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string mdFile = GetMarkDown(args);
                string ruleset = null;
                if (!string.IsNullOrEmpty(mdFile) && File.Exists(mdFile))
                {
                    ruleset = File.ReadAllText(mdFile);
                }

                ModulePack pack = new ModulePack {Content = xml, Ruleset = ruleset};
                string basePath = Path.GetDirectoryName(Application.ExecutablePath) ?? "";
                SaveFileDialog dialog = new SaveFileDialog
                {
                    InitialDirectory = basePath, RestoreDirectory = true,
                    Filter = "HTBAH-Modul | *.htbam"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, ModulePack.Serialize(pack));
                    MessageBox.Show("Modul wurde erfolgreich erstellt!", "Erfolg!", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unbekannter Fehler!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetMarkDown(string[] args)
        {
            foreach (string file in args)
            {
                if (Path.GetExtension(file).ToLower().EndsWith("md"))
                {
                    return file;
                }
            }

            return null;
        }

        private static string GetXML(string[] args)
        {
            foreach (string file in args)
            {
                if (Path.GetExtension(file).ToLower().EndsWith("xml"))
                {
                    return file;
                }
            }

            return null;
        }
    }
}
