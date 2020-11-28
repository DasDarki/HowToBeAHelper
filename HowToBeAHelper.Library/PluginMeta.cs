using System;
using System.Collections.Generic;

namespace HowToBeAHelper
{
    /// <summary>
    /// The meta defines needed information in order for the plugin manager to work.
    /// The meta gets loaded by a meta xml file. Both C# and LUA plugins need the meta file.
    /// </summary>
    public class PluginMeta
    {
        /// <summary>
        /// The id of the plugin. The id is required and identifies the plugin.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The display name of the plugin. If name is entered, its the name otherwise the id.
        /// </summary>
        public string Display => string.IsNullOrEmpty(Name) ? Id : Name;

        /// <summary>
        /// The type of the plugin. Its required and the loading process will fail, if its not entered.
        /// </summary>
        public PluginType Type { get; }

        /// <summary>
        /// The name is optional and is the display for the plugin. If the name is not set, the id will take over.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The author of the plugin. The author property is optional and has not any functionality but
        /// displaying who the plugin wrote.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// The description of the plugin what it is doing.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The version of the plugin. The version is required and will be used for differentiating the plugins
        /// as well as updating them.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// The dependencies are semi-required. If no dependencies are entered, the list is empty. The content
        /// of the dependencies are other plugin ids.
        /// The loader tries to load the dependencies first in order to prevent errors.
        /// </summary>
        public IReadOnlyList<string> Dependencies { get; }

        /// <summary>
        /// The default constructor for the plugin meta. It is being used by the main plugin handler. Don't call it yourself.
        /// </summary>
        public PluginMeta(PluginType type, string id, string name, string author, Version version, 
            IReadOnlyList<string> dependencies, string description)
        {
            Type = type;
            Id = id;
            Name = name;
            Author = author;
            Version = version;
            Dependencies = dependencies;
            Description = description;
        }
    }
}
