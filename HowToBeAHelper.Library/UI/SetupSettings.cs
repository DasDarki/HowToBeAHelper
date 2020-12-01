using System.Collections.Generic;
using HowToBeAHelper.UI.Controls;

namespace HowToBeAHelper.UI
{
    /// <summary>
    /// The class represents needed settings to setup specific elements.
    /// </summary>
    public class SetupSettings
    {
        /// <summary>
        /// All CSS classes which will be appended to the element.
        /// </summary>
        public List<string> Classes { get; } = new List<string>();

        /// <summary>
        /// The label of this settings instance.
        /// </summary>
        public string Label => _label;

        /// <summary>
        /// The text of this settings instance.
        /// </summary>
        public string Text => _text;

        /// <summary>
        /// The color of this settings instance.
        /// </summary>
        public Color Color => _color;

        /// <summary>
        /// The items of this settings instance.
        /// </summary>
        public List<string> Items => _items;

        private List<string> _items = new List<string>();
        private string _label, _text;
        private Color _color = Color.Primary;

        /// <summary>
        /// Sets the CSS classes array for the new element.
        /// </summary>
        /// <param name="classes">The CSS classes to be set</param>
        /// <returns>This instance of setup settings</returns>
        public SetupSettings AddClasses(params string[] classes)
        {
            Classes.AddRange(classes);
            return this;
        }

        /// <summary>
        /// Sets the items of a valid <see cref="ISelect"/>.
        /// </summary>
        /// <param name="items">The items to be set</param>
        /// <returns>This instance of setup settings</returns>
        public SetupSettings SetItems(List<string> items)
        {
            _items = items;
            return this;
        }

        /// <summary>
        /// Sets the label of a valid <see cref="IControl"/>.
        /// </summary>
        /// <param name="label">The label text to be set</param>
        /// <returns>This instance of setup settings</returns>
        public SetupSettings SetLabel(string label)
        {
            _label = label;
            return this;
        }

        /// <summary>
        /// Sets the color of a valid <see cref="IButton"/>.
        /// </summary>
        /// <param name="color">The color to be set</param>
        /// <returns>This instance of setup settings</returns>
        public SetupSettings SetColor(Color color)
        {
            _color = color;
            return this;
        }

        /// <summary>
        /// Sets the text of a valid <see cref="IButton"/> or <see cref="ICard"/> as header
        /// or <see cref="IInput{T}"/> as placeholder or <see cref="ICheckbox"/>.
        /// </summary>
        /// <param name="text">The text to be set</param>
        /// <returns>This instance of setup settings</returns>
        public SetupSettings SetText(string text)
        {
            _text = text;
            return this;
        }

        /// <summary>
        /// Creates a default instance of setup settings with just CSS classes.
        /// </summary>
        /// <param name="classes">The wanted CSS classes</param>
        /// <returns>The newly created settings</returns>
        public static SetupSettings Default(params string[] classes)
        {
            return new SetupSettings().AddClasses(classes);
        }
    }
}
