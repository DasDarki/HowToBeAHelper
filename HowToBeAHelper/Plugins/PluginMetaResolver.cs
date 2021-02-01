using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace HowToBeAHelper.Plugins
{
    internal static class PluginMetaResolver
    {
        public static PluginMeta Resolve(string path, out string entryPoint)
        {
            entryPoint = null;
            try
            {
                string file = Path.Combine(path, "meta.xml");
                string xml = File.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                if(doc.DocumentElement == null)
                    throw new NullReferenceException("The document element of the meta is null.");
                XmlNode pluginElement = doc.DocumentElement.CloneNode(false);
                PluginType type = GetPluginType(GetNodeAttribute(pluginElement, "type"));
                string id = GetNodeAttribute(pluginElement, "id");
                string name = SearchNodeValue(doc.DocumentElement, "name", true);
                Version version = Version.Parse(SearchNodeValue(doc.DocumentElement, "version"));
                string author = SearchNodeValue(doc.DocumentElement, "author", true);
                string description = SearchNodeValue(doc.DocumentElement, "description", true);
                List<string> dependencies = new List<string>();
                XmlNodeList dependencyNodes = doc.DocumentElement.SelectNodes("plugin/dependencies/plugin");
                if (dependencyNodes != null)
                {
                    for (int i = 0; i < dependencyNodes.Count; i++)
                    {
                        XmlNode node = dependencyNodes[i];
                        if (node != null)
                        {
                            string value = node.InnerText;
                            if (!string.IsNullOrEmpty(value))
                                dependencies.Add(value);
                        }
                    }
                }

                entryPoint = SearchNodeValue(doc.DocumentElement, "entryPoint");
                return new PluginMeta(type, id, name, author, version, dependencies.AsReadOnly(), description);
            }
            catch(Exception e)
            {
                Log.Append("Tried to resolve a plugin meta on path {0} without success because of an error! {1}", path, e);
                return null;
            }
        }

        private static string SearchNodeValue(XmlElement doc, string nodeName, bool optional = false)
        {
            XmlNode node = doc.SelectSingleNode(nodeName);
            if (node == null)
            {
                if (!optional)
                    throw new NullReferenceException("The required node " + nodeName + " is missing!");
                return "";
            }

            return node.InnerText;
        }

        private static string GetNodeAttribute(XmlNode node, string name, bool optional = false)
        {
            string value = node.Attributes?[name].Value;
            if(value == null && !optional)
                throw new NullReferenceException("Required attribute " + name + " is invalid.");
            return value;
        }

        private static PluginType GetPluginType(string type)
        {
            switch (type.ToLower())
            {
                case "csharp":
                    return PluginType.CSharp;
                case "lua":
                    return PluginType.LUA;
                default:
                    throw new ArgumentException("The given plugin type is invalid!");
            }
        }
    }
}
