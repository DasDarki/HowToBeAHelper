using System.Collections.Generic;

namespace HowToBeAHelper.UI.Layout
{
    /// <summary>
    /// Represents a parent element. If an element is a parent it can hold other elements
    /// as their children
    /// </summary>
    public interface IParent : IElement
    {
        /// <summary>
        /// A list containing every children of this parent.
        /// </summary>
        List<IElement> Children { get; }

        /// <summary>
        /// Creates an element in this parent and adds it to the children.
        /// </summary>
        /// <typeparam name="T">The type of the element</typeparam>
        /// <param name="id">The id of the element. Must be unique over all elements. If null, the id is auto generated</param>
        /// <param name="settings">The settings of the newly create element</param>
        /// <returns>The created element</returns>
        T Create<T>(string id = null, SetupSettings settings = null) where T : IElement;

        /// <summary>
        /// Resets and clears every element underlying of this parent.
        /// </summary>
        void Reset();
    }
}
