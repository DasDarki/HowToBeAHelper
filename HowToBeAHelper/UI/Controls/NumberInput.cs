using System.Globalization;

namespace HowToBeAHelper.UI.Controls
{
    internal class NumberInput : Input<double>, INumberInput
    {
        public double Step
        {
            get => _step;
            set
            {
                _step = value;
                CefUI.SetElementAttribute(ID, "step", _step.ToString(CultureInfo.InvariantCulture));
            }
        }

        private double _step;

        public double Min
        {
            get => _min;
            set
            {
                _min = value;
                CefUI.SetElementAttribute(ID, "min", _min.ToString(CultureInfo.InvariantCulture));
            }
        }

        private double _min;

        public double Max
        {
            get => _max;
            set
            {
                _max = value;
                CefUI.SetElementAttribute(ID, "max", _max.ToString(CultureInfo.InvariantCulture));
            }
        }

        private double _max;

        public NumberInput(IElement parent, string id, SetupSettings settings) : base(parent, id, settings)
        {
        }

        public override void Reset()
        {
            Value = 0;
        }

        protected override double ToValue(string raw)
        {
            try
            {
                return double.Parse(raw);
            }
            catch
            {
                return 0;
            }
        }

        protected override string FromValue(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override string GetInnerTypes()
        {
            return $"type=\"number\" step=\"{Step}\" min=\"{Min}\" max=\"{Max}\"";
        }
    }
}
