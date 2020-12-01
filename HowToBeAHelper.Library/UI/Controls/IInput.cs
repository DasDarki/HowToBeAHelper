using System;

namespace HowToBeAHelper.UI.Controls
{
    /// <summary>
    /// The base element for every input. The base input can't be created but offers the basic
    /// functionality. From here other inputs are derived.
    /// </summary>
    public interface IInput<T> : IControl
    {
        /// <summary>
        /// The placeholder of the input.
        /// </summary>
        string Placeholder { get; set; }

        /// <summary>
        /// The value of the input.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Gets called when the input changes.
        /// </summary>
        event Action<T> Change;

        /// <summary>
        /// Gets called when the input looses focus.
        /// </summary>
        event Action FocusOut;
    }
}
