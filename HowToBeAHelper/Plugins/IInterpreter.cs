namespace HowToBeAHelper.Plugins
{
    internal interface IInterpreter
    {
        Plugin Run(string path, PluginMeta meta, string entryPoint);
    }
}
