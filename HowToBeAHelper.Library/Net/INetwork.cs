using System;
using Newtonsoft.Json.Linq;

namespace HowToBeAHelper.Net
{
    /// <summary>
    /// The network layer allows interaction with the master network behind the HTBAH system.
    /// The network interaction of this API only works, if the user is logged in.
    /// </summary>
    public interface INetwork
    {
        /// <summary>
        /// Prepares a remote event to all connected devices of the currently logged in user in the network.
        /// </summary>
        /// <param name="eventName">The event name of the trigger</param>
        /// <returns>The prepared event or null if not connected or logged in</returns>
        IRemoteEvent Self(string eventName);

        /// <summary>
        /// Prepares a remote event to all connected devices of a specific user in the network.
        /// </summary>
        /// <param name="eventName">The event name of the trigger</param>
        /// <param name="username">The name of the wanted user</param>
        /// <returns>The prepared event or null if not connected or logged in</returns>
        IRemoteEvent Unicast(string eventName, string username);

        /// <summary>
        /// Prepares a remote event to all connected devices of all specified users in the network.
        /// </summary>
        /// <param name="eventName">The event name of the trigger</param>
        /// <param name="usernames">All names of the wanted users</param>
        /// <returns>The prepared event or null if not connected or logged in</returns>
        IRemoteEvent Multicast(string eventName, params string[] usernames);

        /// <summary>
        /// Adds an event handler which listens to the specified event in the network.
        /// </summary>
        /// <param name="eventName">The event to listen to</param>
        /// <param name="callback">The callback which gets called when the event is received</param>
        void On(string eventName, Action<JArray> callback);
    }
}
