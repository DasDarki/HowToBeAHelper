using System;
using System.Collections.Generic;
using CefSharp;

namespace HowToBeAHelper.UI.Controls
{
    internal class Select : Control, ISelect
    {
        public int DefaultIndex { get; set; }

        public int CurrentIndex 
        { 
            get => _currentIndex;
            set
            {
                string val = GetValueOfIndex(value);
                if(val == null) return;
                _currentIndex = value;
                MainForm.Instance.Browser.ExecuteScriptAsyncWhenPageLoaded($"ui_SetSelectValue('{ID}', {_currentIndex})");
            }
        }

        private int _currentIndex;

        public bool IsFullwidth
        {
            get => _fullwidth;
            set
            {
                _fullwidth = value;
                if (value)
                {
                    CefUI.AddElementClass(ID + "_parent", "is-fullwidth");
                }
                else
                {
                    CefUI.RemoveElementClass(ID + "_parent", "is-fullwidth");
                }
            }
        }

        private bool _fullwidth;

        public List<string> Items { get; }

        public event Action<string> Change;

        private readonly string _data;

        public Select(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            Items = settings.Items;
            DefaultIndex = 0;
            _currentIndex = DefaultIndex;
            _data = settings.Data;
        }

        protected override string GetInnerHTML(string classes)
        {
            string items = "";
            int index = 0;
            foreach (string item in Items)
            {
                string t = DefaultIndex == index ? "selected" : "";
                items += $"<option value=\"{index}\" {t}>{item}</option>";
                index++;
            }

            return $"<div class=\"select\" id=\"{ID}_parent\"><select class=\"{classes}\" {_data} onchange=\"ui_OnChange('{ID}')\" id=\"{ID}\" >" + items + "</select></div>";
        }

        public override void Reset()
        {
            CurrentIndex = DefaultIndex;
        }

        public void UpdateItems()
        {
            string items = "";
            int index = 0;
            foreach (string item in Items)
            {
                string t = DefaultIndex == index ? "selected" : "";
                items += $"<option value=\"{item}\" {t}>{item}</option>";
                index++;
            }

            CefUI.SetInnerHTML(ID, items);
        }

        internal void TriggerChange(string val)
        {
            int index = Items.IndexOf(val);
            _currentIndex = index;
            Change?.Invoke(val);
        }

        private string GetValueOfIndex(int index)
        {
            return Items.Count > index ? Items[index] : null;
        }
    }
}
