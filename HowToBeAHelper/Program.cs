using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;

namespace HowToBeAHelper
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            if (!File.Exists(Path.Combine(appPath, "disableupdate")))
            {
                string updaterPath = Path.Combine(appPath,
                    "HowToBeAHelper.Updater.exe");
                if (File.Exists(updaterPath))
                {
                    if (Updater.Start())
                    {
                        Process.Start(new ProcessStartInfo(updaterPath)
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Es wurde ein Problem bei der Installation bemerkt! Wir empfehlen eine Neuinstallation!",
                        "Keinen Updater gefunden!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (Bootstrap.Init(args))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(MainForm.Instance = new MainForm());
            }
        }

        internal static bool IsElevated()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
