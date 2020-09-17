using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HowToBeAHelper.BuiltIn;
using HowToBeAHelper.Model.Characters;

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
        /// The stored username.
        /// </summary>
        internal static string StoredUsername;

        /// <summary>
        /// The stored password.
        /// </summary>
        internal static string StoredPassword;

        /// <summary>
        /// Whether the user should be logged in automatically or not.
        /// </summary>
        internal static bool IsAutomaticallyLoggedIn { get; set; }

        /// <summary>
        /// The character manager of the local storage.
        /// </summary>
        internal static CharacterManager CharacterManager { get; set; }

        internal static Settings Settings { get; set; }

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
            Settings = Settings.Load();
            CharacterManager = new CharacterManager();
            IsAutomaticallyLoggedIn = Load(out StoredUsername, out StoredPassword);
            return true;
        }

        private static bool Load(out string username, out string password)
        {
            username = null;
            password = null;
            if (!File.Exists(Path.Combine(DataPath, "loginstorage.toml"))) return false;
            try
            {
                var storage = File.ReadAllLines(Path.Combine(DataPath, "loginstorage.toml"));
                username = storage[0];
                password = storage[1];
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void Save(string username, string password)
        {
            try
            {
                File.WriteAllLines(Path.Combine(DataPath, "loginstorage.toml"), new List<string>
                {
                    username, password
                });
            }
            catch
            {
                //Ignore
            }
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

        internal static void DeleteLogin()
        {
            try
            {
                File.Delete(Path.Combine(DataPath, "loginstorage.toml"));
            }
            catch
            {
                //Ignore
            }
        }
    }
}
