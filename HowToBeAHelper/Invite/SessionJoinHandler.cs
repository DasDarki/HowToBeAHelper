using Newtonsoft.Json;

namespace HowToBeAHelper.Invite
{
    internal static class SessionJoinHandler
    {
        public static bool Handle(object invite)
        {
            if (!InviteHandler.CheckAndPush(invite))
            {
                InviteHandler.Start();
                if(invite != null)
                    Bootstrap.AutoJoinSession = JsonConvert.SerializeObject(invite);
                return true;
            }

            return false;
        }
    }
}
