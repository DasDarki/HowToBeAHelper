namespace HowToBeAHelper.UI.Controls
{
    /// <summary>
    /// The number input is a kind of input which only takes numbers.
    /// </summary>
    public interface INumberInput : IInput<double>
    {
        /// <summary>
        /// In which steps the number input is increasing and decreasing.
        /// </summary>
        double Step { get; set; }

        /// <summary>
        /// The minimum value of the input.
        /// </summary>
        double Min { get; set; }

        /// <summary>
        /// The maximum value of the input.
        /// </summary>
        double Max { get; set; }
    }
}
