namespace HowToBeAHelper.Plugins
{
    internal class LuaInterpreter : IInterpreter
    {
        public Plugin Run(string path, PluginMeta meta, string entryPoint)
        {
            return new LuaPlugin(meta, path, entryPoint);
        }
    }
}
