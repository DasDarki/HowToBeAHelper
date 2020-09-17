using System.IO;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;

namespace HowToBeAHelper
{
    internal class Updater
    {
        internal const string CurrentVersion = "1.0.0";

        internal static Changelog Changelog { get; private set; }

        private const string UpdateDataUrl = "https://eternitylife.de/htbah_update.json";

        [JsonProperty("version")]
        private string Version { get; set; }

        [JsonProperty("file")]
        private string FileUrl { get; set; }

        [JsonProperty("changelog")]
        private string ChangelogUrl { get; set; }

        internal static bool Start()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    Updater data = JsonConvert.DeserializeObject<Updater>(client.DownloadString(UpdateDataUrl));
                    if (data.Version != CurrentVersion)
                    {
                        client.DownloadFile(data.FileUrl,
                            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                                "update.zip"));
                        return true;
                    }

                    Changelog = Changelog.Parse(client.DownloadString(data.ChangelogUrl));
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
