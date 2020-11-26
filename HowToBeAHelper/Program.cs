using System;
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
            Log.Append("============= NEW START =============");
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            if (!File.Exists(Path.Combine(appPath, "disableupdate")))
            {
                if (Updater.Start())
                {
                    return;
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
