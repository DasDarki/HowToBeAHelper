using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper.UI
{
    /// <summary>
    /// The modal is some kind of alert box which also can take input forms.
    /// </summary>
    public interface IModal : IParent
    {
        /// <summary>
        /// The title of the modal.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Opens the modal.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the modal.
        /// </summary>
        void Close();
    }
}
