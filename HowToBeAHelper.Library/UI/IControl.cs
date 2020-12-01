namespace HowToBeAHelper.UI
{
    /// <summary>
    /// The control is a specific element which controls the UI.
    /// </summary>
    public interface IControl : IElement
    {
        /// <summary>
        /// The label of the control. If null, the control has no label.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Resets the control to its default state.
        /// </summary>
        void Reset();
    }
}
