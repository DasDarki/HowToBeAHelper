using System.Collections.Generic;
using System.IO;
using CefSharp;
using HowToBeAHelper.Model.Characters;
using HowToBeAHelper.Modules;
using HowToBeAHelper.UI;
using HowToBeAHelper.UI.Controls;
using HowToBeAHelper.UI.Layout;
using Newtonsoft.Json;

namespace HowToBeAHelper
{
    internal class ModuleManager
    {
        public List<Module> Modules { get; }

        private IParent _charEditor, _charViewer;
        private readonly List<IElement> _createdElements;
        private readonly Dictionary<string, INumberInput> _numberInputs;
        private readonly Dictionary<string, ITextInput> _textInputs;
        private readonly Dictionary<string, ISelect> _selects;
        private Character _currentCharacter;

        internal ModuleManager()
        {
            _createdElements = new List<IElement>();
            _numberInputs = new Dictionary<string, INumberInput>();
            _textInputs = new Dictionary<string, ITextInput>();
            _selects = new Dictionary<string, ISelect>();
            Modules = new List<Module>();
        }

        internal void Hook()
        {
            CefUI.UI.ContainerLoad += OnContainerLoad;
            Bootstrap.System.CharacterLoad += OnCharacterLoad;
        }

        private void OnCharacterLoad(Character character)
        {
            _currentCharacter = character;
            if (character.ModulesData == null) return;
            foreach (string key in character.ModulesData.Keys)
            {
                object val = character.ModulesData[key];
                FillDataInto(key, val);
            }
        }

        private void FillDataInto(string key, object val)
        {
            if (val is string s)
            {
                if (_textInputs.ContainsKey(key))
                    _textInputs[key].Value = s;
                else if (_selects.ContainsKey(key) && int.TryParse(s, out int i))
                    _selects[key].CurrentIndex = i;
            }
            else if (val is double d)
            {
                if (_numberInputs.ContainsKey(key))
                    _numberInputs[key].Value = d;
            }
            else if (val is long l)
            {
                if (_numberInputs.ContainsKey(key))
                    _numberInputs[key].Value = l;
            }
            else if (val is float f)
            {
                if (_numberInputs.ContainsKey(key))
                    _numberInputs[key].Value = f;
            }
            else if (val is int i)
            {
                if (_numberInputs.ContainsKey(key))
                    _numberInputs[key].Value = i;
            }
        }

        private void OnContainerLoad(ContainerType type, IParent parent)
        {
            switch (type)
            {
                case ContainerType.CharEditor:
                    _charEditor = parent;
                    AppendToParent(parent, "editor");
                    break;
                case ContainerType.CharViewer:
                    _charViewer = parent;
                    AppendToParent(parent, "viewer");
                    break;
            }
        }

        internal void Reload()
        {
            Reset();
            Modules.Clear();
            List<object> modulesForJs = new List<object>();
            foreach (string file in Directory.GetFiles(Bootstrap.ModulesPath))
            {
                if (Path.GetExtension(file).ToLower().EndsWith("htbam"))
                {
                    Module module = TryExtract(file);
                    if (module == null) continue;
                    Modules.Add(module);
                    modulesForJs.Add(new {meta = module.Meta, ruleset = module.Ruleset});
                }
            }

            MainForm.Instance.Browser.ExecuteScriptAsync(
                $"updateModules(`{FrontendBridge.EncodeBase64(JsonConvert.SerializeObject(modulesForJs))}`)");
            ExecuteCustomAbilities("add");
            if (_charEditor != null)
            {
                AppendToParent(_charEditor, "editor");
            }

            if (_charViewer != null)
            {
                AppendToParent(_charViewer, "viewer");
            }
        }

        private Module TryExtract(string file)
        {
            try
            {
                ModulePack pack = ModulePack.Deserialize(File.ReadAllBytes(file));
                Module module = Module.TryLoad(pack.Content);
                module.Ruleset = pack.Ruleset;
                return module;
            }
            catch
            {
                return null;
            }
        }

