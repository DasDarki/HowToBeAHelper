using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using HowToBeAHelper.BuiltIn;
using HowToBeAHelper.Model.Characters;
using Microsoft.Win32;
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

        public void updateSettingsBool(string key, bool val)
        {
            try
            {
                switch (key)
                {
                    case "autostart":
                        Bootstrap.Settings.AutoStart = val;
                        using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                        {
                            if (regKey == null) return;
                            if (val)
                            {
                                regKey.SetValue("How to be a Helper", Application.ExecutablePath);
                            }
                            else
                            {
                                regKey.DeleteValue("How to be a Helper");
                            }
                        }
                        break;
                    case "startmini":
                        Bootstrap.Settings.StartMinimize = val;
                        break;
                    case "minitray":
                        Bootstrap.Settings.MinimizeToTray = val;
                        break;
                }

                Bootstrap.Settings.Save();
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void refreshCharacters(string username, IJavascriptCallback callback)
        {
            try
            {
                _form.Run(async () =>
                {
                    await _form.Master.RefreshCharacters(username, json => callback.ExecuteAsync(json));
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void logout()
        {
            try
            {
                _form.Run(async () =>
                {
                    await _form.Master.Logout();
                });
                Bootstrap.IsAutomaticallyLoggedIn = false;
                Bootstrap.StoredPassword = null;
                Bootstrap.StoredUsername = null;
                Bootstrap.DeleteLogin();
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void syncSkills(string username, string charId, string json)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return;
                }

                _form.Run(async () =>
                {
                    await _form.Master.SyncSkills(username, charId, json);
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void syncCharDataLocally(string json)
        {
            try
            {
                Character character = JsonConvert.DeserializeObject<Character>(json);
                Bootstrap.CharacterManager.Characters.RemoveAll(o => o.ID == character.ID);
                Bootstrap.CharacterManager.Characters.Add(character);
                Bootstrap.CharacterManager.Save();

            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void syncCharData(string username, string charId, string key, string val)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return;
                }

                _form.Run(async () =>
                {
                    await _form.Master.SyncCharData(username, charId, key, val);
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void deleteCharacter(string username, bool isLocal, string charId, IJavascriptCallback callback)
        {
            try
            {
                if (isLocal)
                {
                    Bootstrap.CharacterManager.Characters.RemoveAll(o => o.ID == charId);
                    Bootstrap.CharacterManager.Save();
                    callback.ExecuteAsync(true);
                }
                else
                {
                    if (string.IsNullOrEmpty(username))
                    {
                        callback.ExecuteAsync(false);
                        return;
                    }

                    _form.Run(async () =>
                    {
                        await _form.Master.DeleteCharacter(username, charId, success =>
                        {
                            callback.ExecuteAsync(success);
                        });
                    });
                }
            }
            catch
            {
                callback.ExecuteAsync(false);
            }
        }

        public void exportCharacter(string json, IJavascriptCallback callback)
        {
            try
            {
                _form.SafeInvoke(() =>
                {
                    Character character = JsonConvert.DeserializeObject<Character>(json);
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.Filter = "PDF | *.pdf";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            CharacterGenerator.GeneratePdf(character, dialog.FileName);
                            callback.ExecuteAsync(true);
                        }
                    }
                });
            }
            catch
            {
                callback.ExecuteAsync(false);
            }
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
            try
            {
                Character character = JsonConvert.DeserializeObject<Character>(json);
                character.CreateYear = DateTime.Now.Year.ToString();
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
            catch
            {
                callback.ExecuteAsync(false, false);
            }
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
