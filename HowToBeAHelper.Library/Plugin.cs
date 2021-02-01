using System;
using System.IO;
using System.Reflection;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper
{
    /// <summary>
    /// The main class for every plugin for HowToBeAHelper. This class is the initializer and entry point for the API.
    /// From here every process is getting managed. This class is the only class with direct access to the API.
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// The internal log path of this plugin.
        /// </summary>
        private string _logPath;

        /// <summary>
        /// The meta of this plugin.
        /// </summary>
        public PluginMeta Meta
        {
            get => _meta;
            set
            {
                if (AssertCaller())
                {
                    _meta = value;
                }
            }
        }

        private PluginMeta _meta;

        /// <summary>
        /// The state of this plugin.
        /// </summary>
        public PluginState State
        {
            get => _state;
            set
            {
                if (AssertCaller())
                {
                    _state = value;
                }
            }
        }

        private PluginState _state = PluginState.Unknown;

        /// <summary>
        /// The current instance of the running plugin manager.
        /// </summary>
        public IPluginManager PluginManager
        {
            get => _pluginManager;
            set
            {
                if (AssertCaller())
                {
                    _pluginManager = value;
                }
            }
        }

        private IPluginManager _pluginManager;

        /// <summary>
        /// The current UI API instance.
        /// </summary>
        public IUI UI
        {
            get => _ui;
            set
            {
                if (AssertCaller())
                {
                    _ui = value;
                }
            }
        }

        private IUI _ui;

        /// <summary>
        /// The page owned by this plugin.
        /// </summary>
        public IParent Page
        {
            get => _page;
            set
            {
                if (AssertCaller())
                {
                    _page = value;
                }
            }
        }

        private IParent _page;

        /// <summary>
        /// The currently running system instance.
        /// </summary>
        public ISystem System
        {
            get => _system;
            set
            {
                if (AssertCaller())
                {
                    _system = value;
                }
            }
        }

        private ISystem _system;

        /// <summary>
        /// Gets called when the plugin is getting started.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Gets called when the plugin is getting stopped.
        /// </summary>
        public virtual void OnStop() {}

        /// <summary>
        /// Gets called when the UI is loaded and the own page can be initialized.
        /// </summary>
        public virtual void OnPageLoad() {}

        /// <summary>
        /// Logs a message to the plugins log file in the root directory of HTBAH.
        /// Arguments can be entered via the placeholder format of the <see cref="string.Format(string,object)"/>
        /// method.
        /// </summary>
        /// <param name="message">The message which should be appended</param>
        /// <param name="args">The arguments which will be filled into the message</param>
        public void Log(string message, params object[] args)
        {
            try
            {
                File.AppendAllLines(GetLogPath(), new[] { $"[{DateTime.Now:G}] " + string.Format(message, args) });
            }
            catch
            {
                //ignore
            }
        }

        private string GetLogPath()
        {
            if (_logPath != null) return _logPath;
            _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                "logs");
            Directory.CreateDirectory(_logPath);
            return _logPath = Path.Combine(_logPath, Meta.Display + ".txt");
        }

        /// <summary>
        /// Checks the calling assembly if it is allowed to call this method.
        /// </summary>
        /// <returns>True, if the assembly is allowed to set</returns>
        internal static bool AssertCaller()
        {
            string location = Assembly.GetCallingAssembly().Location;
            return Path.GetFileNameWithoutExtension(location) == "HowToBeAHelper.Library";
        }
    }
}
