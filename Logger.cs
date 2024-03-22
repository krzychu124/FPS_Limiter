using System.Diagnostics;
using System.Runtime.CompilerServices;
using Colossal.Logging;

namespace FPS_Limiter
{
    public static class Logger
    {
        private static ILog _log = LogManager.GetLogger($"{nameof(FPS_Limiter)}.{nameof(Mod)}");

        [Conditional("DEBUG")]
        public static void LogDebugInfo(string message, [CallerMemberName] string memberName = "") {
            _log.Debug($"[FPSLimiter.{memberName}()] {message}");
        }
        
        public static void LogInfo(string message) {
            _log.Info(message);
        }
    }
}
