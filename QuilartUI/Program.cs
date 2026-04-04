using NatLib.Logging;
using QuilartUI.Controllers;
using QuilartUI.Elements;
using QuilartUI.Services;

namespace QuilartUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggingConfiguration.Instance.MinimumLevel = LogLevel.Trace;
            var window = new UIWindow();
            
            ConsoleLogger.Instance.LogTrace($"{LoggerFactory.LoggersCount}");

            ServiceController.Get<WindowHandlerService>().Run();
        }
    }
}