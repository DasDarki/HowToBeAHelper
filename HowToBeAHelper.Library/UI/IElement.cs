using System;

namespace HowToBeAHelper.UI
{
    /// <summary>
    /// The base element offering base functionality.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// The id of the element.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The parent of this element.
        /// </summary>
        IElement Parent { get; }

        /// <summary>
        /// Whether the element is visible or not.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Destroys the element and removes it from the UI.
        /// </summary>
        void Destroy();
        
        /// <summary>
        /// Removes the class from the CSS class list of this element.
        /// </summary>
        /// <param name="clazz">The CSS class to remove</param>
        void RemoveClass(string clazz);

        /// <summary>
        /// Adds the class to the CSS class list of this element.
        /// </summary>
        /// <param name="clazz">The CSS class to add</param>
        void AddClass(string clazz);

        /// <summary>
        /// Checks if the given class is a CSS class of this element.
        /// </summary>
        /// <param name="clazz">The CSS class to check</param>
        /// <returns>True, if the CSS class list of this element is containing the given class</returns>
        bool HasClass(string clazz);

        /// <summary>
        /// Sets a CSS key to given value. Look at jquery wiki for more information:<br/>
        /// https://api.jquery.com/css/
        /// </summary>
        /// <param name="key">The property key name</param>
        /// <param name="value">The value to be set to</param>
        void Style(string key, object value);
    }
}
