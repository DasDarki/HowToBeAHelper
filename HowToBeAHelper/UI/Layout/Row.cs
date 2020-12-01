namespace HowToBeAHelper.UI.Layout
{
    internal class Row : Parent, IRow
    {
        public bool IsGapless
        {
            get => HasClass("is-gapless");
            set
            {
                if (value)
                {
                    AddClass("is-gapless");
                }
                else
                {
                    RemoveClass("is-gapless");
                }
            }
        }

        public bool IsCentered
        {
            get => HasClass("is-centered");
            set
            {
                if (value)
                {
                    AddClass("is-centered");
                }
                else
                {
                    RemoveClass("is-centered");
                }
            }
        }

        public bool IsFullwidth
        {
            get => HasClass("is-fullwidth");
            set
            {
                if (value)
                {
                    AddClass("is-fullwidth");
                }
                else
                {
                    RemoveClass("is-fullwidth");
                }
            }
        }

        internal Row(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
        }

        internal override string GetHTML(string classes)
        {
            return $"<div id=\"{ID}\", class=\"columns {classes}\"></div>";
        }
    }
}
