using System;
using System.Collections.Generic;
using CefSharp;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Layout;
using MoonSharp.Interpreter;

namespace HowToBeAHelper.Scripting
{
    [MoonSharpUserData]
    internal class ScriptUI : IUI
    {
        private readonly Dictionary<string, Action<bool>> _confirmCallbacks = new Dictionary<string, Action<bool>>();
        private readonly Dictionary<string, IModal> _modals = new Dictionary<string, IModal>();

        public void NotifySuccess(string message, int duration = 3000)
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.NotifySuccess(message, duration);
            });
        }

        public void NotifyError(string message, int duration = 3000)
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.NotifyError(message, duration);
            });
        }

        public void AlertSuccess(string text, string title = "Juhu!")
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.AlertSuccess(text, title);
            });
        }

        public void AlertError(string text, string title = "Juhu!")
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.AlertError(text, title);
            });
        }

        public void AlertConfirm(string text, Action<bool> callback, string title = "Sicher?", string yes = "JA!",
            string no = "NEEEE!!!")
        {
            string id = Guid.NewGuid().ToString();
            while (_confirmCallbacks.ContainsKey(id))
            {
                id = Guid.NewGuid().ToString();
            }

            _confirmCallbacks.Add(id, callback);
            MainForm.Instance.Browser.ExecuteScriptAsync($"alertConfirm(`{id}`, `{text}`, `{title}`, `{yes}`, `{no}`)");
        }

        public void ShowLoader()
        {
            CefUI.SetElementDisplay("loader", true);
        }

        public void HideLoader()
        {
            CefUI.SetElementDisplay("loader", false);
        }

        public IModal RegisterModal(string title = "")
        {
            string id = Guid.NewGuid().ToString();
            while (_modals.ContainsKey(id))
            {
                id = Guid.NewGuid().ToString();
            }

            Modal modal = new Modal(null, id, SetupSettings.Default());
            _modals.Add(id, modal);
            MainForm.Instance.Browser.ExecuteScriptAsyncWhenPageLoaded(
                $"ui_CreateElement('modal-plugins', `{modal.GetHTML("")}`)");
            return modal;
        }

        internal void TriggerConfirm(string id, bool val)
        {
            if(!_confirmCallbacks.ContainsKey(id)) return;
            _confirmCallbacks[id](val);
            _confirmCallbacks.Remove(id);
        }

        public event Action<IParent> ContainerLoad;
    }
}
