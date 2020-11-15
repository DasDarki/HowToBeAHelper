namespace HowToBeAHelper
{
    /// <summary>
    /// This class contains delegates for the event handling of the UI framework behind HTBAH.
    /// </summary>
    public class Events
    {
        /// <summary>
        /// Gets called when a key is getting pressed down.
        /// </summary>
        /// <param name="key">The key which is down</param>
        /// <param name="isShiftDown">True, if the shift key is down</param>
        /// <param name="isAltDown">True, if the alt key is down</param>
        /// <param name="isCtrlDown">True, if the control key is down</param>
        /// <param name="isMetaDown">True, if the meta / logo key is down</param>
        public delegate void Keydown(VirtualKeys key, bool isShiftDown, bool isAltDown, bool isCtrlDown, bool isMetaDown);
    }
}
