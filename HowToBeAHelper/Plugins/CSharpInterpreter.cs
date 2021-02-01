using System;
using System.IO;
using System.Reflection;

namespace HowToBeAHelper.Plugins
{
    internal class CSharpInterpreter : IInterpreter
    {
        private readonly Type _pluginType = typeof(Plugin);

        public Plugin Run(string path, PluginMeta meta, string entryPoint)
        {
            string mainPath = Path.Combine(path, entryPoint);
            if (!File.Exists(mainPath)) return null;
            foreach (string child in Directory.GetFiles(path))
            {
                if (child != mainPath && Path.GetExtension(child).ToLower().Contains("dll"))
                {
                    Assembly.LoadFile(child);
                }
            }

            Assembly assembly = Assembly.Load(File.ReadAllBytes(mainPath));
            Type mainType = null;
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (_pluginType.IsAssignableFrom(type))
                {
                    mainType = type;
                    break;
                }
            }

            if(mainType == null)
                throw new Exception("No plugin class was found but its needed!");
            Plugin plugin = (Plugin) Activator.CreateInstance(mainType);
            plugin.Meta = meta;
            return plugin;
        }
    }
}
