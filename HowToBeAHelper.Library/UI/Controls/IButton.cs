using System;

namespace HowToBeAHelper.UI.Controls
{
    /// <summary>
    /// The button is a control which takes click actions and does something with it.
    /// </summary>
    public interface IButton : IControl
    {
        /// <summary>
        /// The text of the button.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// The background color of the button.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Gets called when someone clicks on the element.
        /// </summary>
        event Action Click;

        /// <summary>
        /// Gets called when someone doubleclicks on the element.
        /// </summary>
        event Action DoubleClick;
    }
}
