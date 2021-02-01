using System;
using System.Collections.Generic;
using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper.UI
{
    internal class Card : Parent, ICard
    {
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                CefUI.SetInnerHTML(ID + "_card_header_text", _header);
            }
        }

        private string _header;
        private readonly Dictionary<string, Action> _footerCallbacks;

        internal Card(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            _header = settings.Text;
            _footerCallbacks = new Dictionary<string, Action>();
        }

        public ICard AddFooterButton(string text, Action click)
        {
            string clickId = Guid.NewGuid().ToString();
            while (_footerCallbacks.ContainsKey(clickId))
            {
                clickId = Guid.NewGuid().ToString();
            }

            _footerCallbacks.Add(clickId, click);
            CefUI.AddCardFooterButton(ID, clickId, text, false);
            return this;
        }

        public ICard AddFooterDangerButton(string text, Action click)
        {
            string clickId = Guid.NewGuid().ToString();
            while (_footerCallbacks.ContainsKey(clickId))
            {
                clickId = Guid.NewGuid().ToString();
            }

            _footerCallbacks.Add(clickId, click);
            CefUI.AddCardFooterButton(ID, clickId, text, true);
            return this;
        }

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID}_card\" class=\"card events-card\">" +
                   $"<header id=\"{ID}_card_header\" class=\"card-header\">" +
                   $"<p id=\"{ID}_card_header_text\">{Header}</p>" +
                   $"</header>" +
                   $"<div class=\"card-table\"><div id=\"{ID}\" class=\"card-content\"></div></div>" +
                   $"<footer id=\"{ID}_card_footer\"></footer></div>";
        }

        public override void Destroy()
        {
            CefUI.CreatedElements.Remove(this);
            CefUI.DestroyElement(ID + "_card");
        }

        internal void TriggerFooterClick(string id)
        {
            if (_footerCallbacks.ContainsKey(id))
            {
                _footerCallbacks[id]();
            }
        }
    }
}
