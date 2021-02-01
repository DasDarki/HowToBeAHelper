namespace HowToBeAHelper.UI.Layout
{
    internal class FieldBody : Parent, IFieldBody
    {
        public FieldBody(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
        }

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID}\" class=\"field-body {classes}\"></div>";
        }
    }
}
