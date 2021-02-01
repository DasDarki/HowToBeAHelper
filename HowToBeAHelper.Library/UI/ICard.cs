using System;
using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper.UI
{
    /// <summary>
    /// The card is a group element which displays the group in a card format with header, body and footer.
    /// </summary>
    public interface ICard : IParent
    {
        /// <summary>
        /// The header title of the card.
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Adds a button to the footer of this card.
        /// </summary>
        /// <param name="text">The text of the button</param>
        /// <param name="click">The action which gets called when clicking the button</param>
        /// <returns>This card instance</returns>
        ICard AddFooterButton(string text, Action click);

        /// <summary>
        /// Adds a danger button to the footer of this card.
        /// </summary>
        /// <param name="text">The text of the button</param>
        /// <param name="click">The action which gets called when clicking the button</param>
        /// <returns>This card instance</returns>
        ICard AddFooterDangerButton(string text, Action click);
    }
}
