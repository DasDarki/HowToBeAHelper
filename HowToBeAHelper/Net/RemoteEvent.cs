using Newtonsoft.Json;

namespace HowToBeAHelper.Net
{
    internal class RemoteEvent : IRemoteEvent
    {
        private readonly string _eventName;
        private readonly string[] _usernames;
        private object[] _args;

        internal RemoteEvent(string eventName, string[] usernames)
        {
            _eventName = eventName;
            _usernames = usernames;
            _args = new object[0];
        }

        public IRemoteEvent Params(params object[] args)
        {
            _args = args;
            return this;
        }

        public void Send()
        {
            MainForm.Instance.Master.Client.EmitAsync("plugins:custom-event", _eventName,
                JsonConvert.SerializeObject(_usernames), JsonConvert.SerializeObject(_args));
        }
    }
}
