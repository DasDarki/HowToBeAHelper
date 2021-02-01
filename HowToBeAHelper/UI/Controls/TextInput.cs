namespace HowToBeAHelper.UI.Controls
{
    internal class TextInput : Input<string>, ITextInput
    {
        public TextInput(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
        }

        public override void Reset()
        {
            Value = "";
        }

        protected override string ToValue(string raw)
        {
            return raw;
        }

        protected override string FromValue(string value)
        {
            return value;
        }

        protected override string GetInnerTypes()
        {
            return "type=\"text\"";
        }
    }
}
