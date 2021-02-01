using System.Collections.Generic;

namespace HowToBeAHelper.UI.Layout
{
    internal class Parent : Element, IParent
    {
        public IReadOnlyList<IElement> Children => InternalChildren.AsReadOnly();

        internal List<IElement> InternalChildren { get; }

        internal Parent(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            InternalChildren = new List<IElement>();
        }

        public T Create<T>(string id = null, SetupSettings settings = null) where T : IElement
        {
            return (T) CefUI.CreateElement<T>(this, id ?? CefUI.GenerateID(), settings ?? new SetupSettings());
        }

        public void Reset()
        {
            InternalChildren.SafeForEach(element =>
            {
                switch (element)
                {
                    case IControl control:
                        control.Reset();
                        break;
                    case IParent parent:
                        parent.Reset();
                        break;
                }
            });
        }

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID}\" class=\"{classes}\"></div>";
        }
    }
}
