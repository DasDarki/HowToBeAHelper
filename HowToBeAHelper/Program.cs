using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using HowToBeAHelper.Invite;

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
#if !DEBUG
                if (Updater.Start())
                {
                    return;
                } 
#endif
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            object invite = null;
            if (args.Length > 0)
            {
                invite = Bootstrap.GenerateInvite(args[0]);
            }

            if (SessionJoinHandler.Handle(invite))
                StartApp();
        }

        private static void StartApp()
        {
            if (Bootstrap.Init())
            {
                Application.Run(new MainForm());
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
