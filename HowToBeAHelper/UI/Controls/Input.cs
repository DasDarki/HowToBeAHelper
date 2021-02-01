using System;
using CefSharp;

namespace HowToBeAHelper.UI.Controls
{
    internal abstract class Input<T> : Control, IInput<T>
    {
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value;
                CefUI.SetElementAttribute(ID, "placeholder", _placeholder);
            }
        }

        public string Data { get; set; } = null;

        private string _placeholder;

        public T Value
        {
            get
            {
                JavascriptResponse response = MainForm.Instance.Browser.EvaluateScriptAsync($"document.getElementById(`{ID}`).value;")
                    .GetAwaiter().GetResult();
                return ToValue(response.Result as string);
            }
            set =>
                MainForm.Instance.Browser
                    .ExecuteScriptAsyncWhenPageLoaded($"document.getElementById(`{ID}`).value = " + FromValue(value));
        }

        public event Action<T> Change;
        public event Action FocusOut;
        public event Action<T> Timeout;

        private readonly string _data;

        protected Input(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            _placeholder = settings.Text;
            _data = settings.Data ?? "";
        }

        protected abstract T ToValue(string raw);

        protected abstract string FromValue(T value);

        protected abstract string GetInnerTypes();

        protected override string GetInnerHTML(string classes)
        {
            return $"<input {_data} class=\"input {classes}\" id=\"{ID}\" onpaste=\"ui_OnChange('{ID}')\" onkeyup=\"ui_OnChange('{ID}')\" onchange=\"ui_OnChange('{ID}')\" oninput=\"ui_OnChange('{ID}')\" placeholder=\"{Placeholder}\" onfocusout=\"ui_OnFocusOut('{ID}')\" {GetInnerTypes()}>";
        }

        internal void AddFocusOut(Action action)
        {
            FocusOut += action;
        }

        internal void TriggerFocusOut()
        {
            FocusOut?.Invoke();
        }

        internal void TriggerTimeout(string raw)
        {
            Timeout?.Invoke(ToValue(raw));
        }

        internal void TriggerChange(string raw)
        {
            Change?.Invoke(ToValue(raw));
        }
    }
}
