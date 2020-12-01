namespace HowToBeAHelper.UI.Layout
{
    /// <summary>
    /// The row is a layout element to align columns and other elements.
    /// </summary>
    public interface IRow : IParent
    {
        /// <summary>
        /// Whether the columns in this row have no gaps.
        /// </summary>
        bool IsGapless { get; set; }

        /// <summary>
        /// Whether the columns are centered in this row.
        /// </summary>
        bool IsCentered { get; set; }

        /// <summary>
        /// Whether the row takes the fullwidth or not.
        /// </summary>
        bool IsFullwidth { get; set; }
    }
}
