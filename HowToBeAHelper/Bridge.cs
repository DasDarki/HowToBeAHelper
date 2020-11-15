namespace HowToBeAHelper.Client
{
    // ReSharper disable InconsistentNaming
    internal class Bridge
    {
        private readonly MainForm _form;

        internal Bridge(MainForm form)
        {
            _form = form;
        }

        public void triggerKeydown(int key, bool isShiftDown, bool isAltDown, bool isCtrlDown, bool isMetaDown)
        {
            _form.TriggerKeydown((VirtualKeys) key, isShiftDown, isAltDown, isCtrlDown, isMetaDown);
        }
    }
}
