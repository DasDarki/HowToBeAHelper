using System;

namespace HowToBeAHelper.UI.Controls
{
    /// <summary>
    /// The checkbox is a true or false control which can be checked.
    /// </summary>
    public interface ICheckbox : IControl
    {
        /// <summary>
        /// Whether the checkbox is checked (true) or not (false).
        /// </summary>
        bool Checked { get; set; }

        /// <summary>
        /// The default value of this checkbox.
        /// </summary>
        bool Default { get; set; }

        /// <summary>
        /// The text of the checkbox which is beside the checkbox.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets called when the checkbox states changed.
        /// </summary>
        event Action<bool> Change;
    }
}
