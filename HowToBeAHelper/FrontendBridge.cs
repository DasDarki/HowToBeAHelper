using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using HowToBeAHelper.BuiltIn;
using HowToBeAHelper.Model.Characters;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using Button = HowToBeAHelper.UI.Controls.Button;

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

        public void ui_OnCheckboxChange(string id, bool val)
        {
            IElement element = CefUI.CreatedElements.SelectFirst(o => o.ID == id);
            if (element != null && element is ICheckbox checkbox)
            {
                ((Checkbox) checkbox).TriggerChange(val);
            }
        }

        public void ui_OnChange(string id, string val)
        {
            IElement element = CefUI.CreatedElements.SelectFirst(o => o.ID == id);
            if (element != null)
            {
                switch (element)
                {
                    case ITextInput textInput:
                        ((TextInput) textInput).TriggerChange(val);
                        break;
                    case INumberInput numberInput:
                        ((NumberInput) numberInput).TriggerChange(val);
                        break;
                    case ISelect select:
                        ((Select) select).TriggerChange(val);
                        break;
                }
            }
        }

        public void ui_OnFocusOut(string id)
        {
            IElement element = CefUI.CreatedElements.SelectFirst(o => o.ID == id);
            if (element != null)
            {
                switch (element)
                {
                    case ITextInput textInput:
                        ((TextInput) textInput).TriggerFocusOut();
                        break;
                    case INumberInput numberInput:
                        ((NumberInput) numberInput).TriggerFocusOut();
                        break;
                }
            }
        }

        public void ui_TriggerConfirm(string id, bool val)
        {
            CefUI.UI.TriggerConfirm(id, val);
        }

        public void ui_FooterClick(string id, string clickId)
        {
            IElement element = CefUI.CreatedElements.SelectFirst(o => o.ID == id);
            if (element != null && element is ICard card)
            {
                ((Card) card).TriggerFooterClick(clickId);
            }
        }

        public void ui_OnClick(string id)
        {
            IElement element = CefUI.CreatedElements.SelectFirst(o => o.ID == id);
            if (element != null)
            {
                if (element is IButton btn)
                {
                    ((Button) btn).TriggerClick();
                }
            }
        }

        public void ui_OnDoubleClick(string id)
        {
            IElement element = CefUI.CreatedElements.SelectFirst(o => o.ID == id);
            if (element != null)
            {
                if (element is IButton btn)
                {
                    ((Button)btn).TriggerDoubleClick();
                }
            }
        }

        public void saveSessionBattle(string session, string data)
        {
            LocalStorage.Write(session, data, "battle");
        }

        public void getSessionBattlers(string session, IJavascriptCallback callback)
        {
            callback.ExecuteAsync(LocalStorage.Read(session, "battle"));
        }

        public void sendMuteToggle(string username, bool flag)
        {
            try
            {
                _form.Run(async () =>
                {
                    await _form.Master.ToggleMute(username, flag);
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void closeSession(string sessionId, IJavascriptCallback callback)
        {
            try
            {
                LocalStorage.Delete(sessionId, "battle");
                _form.Run(async () =>
                {
                    await _form.Master.CloseSession(sessionId, b =>
                    {
                        callback.ExecuteAsync(b);
                    });
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void kickPlayer(string sessionId, string userId, IJavascriptCallback callback)
        {
            try
            {
                _form.Run(async () =>
                {
                    await _form.Master.KickPlayer(sessionId, userId, b =>
                    {
                        callback.ExecuteAsync(b);
                    });
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void syncSessionSkills(string sessionId, string charId, string json)
        {
            try
            {
                if (string.IsNullOrEmpty(charId))
                {
                    return;
                }

                _form.Run(async () =>
                {
                    await _form.Master.SyncSessionSkills(sessionId, charId, json);
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void syncSessionCharData(string sessionId, string username, string charId, string key, string json)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return;
                }

                _form.Run(async () =>
                {
                    await _form.Master.SyncSessionCharData(sessionId, username, charId, key, json);
                });
            }
            catch
            {
                //Ignore: Need handling
            }
        }

        public void joinSession(string sessionId, string sessionPassword, string charId, IJavascriptCallback callback)
        {
            try
            {
                _form.Run(async () =>
                {
                    await _form.Master.JoinSession(sessionId, sessionPassword, charId, newSession =>
                    {
                        callback.ExecuteAsync(newSession);
                    });
                });
            }
            catch
            {
                callback.ExecuteAsync("");
            }
        }

        public void createSession(string name, string password, IJavascriptCallback callback)
        {
            try
            {
                _form.Run(async () =>
                {
                    await _form.Master.CreateSession(name, password, newSession =>
                    {
                        callback.ExecuteAsync(newSession);
                    });
                });
                
            }
            catch
            {
                callback.ExecuteAsync("");
            }
        }

        public void showDevtools()
        {
            _browser.ShowDevTools();
        }

        public void updateSettingsBool(string key, bool val)
        {
            try
            {
                switch (key)
                {
                    case "autostart":
                        if (!Program.IsElevated()) return;
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

        public void registerUser(string username, string email, string password, IJavascriptCallback callback)
        {
            _form.Run(async () =>
            {
                await _form.Master.RegisterUser(username, email, HashPassword(password), status =>
                {
                    callback.ExecuteAsync(status);
                });
            });
        }

        public void forgotMyPassword(string username, IJavascriptCallback callback)
        {
            _form.Run(async () =>
            {
                await _form.Master.ForgotMyPassword(username, status =>
                {
                    callback.ExecuteAsync(status);
                });
            });
        }

        public void saveEmailInput(string username, string email)
        {
            _form.Run(async () =>
            {
                await _form.Master.SaveEmail(username, email);
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
