using System;
using Colossal.Serialization.Entities;
using Game;
using Game.Settings;
using Game.Simulation;
using Game.UI.InGame;
using UnityEngine;

namespace FPS_Limiter
{
    public partial class FPSLimiterSystem : GameSystemBase
    {
        private float _previousSimSpeed = 10f;
        private GameMode _currentStage;
        private SimulationSystem _simulationSystem;
        private GameScreenUISystem _gameScreenUISystem;
        private FPSLimiterSettings _fpsLimiterSettings;

        protected override void OnCreate() {
            base.OnCreate();
            _simulationSystem = World.GetExistingSystemManaged<SimulationSystem>();
            _gameScreenUISystem = World.GetExistingSystemManaged<GameScreenUISystem>();
            _fpsLimiterSettings = SettingsManager.Settings;
            UpdateFpsLimit(simRunning: true); // assuming we are in main menu
        }

        protected override void OnUpdate() {
            if (_gameScreenUISystem != null && _gameScreenUISystem.isMenuActive)
            {
                Logger.LogDebugInfo("isMainMenu...");
                _previousSimSpeed = 10f;
                QualitySettings.vSyncCount = !_fpsLimiterSettings.MenuLimitEnabled && SharedSettings.instance.graphics.vSync ? 1 : 0;
                Application.targetFrameRate = _fpsLimiterSettings.MenuLimitEnabled ? _fpsLimiterSettings.Menu : -1;
                return;
            }

            if ((_currentStage & GameMode.GameOrEditor) != 0 && Math.Abs(_simulationSystem.selectedSpeed - _previousSimSpeed) > float.Epsilon)
            {
                _previousSimSpeed = _simulationSystem.selectedSpeed;
                Logger.LogDebugInfo($"Detected simulation speed change to: {_previousSimSpeed} (0 - paused)");

                UpdateFpsLimit(simRunning: _previousSimSpeed > 0.01f);
            }
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode) {
            Logger.LogDebugInfo($"Scene changed to: {mode}");
            _currentStage = mode;
            if (_gameScreenUISystem == null)
            {
                _gameScreenUISystem = World.GetExistingSystemManaged<GameScreenUISystem>();
                Logger.LogDebugInfo($"Getting ref of GameScreenUISystem... Found? {_gameScreenUISystem != null}");
            }
            if (_simulationSystem == null)
            {
                _simulationSystem = World.GetExistingSystemManaged<SimulationSystem>();
                Logger.LogDebugInfo($"Getting ref of SimulationSystem... Found? {_simulationSystem != null}");
            }
            
            switch (mode)
            {
                case GameMode.Game:
                case GameMode.Editor:
                case GameMode.GameOrEditor:
                    UpdateFpsLimit(!SharedSettings.instance.gameplay.pausedAfterLoading);
                    break;
                case GameMode.MainMenu:
                    _previousSimSpeed = 10f;
                    QualitySettings.vSyncCount = !_fpsLimiterSettings.MenuLimitEnabled && SharedSettings.instance.graphics.vSync ? 1 : 0;
                    Application.targetFrameRate = _fpsLimiterSettings.MenuLimitEnabled ? _fpsLimiterSettings.Menu : -1;
                    break;
            }
        }

        private void UpdateFpsLimit(bool simRunning) {
            Logger.LogDebugInfo($"Updating FPS Limit state. Disable? {simRunning}");
            if (simRunning)
            {
                QualitySettings.vSyncCount = !_fpsLimiterSettings.InGameLimitEnabled && SharedSettings.instance.graphics.vSync ? 1 : 0;
                Application.targetFrameRate = _fpsLimiterSettings.InGameLimitEnabled ? _fpsLimiterSettings.InGame : -1;
            }
            else
            {
                bool limitEnabled = _fpsLimiterSettings.PausedLimitEnabled || _fpsLimiterSettings.InGameLimitEnabled;
                QualitySettings.vSyncCount = !limitEnabled && SharedSettings.instance.graphics.vSync ? 1 : 0;
                Application.targetFrameRate = _fpsLimiterSettings.PausedLimitEnabled 
                    ? _fpsLimiterSettings.Paused 
                    : _fpsLimiterSettings.InGameLimitEnabled 
                        ? _fpsLimiterSettings.InGame
                        : -1;
            }
        }
    }
}
