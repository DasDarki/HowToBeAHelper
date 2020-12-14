using HowToBeAHelper;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Controls;
using HowToBeAHelper.UI.Layout;

namespace TestPlugin
{
    public class PluginMain : Plugin
    {
        public override void OnStart()
        {
            UI.ContainerLoad += OnContainerLoad;
        }

        private void OnContainerLoad(ContainerType type, IParent page)
        {
            if (type == ContainerType.CharEditor)
            {
                IButton button = page.Create<IButton>("test-editor", SetupSettings.Default().SetText("Test Edit"));
                button.Click += () =>
                {
                    UI.AlertSuccess("HALLOOOOO!!!");
                };
            }
        }

        public override void OnPageLoad()
        {
            IButton button = Page.Create<IButton>("test", SetupSettings.Default().SetText("Test"));
            button.Click += ButtonOnClick;
        }

        private void ButtonOnClick()
        {
            UI.NotifySuccess("ES FUNKT!");
        }
    }
}
