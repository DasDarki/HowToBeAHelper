using Newtonsoft.Json;

namespace HowToBeAHelper.Invite
{
    internal static class SessionJoinHandler
    {
        public static bool Handle(object invite)
        {
#if DEBUG
            return true;
#else
            if (!InviteHandler.CheckAndPush(invite))
            {
                InviteHandler.Start();
                if(invite != null)
                    Bootstrap.AutoJoinSession = JsonConvert.SerializeObject(invite);
                return true;
            }

            return false;
#endif
        }
    }
}
