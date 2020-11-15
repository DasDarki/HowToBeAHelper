namespace HowToBeAHelper.UI
{
    /// <summary>
    /// The client is the main component of this helper software. From there many parts are being
    /// managed and controlled.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Gets called when the client registers a keydown press.
        /// </summary>
        event Events.Keydown Keydown;
    }
}
