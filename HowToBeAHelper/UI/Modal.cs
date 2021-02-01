using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper.UI
{
    internal class Modal : Parent, IModal
    {
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                CefUI.SetInnerHTML(ID + "_title", _title);
            }
        }

        private string _title;

        internal Modal(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
            DenyDestroy = true;
        }

        public void Open()
        {
            CefUI.AddElementClass(ID + "_modal", "is-active");
        }

        public void Close()
        {
            CefUI.RemoveElementClass(ID + "_modal", "is-active");
        }

        internal override string GetHTML(string classes)
        {
            return @"<div class=""modal"" id=""%MODAL_ID%_modal"">
        <div class=""modal-background""></div>
        <div class=""modal-card"">
            <header class=""modal-card-head"">
                <p class=""modal-card-title"" id=""%MODAL_ID%_title"">%MODAL_TITLE%</p>
                <button class=""delete"" aria-label=""close""
                        onclick=""$('#%MODAL_ID%_modal').removeClass('is-active')""></button>
            </header>
            <section class=""modal-card-body"" id=""%MODAL_ID%"">
                
            </section>
        </div>
    </div>".Replace("%MODAL_ID%", ID).Replace("%MODAL_TITLE%", Title);
        }
    }
}
