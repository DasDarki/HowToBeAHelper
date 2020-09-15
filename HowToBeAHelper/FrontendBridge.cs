using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CefSharp;
using CefSharp.WinForms;
using HowToBeAHelper.Model.Characters;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming
namespace HowToBeAHelper
{
    internal class FrontendBridge
    {
        private readonly ChromiumWebBrowser _browser;
        private readonly MainForm _form;

        internal FrontendBridge(MainForm form)
        {
            _form = form;
            _browser = _form.Browser;
        }

        public void saveCharacter(string json, string username, IJavascriptCallback callback)
        {
            Character character = JsonConvert.DeserializeObject<Character>(json);
            if (string.IsNullOrEmpty(username))
            {
                Bootstrap.CharacterManager.Characters.Add(character);
                Bootstrap.CharacterManager.Save();
                callback.ExecuteAsync(true);
                return;
            }
            new Thread(async () =>
            {
                await _form.Master.SaveCharacter(username, character, status =>
                {
                    if (!status)
                    {
                        Bootstrap.CharacterManager.Characters.Add(character);
                        Bootstrap.CharacterManager.Save();
                    }

                    callback.ExecuteAsync(status);
                });
            }) { IsBackground = true }.Start();
        }

        public void saveLogin(string username, string password)
        {
            Bootstrap.Save(username, HashPassword(password));
            Bootstrap.StoredPassword = password;
            Bootstrap.StoredUsername = username;
            Bootstrap.IsAutomaticallyLoggedIn = true;
        }

        public void registerUser(string username, string password, IJavascriptCallback callback)
        {
            new Thread(async () =>
            {
                await _form.Master.RegisterUser(username, HashPassword(password), status =>
                {
                    callback.ExecuteAsync(status);
                });
            }) {IsBackground = true}.Start();
        }

        public void loginUser(bool needsHash, string username, string password, IJavascriptCallback callback)
        {
            new Thread(async () =>
            {
                await _form.Master.LoginUser(username, needsHash ? HashPassword(password) : password, chars =>
                {
                    callback.ExecuteAsync(chars);
                });
            }) {IsBackground = true}.Start();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var @byte in bytes)
                {
                    builder.Append(@byte.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
