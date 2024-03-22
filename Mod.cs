using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Colossal.IO.AssetDatabase;
using Colossal.Localization;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Unity.Entities;

namespace FPS_Limiter
{
    public class Mod : IMod
    {
        private FPSLimiterSystem _fpsLimiterSystem;

        public void OnLoad(UpdateSystem updateSystem) {
            LoadLocales();
            SettingsManager.LoadSettingsAndUI(this);
            updateSystem.UpdateAt<FPSLimiterSystem>(SystemUpdatePhase.LateUpdate);
            _fpsLimiterSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<FPSLimiterSystem>();
        }

        public void OnDispose() {
            _fpsLimiterSystem.Enabled = false;
            _fpsLimiterSystem = null;
        }
        
        private void LoadLocales()
        {
            string file = Path.Combine(ModInstallFolder, $"i18n.csv");
            if (!File.Exists(file)) return;
            
            IEnumerable<string[]> fileLines = File.ReadAllLines(file).Select(x => x.Split('\t'));
            Logger.LogDebugInfo($"READ: \n{string.Join("\n", fileLines.Select(t => string.Join("|", t)))} \nEND");
            int enColumn = Array.IndexOf(fileLines.First(), "en-US");
            MemorySource enMemoryFile = new MemorySource(fileLines.Skip(1).ToDictionary(x => x[0], x => x.ElementAtOrDefault(enColumn)));
            foreach (string lang in GameManager.instance.localizationManager.GetSupportedLocales())
            {
                GameManager.instance.localizationManager.AddSource(lang, enMemoryFile);
                if (lang != "en-US")
                {
                    int valueColumn = Array.IndexOf(fileLines.First(), lang);
                    if (valueColumn > 0)
                    {
                        Logger.LogInfo($"Other locale: ${valueColumn}|{lang}");
                        MemorySource i18nFile = new MemorySource(fileLines.Skip(1).ToDictionary(x => x[0], x => x.ElementAtOrDefault(valueColumn)));
                        GameManager.instance.localizationManager.AddSource(lang, i18nFile);
                    }
                }
            }
            Logger.LogInfo("Locale loaded");
        }
        
        private string ModInstallFolder
        {
            get
            {
                if (_modInstallFolder is null)
                {
                    string thisFullName = Assembly.GetExecutingAssembly().FullName;                   
                    ExecutableAsset thisInfo = AssetDatabase.global.GetAsset(SearchFilter<ExecutableAsset>.ByCondition(x => x.definition?.FullName == thisFullName));
                    if (thisInfo is null)
                    {
                        throw new Exception($"{nameof(FPS_Limiter)}.{nameof(Mod)} mod info was not found!!!!");
                    }
                    _modInstallFolder = Path.GetDirectoryName(thisInfo.GetMeta().path);
                }
                return _modInstallFolder;
            }
        }

        private static string _modInstallFolder;
    }
}
