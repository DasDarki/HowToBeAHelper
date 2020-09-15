﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using HowToBeAHelper.Net;
using HowToBeAHelper.Properties;

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
            Text = Settings.Default.Title;
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
                }
            };
            Browser.JavascriptObjectRepository.Register("bridge", Bridge = new FrontendBridge(this), false,
                BindingOptions.DefaultBinder);
            Controls.Add(Browser);
            Browser.Dock = DockStyle.Fill;
            Browser.FrameLoadEnd += (sender, args) =>
            {
                if (args.Frame.IsMain)
                {
                    Browser.ShowDevTools();
                    SafeInvoke(() =>
                    {
                        Visible = true;
                        new Thread(async () =>
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
                        }) {IsBackground = true}.Start();
                    });
                }
            };
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

        private void SafeInvoke(Action action)
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
    }
}
