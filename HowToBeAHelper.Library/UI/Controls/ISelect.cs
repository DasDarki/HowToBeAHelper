using System;
using System.Collections.Generic;

namespace HowToBeAHelper.UI.Controls
{
    /// <summary>
    /// The select offers a combo box where the user can choose from specific options.
    /// </summary>
    public interface ISelect : IControl
    {
        /// <summary>
        /// The current index of the select.
        /// </summary>
        int CurrentIndex { get; set; }

        /// <summary>
        /// The default index of the select.
        /// </summary>
        int DefaultIndex { get; set; }

        /// <summary>
        /// A list containing every item in the select.
        /// </summary>
        List<string> Items { get; }

        /// <summary>
        /// Whether the row takes the fullwidth or not.
        /// </summary>
        bool IsFullwidth { get; set; }

        /// <summary>
        /// Gets called when the select value changed.
        /// </summary>
        event Action<string> Change; 

        /// <summary>
        /// Updates all items in the select.
        /// </summary>
        void UpdateItems();
    }
}
