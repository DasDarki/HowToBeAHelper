using HowToBeAHelper.Scripting;

namespace HowToBeAHelper.Plugins
{
    internal class LuaPlugin : Plugin
    {
        internal ScriptingRuntime Runtime { get; }

        private readonly string _entryPoint;

        internal LuaPlugin(PluginMeta meta, string parentPath, string entryPoint)
        {
            Meta = meta;
            _entryPoint = entryPoint;
            Runtime = new ScriptingRuntime(this, parentPath);
        }

        public override void OnStart()
        {
            Runtime.RegisterObject("UI", UI);
            Runtime.LoadRelativeScript(_entryPoint);
            Runtime.CallFunction("OnStart");
        }

        public override void OnStop()
        {
            Runtime.CallFunction("OnStop");
        }
    }
}
