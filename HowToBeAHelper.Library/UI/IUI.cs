using System;
using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper.UI
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
        /// Shows an error alert box onto the screen.
        /// </summary>
        /// <param name="text">The text of the alert</param>
        /// <param name="title">The title of the alert</param>
        void AlertError(string text, string title = "Huch?");

        /// <summary>
        /// Shows a confirmation alert box onto the screen. I
        /// </summary>
        /// <param name="text">The text of the alert</param>
        /// <param name="callback">The callback which gets called back after confirmation</param>
        /// <param name="title">The title of the alert</param>
        /// <param name="yes">The text of the "yes"-button</param>
        /// <param name="no">The text of the "no"-button</param>
        void AlertConfirm(string text, Action<bool> callback, string title = "Sicher?", string yes = "JA!", string no = "NEEEE!!!");

        /// <summary>
        /// Shows the loader on the screen.
        /// </summary>
        void ShowLoader();

        /// <summary>
        /// Hides the loader from the screen.
        /// </summary>
        void HideLoader();

        /// <summary>
        /// Registers a new modal in the system.
        /// </summary>
        /// <param name="title">The title of the modal</param>
        /// <returns>The newly created modal</returns>
        IModal RegisterModal(string title = "");

        /// <summary>
        /// Gets called when a specific container of the main UI gets loaded. Can be used for modifying the container.
        /// Removals cannot be done for native UI elements.
        /// </summary>
        event Action<ContainerType, IParent> ContainerLoad;
    }
}
