using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using HowToBeAHelper.Model.Characters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOClient;

namespace HowToBeAHelper.Net
{
    /// <summary>
    /// The client for the connection and interaction with the master server.
    /// </summary>
    internal class MasterClient : INetwork
    {
        /// <summary>
        /// Whether a connection to the master is established or not.
        /// </summary>
        public bool IsConnected => Client.Connected;

        internal SocketIO Client { get; }

        private readonly Dictionary<string, Action<JArray>> _customRespones;
        private Action<int> _registerCallback;

        internal MasterClient()
        {
            _customRespones = new Dictionary<string, Action<JArray>>();
            Client = new SocketIO(Properties.Settings.Default.MasterUrl, new SocketIOOptions
            {
                Reconnection = false
            });
            Client.OnDisconnected += OnDisconnected;
            Client.On("user:register:result", response =>
            {
                _registerCallback(response.GetValue<int>());
                _registerCallback = null;
            });
            Client.On("character:sync-data", response =>
            {
                string charId = response.GetValue<string>();
                string key = response.GetValue<string>(1);
                string json = response.GetValue<string>(2);
                Bootstrap.System.SetCharData(charId, key, json);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applyCharDataSync(`{charId}`, `{key}`, `{json}`)");
            });
            Client.On("session:sync-mod-data", response =>
            {
                string sessionId = response.GetValue<string>();
                string key = response.GetValue<string>(1);
                string json = response.GetValue<string>(2);
                string charId = response.GetValue<string>(3);
                object val = JsonConvert.DeserializeObject<object>(json);
                Bootstrap.ModuleManager?.ApplySyncData(charId, key, val);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applySessionCharModDataSync(`{sessionId}`, `{key}`, `{json}`, `{charId}`)");
            });
            Client.On("session:sync-data", response =>
            {
                string sessionId = response.GetValue<string>();
                string key = response.GetValue<string>(1);
                string json = response.GetValue<string>(2);
                string charId = response.GetValue<string>(3);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applySessionCharDataSync(`{sessionId}`, `{key}`, `{json}`, `{charId}`)");
            });
            Client.On("character:sync-mod-data", response =>
            {
                string charId = response.GetValue<string>();
                string key = response.GetValue<string>(1);
                string json = response.GetValue<string>(2);
                object val = JsonConvert.DeserializeObject<object>(json);
                Bootstrap.System.SetCharModData(charId, key, val);
                Bootstrap.ModuleManager?.ApplySyncData(charId, key, val);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applyCharModDataSync(`{charId}`, `{key}`, `{json}`)");
            });
            Client.On("character:sync-skills", response =>
            {
                string charId = response.GetValue<string>();
                string json = response.GetValue<string>(1);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applyCharSkillsSync(`{charId}`, `{json}`)");
            });
            Client.On("session:sync-skills", response =>
            {
                string sessionId = response.GetValue<string>();
                string json = response.GetValue<string>(1);
                string charId = response.GetValue<string>(2);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applySessionCharSkillsSync(`{sessionId}`, `{json}`, `{charId}`)");
            });
            Client.On("character:sync-delete", response =>
            {
                string charId = response.GetValue<string>();
                Bootstrap.System.OnDeleteChar(charId);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applyCharDeletionSync(`{charId}`)");
            });
            Client.On("character:sync-creation", response =>
            {
                string json = response.GetValue<string>();
                MainForm.Instance.Browser.ExecuteScriptAsync($"applyCharCreationSync(`{json}`)");
            });
            Client.On("session:sync-join", response =>
            {
                string sessionId = response.GetValue<string>();
                string json = response.GetValue<string>(1);
                Bootstrap.System.OnCharCreate(json);
                MainForm.Instance.Browser.ExecuteScriptAsync($"applySessionJoinSync(`{sessionId}`, `{json}`)");
            });
            Client.On("session:kicked", response =>
            {
                string sessionId = response.GetValue<string>();
                MainForm.Instance.Browser.ExecuteScriptAsync($"applySessionKick(`{sessionId}`)");
            });
            Client.On("session:closed", response =>
            {
                string sessionId = response.GetValue<string>();
                MainForm.Instance.Browser.ExecuteScriptAsync($"applySessionKick(`{sessionId}`)");
            });
            Client.On("user:mute", response =>
            {
                bool flag = response.GetValue<bool>();
                //Requests.Get("http://localhost:9917/?flag=" + flag);
            });
            Client.On("plugins:custom-event", response =>
            {
                try
                {
                    string eventName = response.GetValue<string>();
                    JArray args = JArray.Parse(response.GetValue<string>(1));
                    if (_customRespones.ContainsKey(eventName))
                        _customRespones[eventName](args);
                }
                catch
                {
                    // Ignore; needs handling
                }
            });
            Client.On("session:dice-roll", response =>
            {
                try
                {
                    string username = response.GetValue<string>();
                    string resultData = response.GetValue<string>(1);
                    MainForm.Instance.Browser.ExecuteScriptAsync($"onRollHostCallback(`{username}`, `{resultData}`)");
                }
                catch
                {
                    // Ignore
                }
            });
        }

