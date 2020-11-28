namespace HowToBeAHelper
{
    /// <summary>
    /// The interface for accessing the UI API of HTBAH.
    /// </summary>
    public interface IUI
    {
        /// <summary>
        /// Prints a green success notify to the top-right corner of the UI.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="duration">The duration in milliseconds, how long the notify is shown</param>
        void NotifySuccess(string message, int duration = 3000);

        /// <summary>
        /// Prints a red error notify to the top-right corner of the UI.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="duration">The duration in milliseconds, how long the notify is shown</param>
        void NotifyError(string message, int duration = 3000);

        /// <summary>
        /// Shows a success alert box onto the screen.
        /// </summary>
        /// <param name="text">The text of the alert</param>
        /// <param name="title">The title of the alert</param>
        void AlertSuccess(string text, string title = "Juhu!");

        /// <summary>
        /// Shows a error alert box onto the screen.
        /// </summary>
        /// <param name="text">The text of the alert</param>
        /// <param name="title">The title of the alert</param>
        void AlertError(string text, string title = "Juhu!");
    }
}
