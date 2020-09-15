using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
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

        public void transferCharacter(string lastId, string json, string username, IJavascriptCallback callback)
        {
            Character character = JsonConvert.DeserializeObject<Character>(json);
            if (string.IsNullOrEmpty(username) || !_form.Master.IsConnected)
            {
                callback.ExecuteAsync(false);
                return;
            }
            _form.Run(async () =>
            {
                await _form.Master.SaveCharacter(username, character, status =>
                {
                    if (status)
                    {
                        Bootstrap.CharacterManager.Characters.RemoveAll(o => o.ID == lastId);
                        Bootstrap.CharacterManager.Save();
                    }

                    callback.ExecuteAsync(status);
                });
            });
        }

        public void saveCharacter(string json, string username, IJavascriptCallback callback)
        {
            Character character = JsonConvert.DeserializeObject<Character>(json);
            if (string.IsNullOrEmpty(username) || !_form.Master.IsConnected)
            {
                Bootstrap.CharacterManager.Characters.Add(character);
                Bootstrap.CharacterManager.Save();
                callback.ExecuteAsync(true, true);
                return;
            }
            _form.Run(async () =>
            {
                await _form.Master.SaveCharacter(username, character, status =>
                {
                    bool local = false;
                    if (!status)
                    {
                        Bootstrap.CharacterManager.Characters.Add(character);
                        Bootstrap.CharacterManager.Save();
                        local = true;
                    }

                    callback.ExecuteAsync(status, local);
                });
            });
        }

        public void openExternalUrl(string url)
        {
            Process.Start(url);
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
            _form.Run(async () =>
            {
                await _form.Master.RegisterUser(username, HashPassword(password), status =>
                {
                    callback.ExecuteAsync(status);
                });
            });
        }

        public void loginUser(bool needsHash, string username, string password, IJavascriptCallback callback)
        {
            _form.Run(async () =>
            {
                await _form.Master.LoginUser(username, needsHash ? HashPassword(password) : password, chars =>
                {
                    callback.ExecuteAsync(chars);
                });
            });
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
