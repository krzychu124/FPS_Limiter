using Colossal.IO.AssetDatabase;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Settings;
using UnityEngine;

namespace FPS_Limiter
{
    [FileLocation("FPSLimiter")]
    public sealed class FPSLimiterSettings : ModSetting
    {
        private const string MainSection = "General";  
        
        [SettingsUISection(MainSection)]
        public bool MenuLimitEnabled { get; set; }
        
        [SettingsUISlider(min = 5f, max = 240f, step = 1f, unit = "integer")]
        [SettingsUISection(MainSection)]
        [SettingsUIDisableByCondition(typeof(FPSLimiterSettings), nameof(IsMenuLimitDisabled))]
        public int Menu { get; set; } 
        public bool IsMenuLimitDisabled() => !MenuLimitEnabled;
        
        [SettingsUISection(MainSection)]
        public bool InGameLimitEnabled { get; set; }
        
        [SettingsUISlider(min = 5f, max = 240f, step = 1f, unit = "integer")]
        [SettingsUISection(MainSection)]
        [SettingsUIDisableByCondition(typeof(FPSLimiterSettings), nameof(IsInGameLimitDisabled))]
        public int InGame { get; set; }

        public bool IsInGameLimitDisabled() => !InGameLimitEnabled;
        
        [SettingsUISection(MainSection)]
        public bool PausedLimitEnabled { get; set; }
        
        [SettingsUISlider(min = 5f, max = 240f, step = 1f, unit = "integer")]
        [SettingsUISection(MainSection)]
        [SettingsUIDisableByCondition(typeof(FPSLimiterSettings), nameof(IsPausedLimitDisabled))]
        public int Paused { get; set; } 
        public bool IsPausedLimitDisabled() => !PausedLimitEnabled;

        public FPSLimiterSettings(IMod mod) : base(mod) {
            SetDefaults();
        }
        
        public override void SetDefaults() {
            MenuLimitEnabled = true;
            InGameLimitEnabled = true;
            PausedLimitEnabled = true;
            Menu = 30;
            InGame = 30;
            Paused = 30;
        }

        public override void Apply() {
            base.Apply();

            if (!GameManager.instance.gameMode.IsGameOrEditor()) {
                // most likely we are in the Main menu
                QualitySettings.vSyncCount = !MenuLimitEnabled && SharedSettings.instance.graphics.vSync ? 1 : 0;
                Application.targetFrameRate = MenuLimitEnabled ? Menu : -1;
            }
        }
    }
}
