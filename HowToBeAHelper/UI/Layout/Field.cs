namespace HowToBeAHelper.UI.Layout
{
    internal class Field : Parent, IField
    {
        internal Field(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
        }

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID}\", class=\"field {classes}\"></div>";
        }
    }
}
