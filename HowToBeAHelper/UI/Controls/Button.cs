using System;

namespace HowToBeAHelper.UI.Controls
{
    internal class Button : Control, IButton
    {
        public string Text 
        { 
            get => _text;
            set
            {
                _text = value;
                CefUI.SetInnerHTML(ID, _text);
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                RemoveClass("is-" + _color.GetName().ToLower());
                _color = value;
                AddClass("is-" + _color.GetName().ToLower());
            }
        }

        private Color _color;
        private string _text;

        public event Action Click;
        public event Action DoubleClick;

        public Button(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            _text = settings.Text;
            _color = settings.Color;
        }

        protected override string GetInnerHTML(string classes)
        {
            return $"<button id=\"{ID}\" class=\"button is-{_color.GetName().ToLower()} {classes}\" onclick=\"bridge.ui_OnClick('{ID}')\" ondblclick=\"bridge.ui_OnDoubleClick('{ID}')\">{Text}</button>";
        }

        public override void Reset()
        {
        }

        internal void TriggerClick()
        {
            Click?.Invoke();
        }

        internal void TriggerDoubleClick()
        {
            DoubleClick?.Invoke();
        }
    }
}
