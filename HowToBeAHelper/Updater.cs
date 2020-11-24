using System.Net;
using AutoUpdaterDotNET;

namespace HowToBeAHelper
{
    internal class Updater
    {
        internal static Changelog Changelog { get; private set; }

        private const string ChangelogDataUrl = "https://eternitylife.de/htbah_changelog.txt";

        internal static bool Start()
        {
            try
            {
                AutoUpdater.Synchronous = true;
                AutoUpdater.Mandatory = true;
                AutoUpdater.UpdateMode = Mode.ForcedDownload;
                bool flag = false;
                AutoUpdater.ApplicationExitEvent += () =>
                {
                    flag = true;
                };
                AutoUpdater.Start("https://eternitylife.de/htbah_update.xml");
                if (flag) return true;
                using (WebClient client = new WebClient())
                {
                    Changelog = Changelog.Parse(client.DownloadString(ChangelogDataUrl));
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
