namespace HowToBeAHelper.UI.Layout
{
    internal class Column : Parent, IColumn
    {
        public Column(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
        }

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID}\", class=\"column {classes}\"></div>";
        }
    }
}
