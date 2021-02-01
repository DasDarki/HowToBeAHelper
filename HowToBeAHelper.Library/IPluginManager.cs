using System.Collections.Generic;

namespace HowToBeAHelper
{
    /// <summary>
    /// The plugin manager manages plugins, starts them, stops them and identifies them. It is also a handler
    /// for specific parts of the plugin process as well as the bridge between API and plugins.
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// A list containing every plugin which was at least once loaded while the runtime is running.
        /// </summary>
        IReadOnlyList<Plugin> Plugins { get; }

        /// <summary>
        /// Loads a plugin from an external source. 
        /// </summary>
        /// <param name="name">The filename or directory name starting at the plugins main folder</param>
        /// <returns>The loaded plugin instance</returns>
        Plugin LoadPlugin(string name);

        /// <summary>
        /// Tries to start the given plugin and returns the success level.
        /// </summary>
        /// <param name="plugin">The plugin which should be started</param>
        /// <returns>True, if the starting process was successful</returns>
        bool StartPlugin(Plugin plugin);

        /// <summary>
        /// Stops the given plugin.
        /// </summary>
        /// <param name="plugin">The plugin which should be started</param>
        /// <param name="clean">If true, the plugin will be deleted from the plugins list</param>
        void StopPlugin(Plugin plugin, bool clean = false);

        /// <summary>
        /// Gets the plugin by the given id and returns it.
        /// </summary>
        /// <param name="id">The id of the plugin</param>
        /// <returns>The plugin which is associated with the given id, or null if nothing was found</returns>
        Plugin GetPlugin(string id);
    }
}
