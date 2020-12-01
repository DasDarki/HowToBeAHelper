namespace HowToBeAHelper.UI
{
    internal abstract class Control : Element, IControl
    {
        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                CefUI.SetInnerHTML(ID + "_label", _label);
            }
        }

        private string _label;

        internal Control(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            _label = settings.Label;
        }

        protected abstract string GetInnerHTML(string classes);

        public abstract void Reset();

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID + "_control"}\" class=\"control\"><label id=\"{ID + "_label"}\">{_label}</label>{GetInnerHTML(classes)}</div>";
        }

        public override void Destroy()
        {
            CefUI.CreatedElements.Remove(this);
            CefUI.DestroyElement(ID);
            CefUI.DestroyElement(ID + "_label");
            CefUI.DestroyElement(ID + "_control");
        }
    }
}
