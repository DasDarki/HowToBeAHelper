using HowToBeAHelper;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Controls;

namespace TestPlugin
{
    public class PluginMain : Plugin
    {
        public override void OnStart()
        {
            
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
