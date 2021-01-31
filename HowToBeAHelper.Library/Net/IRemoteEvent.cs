namespace HowToBeAHelper.Net
{
    /// <summary>
    /// The remote event allows managing of specific areas when networking with the master server.
    /// </summary>
    public interface IRemoteEvent
    {
        /// <summary>
        /// Sets the parameter of this remote event.
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        /// <returns>This instance of the remote event</returns>
        IRemoteEvent Params(params object[] args);

        /// <summary>
        /// Sends the remote event to the master for the processing.
        /// </summary>
        void Send();
    }
}
