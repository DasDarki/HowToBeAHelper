using System;
using System.IO;
using System.Reflection;

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
        /// The meta of this plugin. Do not manually set.
        /// </summary>
        public PluginMeta Meta { get; set; }

        /// <summary>
        /// The state of this plugin. Do not manually set.
        /// </summary>
        public PluginState State { get; set; } = PluginState.Unknown;

        /// <summary>
        /// The current instance of the running plugin manager. Do not manually set.
        /// </summary>
        public IPluginManager PluginManager { get; set; }

        /// <summary>
        /// The current UI API instance. Do not manually set.
        /// </summary>
        public IUI UI { get; set; }

        /// <summary>
        /// Gets called when the plugin is getting started.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Gets called when the plugin is getting stopped.
        /// </summary>
        public virtual void OnStop() {}

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
    }
}