        private void AppendToParent(IParent parent, string extra)
        {
            foreach (Module module in Modules)
            {
                if (module.Form == null) continue;
                foreach (ModuleForm.Row formRow in module.Form.Rows)
                {
                    IRow row = parent.Create<IRow>(null, SetupSettings.Default("is-fullwidth", "is-centered"));
                    _createdElements.Add(row);
                    foreach (ModuleForm.Column formColumn in formRow.Columns)
                    {
                        IColumn column = row.Create<IColumn>();
                        foreach (ModuleForm.Select formSelect in formColumn.Selects)
                        {
                            if (string.IsNullOrEmpty(formSelect.Key)) continue;
                            string key = $"{module.Meta.Name}:{formSelect.Key}";
                            if (_selects.ContainsKey(key)) continue;
                            ISelect select = column.Create<ISelect>(null,
                                SetupSettings.Default(extra + "-modinput")
                                    .SetLabel(formSelect.Label).SetItems(formSelect.Options)
                                    .SetData(
                                        $"data-type=\"select\" data-key=\"{key}\""));
                            select.IsFullwidth = true;
                            if (extra == "viewer")
                            {
                                _selects.Add(key, select);
                                select.Change += s =>
                                {
                                    MainForm.Instance.Run(() =>
                                    {
                                        SyncData(key, s);
                                    });
                                };
                            }
                        }

                        foreach (ModuleForm.Input formInput in formColumn.Inputs)
                        {
                            if (string.IsNullOrEmpty(formInput.Key)) continue;
                            string type = formInput.Type.ToLower();
                            string key = $"{module.Meta.Name}:{formInput.Key}";
                            switch (type)
                            {
                                case "text":
                                    if (_textInputs.ContainsKey(key)) continue;
                                    ITextInput textInput = column.Create<ITextInput>(null,
                                        SetupSettings.Default(extra + "-modinput")
                                            .SetLabel(formInput.Label).SetText(formInput.Placeholder)
                                            .SetData(
                                                $"data-type=\"text\" data-key=\"{key}\""));
                                    if (extra == "viewer")
                                    {
                                        _textInputs.Add(key, textInput);
                                        var input = textInput;
                                        textInput.FocusOut += () =>
                                        {
                                            MainForm.Instance.Run(() =>
                                            {
                                                SyncData(key, input.Value);
                                            });
                                        };
                                    }

                                    break;
                                case "number":
                                    if (_numberInputs.ContainsKey(key)) continue;
                                    string dataDbl = "";
                                    string dblAction = "";
                                    if (formInput.DblClickAction == "dice_roll" && extra == "viewer")
                                    {
                                        dataDbl = "data-dbl=\"dice\"";
                                        dblAction = $"ondblclick=\"onDiceValue(this, \\`{formInput.Label}\\`)\"";
                                    }

                                    INumberInput numberInput = column.Create<INumberInput>(null, SetupSettings.Default(extra + "-modinput")
                                        .SetLabel(formInput.Label).SetText(formInput.Placeholder)
                                        .SetData(
                                            $"data-type=\"number\" data-key=\"{key}\" {dataDbl} {dblAction}"));
                                    if (extra == "viewer")
                                    {
                                        _numberInputs.Add(key, numberInput);
                                        var input = numberInput;
                                        numberInput.FocusOut += () =>
                                        {
                                            MainForm.Instance.Run(() =>
                                            {
                                                SyncData(key, input.Value);
                                            });
                                        };
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void ApplySyncData(string charId, string key, object val)
        {
            if (_currentCharacter != null && _currentCharacter.ID == charId)
            {
                if (_currentCharacter.ModulesData == null)
                    _currentCharacter.ModulesData = new Dictionary<string, object>();
                if (_currentCharacter.ModulesData.ContainsKey(key))
                    _currentCharacter.ModulesData.Remove(key);
                FillDataInto(key, val);
            }
        }

        private void SyncData(string key, object val)
        {
            if (_currentCharacter.ModulesData == null)
                _currentCharacter.ModulesData = new Dictionary<string, object>();
            if (_currentCharacter.ModulesData.ContainsKey(key))
                _currentCharacter.ModulesData.Remove(key);
            _currentCharacter.ModulesData.Add(key, val);
            if (Bootstrap.System.IsLocalCharacter)
            {
                Bootstrap.System.SaveLocalCharacters();
            }
            else
            {
                if (Bootstrap.System.IsSessionView)
                {
                    string json = JsonConvert.SerializeObject(val);
                    MainForm.Instance.Master.SyncSessionModulesData(Bootstrap.System.CurrentSession.ID,
                            Bootstrap.System.User, _currentCharacter.ID, key, json)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    string json = JsonConvert.SerializeObject(val);
                    MainForm.Instance.Master.SyncModulesData(Bootstrap.System.User, _currentCharacter.ID, key, json)
                        .GetAwaiter().GetResult();
                }
            }
        }

        private void AddNonExistingToList(List<string> target, List<string> list)
        {
            foreach (string item in list)
            {
                if(!target.Contains(item.Trim()))
                    target.Add(item.Trim());
            }
        }

        private void ExecuteCustomAbilities(string method)
        {
            if (Modules.Count == 0) return;
            List<string> act = new List<string>();
            List<string> knowledge = new List<string>();
            List<string> social = new List<string>();
            foreach (Module module in Modules)
            {
                AddNonExistingToList(act, module.Skills.Acting);
                AddNonExistingToList(knowledge, module.Skills.Knowledge);
                AddNonExistingToList(social, module.Skills.Social);
            }

            MainForm.Instance.Browser.ExecuteScriptAsync(
                $"{method}CustomAbilities('act', `{JsonConvert.SerializeObject(act)}`)");
            MainForm.Instance.Browser.ExecuteScriptAsync(
                $"{method}CustomAbilities('knowledge', `{JsonConvert.SerializeObject(knowledge)}`)");
            MainForm.Instance.Browser.ExecuteScriptAsync(
                $"{method}CustomAbilities('social', `{JsonConvert.SerializeObject(social)}`)");
        }

        private void Reset()
        {
            _numberInputs.Clear();
            _textInputs.Clear();
            ExecuteCustomAbilities("remove");
            foreach (IElement element in _createdElements)
            {
                element.Destroy();
            }

            _createdElements.Clear();
        }
    }
}
