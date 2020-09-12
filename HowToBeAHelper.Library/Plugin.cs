namespace HowToBeAHelper
{
    /// <summary>
    /// The main class for every plugin for HowToBeAHelper. This class is the initializer and entry point for the API.
    /// From here every process is getting managed. This class is the only class with direct access to the API.
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// Gets called when the plugin is getting started.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Gets called when the plugin is getting stopped.
        /// </summary>
        public virtual void OnStop() {}

        /// <summary>
        /// Gets called when the plugin is initialized. The init step is shortly before any plugin is getting started.
        /// </summary>
        public virtual void OnInit() {}
    }
}
