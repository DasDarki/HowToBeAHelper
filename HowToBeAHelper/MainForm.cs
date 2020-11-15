using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using HowToBeAHelper.UI;

namespace HowToBeAHelper.Client
{
    internal partial class MainForm : Form, IClient
    {
        internal ChromiumWebBrowser Browser { get; }

        public event Events.Keydown Keydown;

        public MainForm()
        {
            InitializeComponent();
            CefSettings settings = new CefSettings();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.WcfEnabled = true;
            Cef.Initialize(settings);
            Browser = new ChromiumWebBrowser($@"{Application.StartupPath}\html\index.html")
            {
                BrowserSettings = new BrowserSettings
                {
                    FileAccessFromFileUrls = CefState.Enabled,
                    UniversalAccessFromFileUrls = CefState.Enabled
                }
            };
            Browser.JavascriptObjectRepository.Register("bridge", new Bridge(this), false,
                BindingOptions.DefaultBinder);
            Controls.Add(Browser);
            Browser.Dock = DockStyle.Fill;
            Browser.MenuHandler = new HiddenMenuHandler();
            Browser.FrameLoadEnd += (sender, args) =>
            {
                if (args.Frame.IsMain)
                {
                    SafeInvoke(() =>
                    {
                        Visible = true;
                    });
                }
            };
            Keydown += OnKeydown;
        }

        private void OnKeydown(VirtualKeys key, bool isshiftdown, bool isaltdown, bool isctrldown, bool ismetadown)
        {
            if (isshiftdown && isctrldown && key == VirtualKeys.D)
            {
                Browser.ShowDevTools();
            }
        }

        internal void SafeInvoke(Action action)
        {
            Invoke(action);
        }

        internal void TriggerKeydown(VirtualKeys key, bool isShiftDown, bool isAltDown, bool isCtrlDown, bool isMetaDown)
        {
            Keydown?.Invoke(key, isShiftDown, isAltDown, isCtrlDown, isMetaDown);
        }

        internal class HiddenMenuHandler : IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters,
                IMenuModel model)
            {

            }

            public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters,
                CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                return true;
            }

            public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
            {
            }

            public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters,
                IMenuModel model, IRunContextMenuCallback callback)
            {
                return true;
            }
        }
    }
}
