using System;
using System.Collections.Generic;
using HowToBeAHelper.Model;
using HowToBeAHelper.Model.Characters;
using HowToBeAHelper.Net;

namespace HowToBeAHelper
{
    /// <summary>
    /// The system is the main management component. It offers conntect to the direct underlying layer
    /// which cares about model management (e.g. characters or sessions) as well as communication
    /// possiblities to the outer layers like the master server or MongoDB.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// The name of the currently logged in user.
        /// </summary>
        string User { get; }

        /// <summary>
        /// Whether a user is logged into the system or not.
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Whether a session is selected or not.
        /// </summary>
        bool IsSessionSelected { get; }

        /// <summary>
        /// The current selected session.
        /// </summary>
        Session CurrentSession { get; }

        /// <summary>
        /// Whether the session is view is active.
        /// </summary>
        bool IsSessionView { get; }

        /// <summary>
        /// A list containing every character in the local storage of the system.
        /// </summary>
        List<Character> LocalCharacters { get; }

        /// <summary>
        /// A list containing every remotely saved character belonging to the currently logged in user.
        /// If not logged in, the list is empty.
        /// </summary>
        IReadOnlyList<Character> RemoteCharacters { get; }

        /// <summary>
        /// The instance of the API layer for the network access.
        /// </summary>
        INetwork Network { get; }

        /// <summary>
        /// Saves the local cached characters to the disk.
        /// </summary>
        void SaveLocalCharacters();

        /// <summary>
        /// Gets called when the user successfully logged in.
        /// </summary>
        event Action LoggedIn;
        /// <summary>
        /// Gets called when the user logged out.
        /// </summary>
        event Action LoggedOut;
        /// <summary>
        /// Gets called when a character has been updated.
        /// </summary>
        event Action<Character> CharacterUpdate;
        /// <summary>
        /// Gets called when a character has been created.
        /// </summary>
        event Action<Character> CharacterCreate;
        /// <summary>
        /// Gets called when a character has been deleted.
        /// </summary>
        event Action<Character> CharacterDelete;
        /// <summary>
        /// Gets called when a character is loaded into the character viewer.
        /// </summary>
        event Action<Character> CharacterLoad;
    }
}
