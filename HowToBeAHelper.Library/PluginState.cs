namespace HowToBeAHelper
{
    /// <summary>
    /// This enum defines the state of a loaded plugin.
    /// </summary>
    public enum PluginState
    {
        /// <summary>
        /// Mostly the first state. If none state is set, this will be the state.
        /// </summary>
        Unknown,
        /// <summary>
        /// The plugin is loaded and ready for the starting process.
        /// </summary>
        Loaded,
        /// <summary>
        /// The plugin is started and runs normally.
        /// </summary>
        Started,
        /// <summary>
        /// The plugin was stopped.
        /// </summary>
        Stopped,
        /// <summary>
        /// The plugin failed at some point.
        /// </summary>
        Failed
    }
}
