using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using HowToBeAHelper.Net;
using HowToBeAHelper.Properties;
using Newtonsoft.Json;

namespace HowToBeAHelper
{
    internal partial class MainForm : Form
    {
        internal static MainForm Instance { get; set; }

        internal ChromiumWebBrowser Browser { get; }

        internal FrontendBridge Bridge { get; }

        internal MasterClient Master { get; }

        internal MainForm()
        {
            Master = new MasterClient();
            InitializeComponent();
            Text = Properties.Settings.Default.Title;
            Icon = Resources.icon;
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
                },
                RequestHandler = new InterfaceRequestHandler()
            };
            Browser.ExecuteScriptAsyncWhenPageLoaded(
                $"emitLocalCharacters(`{JsonConvert.SerializeObject(Bootstrap.CharacterManager.Characters)}`)");
            Browser.ExecuteScriptAsyncWhenPageLoaded($"applyAutoSessionJoin(`{Bootstrap.AutoJoinSession}`)");
            Browser.ExecuteScriptAsyncWhenPageLoaded($"setSettings(`{JsonConvert.SerializeObject(Bootstrap.Settings)}`)");
            Browser.ExecuteScriptAsyncWhenPageLoaded($"applyChangelog(`{Updater.Changelog?.Version ?? "1.0"}`, `{Updater.Changelog?.Summary ?? ""}`, `{Updater.Changelog?.Content ?? ""}`, `{Updater.Changelog?.Author?.TrimEnd() ?? ""}`, `{Updater.Changelog?.Date?.TrimEnd() ?? ""}`)");
            Browser.JavascriptObjectRepository.Register("bridge", Bridge = new FrontendBridge(this), false,
                BindingOptions.DefaultBinder);
            Controls.Add(Browser);
            Browser.Dock = DockStyle.Fill;
            Browser.MenuHandler = new HiddenMenuHandler();
            Browser.FrameLoadEnd += (sender, args) =>
            {
                if (args.Frame.IsMain)
                {
                    //Browser.ShowDevTools();
                    SafeInvoke(() =>
                    {
                        Visible = true;
                        Run(async () =>
                        {
                            if (await Master.Connect())
                            {
                                NotifySuccess("Verbindung zum Master hergestellt!");
                            }
                            else
                            {
                                NotifyError("Verbindung zum Master fehlgeschlagen!");
                                StartReconnecting();
                            }
                        });
                    });
                }
            };
            if (Bootstrap.Settings.StartMinimize)
            {
                WindowState = FormWindowState.Minimized;
            }

            foreach (Plugin plugin in Bootstrap.PluginManager.LoadedPlugins)
            {
                Browser.ExecuteScriptAsyncWhenPageLoaded(
                    $"appendPluginEntry(`{plugin.Meta.Id}`, `{plugin.Meta.Display}`, `{plugin.State.GetName().ToLower()}`)");
            }
        }

        internal void Run(Action callback)
        {
            new Thread(() =>
            {
                callback();
            }){IsBackground = true}.Start();
        }

        internal void StartReconnecting()
        {
            Master.StartReconnecting(() => { }, () =>
            {
                NotifySuccess("Verbindung zum Master hergestellt!");
            });
        }

        /// <summary>
        /// Creates a timed notification in the view.
        /// </summary>
        /// <param name="text">The text of the notify</param>
        /// <param name="duration">The duration, how long it stays</param>
        public void NotifySuccess(string text, int duration = 3000)
        {
            Browser.ExecuteScriptAsync($"notifySuccess('{text}', {duration})");
        }

        /// <summary>
        /// Creates a timed notification in the view.
        /// </summary>
        /// <param name="text">The text of the notify</param>
        /// <param name="duration">The duration, how long it stays</param>
        public void NotifyError(string text, int duration = 5000)
        {
            Browser.ExecuteScriptAsync($"notifyError('{text}', {duration})");
        }

        public void AlertSuccess(string text, string title = "Juhu!")
        {
            Browser.ExecuteScriptAsync($"alertSuccess(`{text}`, `{title}`)");
        }

        public void AlertError(string text, string title = "Juhu!")
        {
            Browser.ExecuteScriptAsync($"alertError(`{text}`, `{title}`)");
        }

        internal void SafeInvoke(Action action)
        {
            Invoke(action);
        }

        #region Seals

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && Bootstrap.Settings.MinimizeToTray)
            {
                Hide();
                trayIcon.Visible = true;
            }
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }

        internal class InterfaceRequestHandler : IRequestHandler
        {
            public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture,
                bool isRedirect)
            {
                return false;
            }

            public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl,
                WindowOpenDisposition targetDisposition, bool userGesture)
            {
                return targetUrl == "https://github.com/DasDarki/HowToBeAHelper" 
                        || targetUrl == "https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode.de";
            }

            public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
                IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
            {
                return null;
            }

            public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host,
                int port, string realm, string scheme, IAuthCallback callback)
            {
                callback.Dispose();
                return false;
            }

            public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize,
                IRequestCallback callback)
            {
                callback.Dispose();
                return false;
            }

            public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl,
                ISslInfo sslInfo, IRequestCallback callback)
            {
                callback.Dispose();
                return false;
            }

            public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port,
                X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
            {
                callback.Dispose();
                return false;
            }

            public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
            {
            }

            public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
            {
            }
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