        private void OnDisconnected(object sender, string e)
        {
            MainForm.Instance.Browser.ExecuteScriptAsync("switchMode(false, false)");
            MainForm.Instance.NotifyError("Verbindung zum Master verloren!");
            MainForm.Instance.StartReconnecting();
        }

        /// <summary>
        /// Connects to the master server. If the connection could be established the <see cref="IsConnected"/> flag will be set to true
        /// and is returning true.
        /// </summary>
        internal async Task<bool> Connect()
        {
            try
            {
                await Client.ConnectAsync();
                MainForm.Instance.Browser.ExecuteScriptAsync("switchMode(true, " + (Bootstrap.IsAutomaticallyLoggedIn ? "true" : "false") + ")");
                if(Bootstrap.IsAutomaticallyLoggedIn)
                    MainForm.Instance.Browser.ExecuteScriptAsync($"autoLogin('{Bootstrap.StoredUsername}', '{Bootstrap.StoredPassword}')");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Request an user creation on the master server.
        /// </summary>
        /// <param name="name">The name for the user</param>
        /// <param name="email">The email for the user</param>
        /// <param name="password">The password for the user</param>
        /// <param name="callback">A callback which gets called, when the process is finished</param>
        internal async Task RegisterUser(string name, string email, string password, Action<int> callback)
        {
            _registerCallback = callback;
            await Client.EmitAsync("user:register", name, email, password);
        }

        internal async Task ForgotMyPassword(string email, Action<bool> callback)
        {
            await Client.EmitAsync("user:forgot-pw", response =>
            {
                callback(response.GetValue<bool>());
            }, email);
        }

        /// <summary>
        /// Requests a user login on the master server.
        /// </summary>
        /// <param name="name">The name for the user</param>
        /// <param name="password">The password for the user</param>
        /// <param name="callback">The callback gets called when acknowledgement was returned. The string are the user characters as JSON string</param>
        internal async Task LoginUser(string name, string password, Action<string> callback)
        {
            await Client.EmitAsync("user:login", response =>
            {
                callback(response.GetValue<string>());
            }, name, password);
        }

        internal async Task SaveEmail(string name, string email)
        {
            await Client.EmitAsync("user:set-email", name, email);
        }

        /// <summary>
        /// Starts a reconnecting thread which tries to reconnect.
        /// This reconnecting thread is only getting started when the first connection fails.
        /// </summary>
        internal void StartReconnecting(Action loadingCallback, Action connectedCallback)
        {
            new Thread(async () =>
            {
                while (!IsConnected)
                {
                    Thread.Sleep(30_000);
                    loadingCallback();
                    if (await Connect())
                    {
                        connectedCallback();
                        break;
                    }
                }
            }) {IsBackground = true}.Start();
        }

        /// <summary>
        /// Saves the character to the users database.
        /// </summary>
        /// <param name="username">The name of the given user</param>
        /// <param name="character">The character</param>
        /// <param name="action">Gets called after acknowledge</param>
        internal async Task SaveCharacter(string username, Character character, Action<bool> action)
        {
            await Client.EmitAsync("character:save", response =>
            {
                action(response.GetValue<bool>());
            }, username, character);
        }

        /// <summary>
        /// Deletes the character out of the users database.
        /// </summary>
        /// <param name="username">The name of the given user</param>
        /// <param name="charId">The character</param>
        /// <param name="action">Gets called after acknowledge</param>
        internal async Task DeleteCharacter(string username, string charId, Action<bool> action)
        {
            await Client.EmitAsync("character:delete", response =>
            {
                action(response.GetValue<bool>());
            }, username, charId);
        }

        /// <summary>
        /// Syncs the skills of the character with the master.
        /// </summary>
        /// <param name="username">The username of the wanted user</param>
        /// <param name="charId">The character to be synced</param>
        /// <param name="json">The json containing the skills update</param>
        internal async Task SyncSkills(string username, string charId, string json)
        {
            await Client.EmitAsync("character:skills", username, charId, json);
        }

        internal async Task SyncModulesData(string username, string charId, string key, string val)
        {
            await Client.EmitAsync("character:mod-update", username, charId, key, val);
        }

        internal async Task SyncSessionModulesData(string sessionId, string username, string charId, string key, string json)
        {
            await Client.EmitAsync("session:mod-update", sessionId, username, charId, key, json);
        }

        /// <summary>
        /// Syncs the general data of the character with the master.
        /// </summary>
        /// <param name="username">The username of the wanted user</param>
        /// <param name="charId">The character to be synced</param>
        /// <param name="key">The key of the general data</param>
        /// <param name="val">The new value of the data</param>
        internal async Task SyncCharData(string username, string charId, string key, string val)
        {
            await Client.EmitAsync("character:update", username, charId, key, val);
        }

        /// <summary>
        /// Triggers a logout on the master.
        /// </summary>
        internal async Task Logout()
        {
            await Client.EmitAsync("user:logout");
        }

        /// <summary>
        /// Requests a new set of characters for the user.
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="callback">The callback which gets called after acknowledgement</param>
        internal async Task RefreshCharacters(string username, Action<string> callback)
        {
            await Client.EmitAsync("user:char-refresh", response =>
            {
                callback(response.GetValue<string>());
            }, username);
        }

        /// <summary>
        /// Requests a session creation with the given arguments.
        /// </summary>
        /// <param name="name">The name of the session game</param>
        /// <param name="password">The password of the session - if given</param>
        /// <param name="callback">The callback which gets called after acknowledgement</param>
        internal async Task CreateSession(string name, string password, Action<string> callback)
        {
            await Client.EmitAsync("session:create", response =>
            {
                callback(response.GetValue<string>());
            }, name, password);
        }

        internal async Task SyncDiceRoll(string user, string sessionID, string result)
        {
            await Client.EmitAsync("session:dice-roll", user, sessionID, result);
        }

        /// <summary>
        /// Requests a session join with the given arguments.
        /// </summary>
        /// <param name="sessionId">The id of the session</param>
        /// <param name="sessionPassword">The password of the session - if given</param>
        /// <param name="charId">The character for the session</param>
        /// <param name="callback">The callback which gets called after acknowledgement</param>
        internal async Task JoinSession(string sessionId, string sessionPassword, string charId, Action<string> callback)
        {
            await Client.EmitAsync("session:join", response =>
            {
                callback(response.GetValue<string>());
            }, sessionId, sessionPassword, charId);
        }

        /// <summary>
        /// Requests a session data with the given arguments.
        /// </summary>
        /// <param name="sessionId">The id of the session</param>
        /// <param name="callback">The callback which gets called after acknowledgement</param>
        internal async Task RequestSession(string sessionId, Action<string> callback)
        {
            await Client.EmitAsync("session:request", response =>
            {
                callback(response.GetValue<string>());
            }, sessionId);
        }

        /// <summary>
        /// Syncs the char data with the session.
        /// </summary>
        /// <param name="sessionId">The id of the session</param>
        /// <param name="username">The name of the user</param>
        /// <param name="charId">The char id which gets synced</param>
        /// <param name="key">The key of the property</param>
        /// <param name="json">The value which gets synced</param>
        internal async Task SyncSessionCharData(string sessionId, string username, string charId, string key, string json)
        {
            await Client.EmitAsync("session:sync-data", sessionId, username, charId, key, json);
        }

        /// <summary>
        /// Syncs the skills data with the session.
        /// </summary>
        /// <param name="sessionId">The id of the session</param>
        /// <param name="charId">The char id which gets synced</param>
        /// <param name="json">The value which gets synced</param>
        internal async Task SyncSessionSkills(string sessionId, string charId, string json)
        {
            await Client.EmitAsync("session:sync-skills", sessionId, charId, json);
        }

        /// <summary>
        /// Kicks the player from the session.
        /// </summary>
        /// <param name="sessionId">The id of the session</param>
        /// <param name="playerId">The id of the player from the session</param>
        /// <param name="callback">The callback which gets called after acknowledgement</param>
        internal async Task KickPlayer(string sessionId, string playerId, Action<bool> callback)
        {
            await Client.EmitAsync("session:kick-player", response =>
            {
                callback(response.GetValue<bool>());
            }, sessionId, playerId);
        }

        /// <summary>
        /// Kicks the player from the session.
        /// </summary>
        /// <param name="sessionId">The id of the session</param>
        /// <param name="callback">The callback which gets called after acknowledgement</param>
        internal async Task CloseSession(string sessionId, Action<bool> callback)
        {
            await Client.EmitAsync("session:close", response =>
            {
                callback(response.GetValue<bool>());
            }, sessionId);
        }

        /// <summary>
        /// Toggles the mute flag of the player by the given username.
        /// </summary>
        internal async Task ToggleMute(string username, bool flag)
        {
            await Client.EmitAsync("user:mute", username, flag);
        }

        public IRemoteEvent Self(string eventName)
        {
            if (!IsConnected || !Bootstrap.System.IsLoggedIn)
                return null;
            return new RemoteEvent(eventName, new[] {Bootstrap.System.User});
        }

        public IRemoteEvent Unicast(string eventName, string username)
        {
            if (!IsConnected || !Bootstrap.System.IsLoggedIn)
                return null;
            return new RemoteEvent(eventName, new[] {username});
        }

        public IRemoteEvent Multicast(string eventName, params string[] usernames)
        {
            if (!IsConnected || !Bootstrap.System.IsLoggedIn)
                return null;
            return new RemoteEvent(eventName, usernames);
        }

        public void On(string eventName, Action<JArray> callback)
        {
            if (!IsConnected || !Bootstrap.System.IsLoggedIn)
                return;
            if(!_customRespones.ContainsKey(eventName))
                _customRespones.Add(eventName, callback);
        }
    }
}
