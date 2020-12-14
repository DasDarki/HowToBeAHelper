using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using HowToBeAHelper.Plugins;
using HowToBeAHelper.Scripting;
using Microsoft.Win32;
using Newtonsoft.Json;

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

        /// <summary>
        /// The system implementiation for the plugins system.
        /// </summary>
        internal static ScriptingSystem System { get; private set; }

        /// <summary>
        /// The settings of the runtime.
        /// </summary>
        internal static Settings Settings { get; set; }

        /// <summary>
        /// If set, the player will automatically try to join in the session when login.
        /// </summary>
        internal static string AutoJoinSession { get; set; } = "";

        internal static PluginManager PluginManager { get; } = new PluginManager();

        private const string UriScheme = "htbah";
        private const string FriendlyName = "HowToBeAHelper";

        /// <summary>
        /// Initializes the app, creates the structure, checks for updates and connects with the network.
        /// </summary>
        /// <returns>True if the app could be initialized successfully</returns>
        internal static bool Init(string[] args)
        {
            RegisterUriScheme();
            UpdateAutoSessionJoin(args);
            AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            DataPath = CreateAppPath("data");
            TempPath = CreateAppPath("temp");
            ConfigPath = CreateAppPath("configs");
            PluginsPath = CreateAppPath("plugins");
            LocalStorage.Init();
            Settings = Settings.Load();
            CharacterManager = new CharacterManager();
            IsAutomaticallyLoggedIn = Load(out StoredUsername, out StoredPassword);
            System = new ScriptingSystem();
            PluginManager.LoadPlugins();
            return true;
        }

        private static void UpdateAutoSessionJoin(string[] args)
        {
            if (args.Length <= 0) return;
            string arg = args[0];
            if (string.IsNullOrEmpty(arg)) return;
            try
            {
                string[] parts = arg.Split('?');
                string url = parts[0].Replace("htbah://", "");
                if (url.ToLower().StartsWith("session"))
                {
                    NameValueCollection queries = HttpUtility.ParseQueryString(parts[1]);
                    if (!queries.AllKeys.Contains("id")) return;
                    string id = queries["id"];
                    string pw = queries.AllKeys.Contains("pw") ? queries["pw"] : "";
                    AutoJoinSession = JsonConvert.SerializeObject(new {id, pw});
                }
            }
            catch
            {
                //Needs: Error handling
            }
        }

        private static void RegisterUriScheme()
        {
            if (!Program.IsElevated()) return;
            using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme))
            {
                string applicationLocation = typeof(Program).Assembly.Location;

                if (key != null)
                {
                    key.SetValue("", "URL:" + FriendlyName);
                    key.SetValue("URL Protocol", "");

                    using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                    {
                        defaultIcon?.SetValue("", applicationLocation + ",1");
                    }

                    using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                    {
                        commandKey?.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                    }
                }
            }
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
