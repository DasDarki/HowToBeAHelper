using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Layout;

namespace HowToBeAHelper.Plugins
{
    internal class PluginManager : IPluginManager
    {
        public IReadOnlyList<Plugin> Plugins => LoadedPlugins.AsReadOnly();

        internal List<Plugin> LoadedPlugins { get; }

        private readonly IInterpreter _csharpInterpreter = new CSharpInterpreter();
        private readonly IInterpreter _luaInterpreter = new LuaInterpreter();

        internal PluginManager()
        {
            LoadedPlugins = new List<Plugin>();
        }

        public Plugin LoadPlugin(string name)
        {
            string path = Bootstrap.PluginsPath + "\\" + name;
            PluginMeta meta = PluginMetaResolver.Resolve(path, out string entryPoint);
            return meta == null ? null : LoadInterpreter(path, meta, entryPoint);
        }

        public Plugin GetPlugin(string id)
        {
            return LoadedPlugins.SelectFirst(plugin =>
                plugin.Meta.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
        }

        public void StopPlugin(Plugin plugin, bool clean = false)
        {
            if (plugin.State == PluginState.Started)
            {
                try
                {
                    plugin.OnStop();
                    plugin.State = PluginState.Stopped;
                    if (clean)
                        LoadedPlugins.Remove(plugin);
                }
                catch (Exception e)
                {
                    Log.Append("The stop process for the plugin {0} throw an error! {1}", plugin.Meta.Id, e);
                    plugin.State = PluginState.Failed;
                }
            }
        }

        public bool StartPlugin(Plugin plugin)
        {
            try
            {
                plugin.OnStart();
                plugin.State = PluginState.Started;
                return true;
            }
            catch (Exception e)
            {
                Log.Append("The stop process for the plugin {0} throw an error! {1}", plugin.Meta.Id, e);
                plugin.State = PluginState.Failed;
                return false;
            }
        }

        internal void LoadPlugins()
        {
            List<Plugin> plugins = new List<Plugin>();
            foreach (string path in Directory.GetDirectories(Bootstrap.PluginsPath))
            {
                string name = path.Replace(Bootstrap.PluginsPath, "").Trim();
                Plugin plugin = LoadPlugin(name);
                if (plugin == null) continue;
                plugins.Add(plugin);
            }

            PrioritizedList list = new PrioritizedList(plugins);
            foreach (Plugin handle in plugins)
            {
                if (handle.Meta.Dependencies.Count > 0)
                {
                    foreach (string parent in handle.Meta.Dependencies)
                    {
                        list.Increment(handle, list.GetRise(parent));
                    }
                }
            }

            plugins = list.Convert();
            LoadedPlugins.AddRange(plugins);
            foreach (Plugin plugin in plugins)
            {
                StartPlugin(plugin);
            }
        }

        private Plugin LoadInterpreter(string path, PluginMeta meta, string entryPoint)
        {
            Plugin plugin = null;
            switch (meta.Type)
            {
                case PluginType.CSharp:
                    plugin = _csharpInterpreter.Run(path, meta, entryPoint);
                    break;
                case PluginType.LUA:
                    plugin = _luaInterpreter.Run(path, meta, entryPoint);
                    break;
            }

            if (plugin == null) return null;
            plugin.Meta = meta;
            plugin.PluginManager = this;
            plugin.UI = CefUI.UI;
            plugin.State = PluginState.Loaded;
            plugin.Page = new Parent(null, "_plugin_" + meta.Id + "_", SetupSettings.Default()){DenyDestroy = true};
            return plugin;
        }

        private class PrioritizedList
        {
            private List<PrioritizedItem> Items { get; }

            internal PrioritizedList(List<Plugin> input)
            {
                Items = new List<PrioritizedItem>();
                foreach (Plugin handle in input)
                {
                    Items.Add(new PrioritizedItem(handle));
                }
            }

            internal void Increment(Plugin handle, int value)
            {
                foreach (PrioritizedItem item in Items)
                {
                    if (item.Handle.Meta.Id == handle.Meta.Id)
                    {
                        item.Value += value;
                    }
                    else if (item.Handle.Meta.Dependencies.Contains(handle.Meta.Id))
                    {
                        Increment(item.Handle, value + 1);
                    }
                }
            }

            internal int GetRise(string parent)
            {
                foreach (PrioritizedItem item in Items)
                {
                    if (item.Handle.Meta.Id == parent)
                    {
                        return item.Value + 1;
                    }
                }
                return 1;
            }

            internal List<Plugin> Convert()
            {
                return Items.OrderBy(o => o.Value).Select(item => item.Handle).ToList();
            }
        }

        private class PrioritizedItem
        {
            internal int Value { get; set; }

            internal Plugin Handle { get; }
            
            internal PrioritizedItem(Plugin handle)
            {
                Value = 0;
                Handle = handle;
            }
        }
    }
}
