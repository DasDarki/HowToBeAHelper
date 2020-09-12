using System.IO;
using System.Reflection;

namespace HowToBeAHelper
{
    internal class Bootstrap
    {
        /// <summary>
        /// The path to the app directory.
        /// </summary>
        internal static string AppPath { get; private set; }

        /// <summary>
        /// The path to the data directory of the app.
        /// </summary>
        internal static string DataPath { get; private set; }

        /// <summary>
        /// The path to the temp directory of the app.
        /// </summary>
        internal static string TempPath { get; private set; }

        /// <summary>
        /// The path to the config files of the app.
        /// </summary>
        internal static string ConfigPath { get; private set; }

        /// <summary>
        /// The path to the plugins folder of the app.
        /// </summary>
        internal static string PluginsPath { get; private set; }

        /// <summary>
        /// Initializes the app, creates the structure, checks for updates and connects with the network.
        /// </summary>
        /// <returns>True if the app could be initialized successfully</returns>
        internal static bool Init()
        {
            AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            DataPath = CreateAppPath("data");
            TempPath = CreateAppPath("temp");
            ConfigPath = CreateAppPath("configs");
            PluginsPath = CreateAppPath("plugins");
            return true;
        }

        /// <summary>
        /// Creates a path beginning from the app path.
        /// </summary>
        /// <param name="name">The name of the new directory</param>
        /// <returns>The path as string</returns>
        private static string CreateAppPath(string name)
        {
            string path = Path.Combine(AppPath, name);
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
