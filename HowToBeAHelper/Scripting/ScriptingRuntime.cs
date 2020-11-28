using System;
using System.IO;
using MoonSharp.Interpreter;

namespace HowToBeAHelper.Scripting
{
    public class ScriptingRuntime
    {
        private readonly string _parentPath;
        private readonly Script _script;
        private readonly Plugin _plugin;

        public ScriptingRuntime(Plugin plugin, string parentPath)
        {
            _plugin = plugin;
            _parentPath = parentPath;
            _script = new Script();
            RegisterFunction("log", (Action<string>) Log);
            RegisterFunction("require", (Func<string, DynValue>) LoadRelativeScript);
        }

        public DynValue LoadRelativeScript(string relativeName)
        {
            string path = Path.Combine(_parentPath, relativeName.Replace("/", "\\"));
            string code = File.ReadAllText(path);
            return _script.DoString(code);
        }

        public DynValue CallFunction(string name, params object[] args)
        {
            DynValue func = _script.Globals.Get(name);
            if (func == null) return null;
            return _script.Call(func, args);
        }

        public void RegisterFunction(string name, object @delegate)
        {
            _script.Globals[name] = @delegate;
        }

        public void RegisterObject(string name, object obj)
        {
            Type type = obj.GetType();
            if(!UserData.IsTypeRegistered(type))
                UserData.RegisterType(type);
            _script.Globals[name] = obj;
        }

        private void Log(string message)
        {
            _plugin.Log(message);
        }
    }
}
