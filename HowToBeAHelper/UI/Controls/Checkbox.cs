using System;
using CefSharp;

namespace HowToBeAHelper.UI.Controls
{
    internal class Checkbox : Control, ICheckbox
    {
        public bool Default { get; set; }

        public bool Checked
        {
            get
            {
                JavascriptResponse response = MainForm.Instance.Browser.EvaluateScriptAsync($"document.getElementById(`{ID}_val`).checked;")
                    .GetAwaiter().GetResult();
                string raw = response.Result as string;
                if (string.IsNullOrEmpty(raw)) return Default;
                return raw == "true";
            }
            set =>
                MainForm.Instance.Browser
                    .ExecuteScriptAsyncWhenPageLoaded($"document.getElementById(`{ID}_val`).checked = " +
                                                      (value ? "true" : "false"));
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                CefUI.SetInnerHTML(ID + "_text", _text);
            }
        }

        private string _text;

        public event Action<bool> Change;

        public Checkbox(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            _text = settings.Text;
        }

        protected override string GetInnerHTML(string classes)
        {
            return $"<label id=\"{ID}\"><input onclick=\"ui_OnCheckboxChange('{ID}')\" type=\"checkbox\" id=\"{ID}_val\"><span id=\"{ID}_text\">{Text}</span></label>";
        }

        public override void Reset()
        {
            Checked = Default;
        }

        internal void TriggerChange(bool v)
        {
            Change?.Invoke(v);
        }
    }
}
