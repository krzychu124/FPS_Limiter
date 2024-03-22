using Colossal.IO.AssetDatabase;
using Game.Modding;

namespace FPS_Limiter
{
    public static class SettingsManager
    {
        private const string SETTINGS_ASSET_NAME = "FPSLimiter General Settings";
        
        public static FPSLimiterSettings Settings { get; private set; }
        
        public static void LoadSettingsAndUI(IMod mod) {
            Settings = new FPSLimiterSettings(mod);
            Settings.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(SETTINGS_ASSET_NAME, Settings, new FPSLimiterSettings(mod));
        }
    }
}
