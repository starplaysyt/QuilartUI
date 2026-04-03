using NatLib.Logging;
using SDL;

namespace QuilartUI.Extensions;

public static class LoggerExtensions
{
    public static void LogSDLIfFalse(this ConsoleLogger logger, bool operationResult)
    {
        if (!operationResult) logger.LogError(SDL3.SDL_GetError() ?? "empty");
    }

    public static unsafe T* LogSDLIfNullPtr<T>(this ConsoleLogger logger, T* pointer) where T : unmanaged
    {
        if (pointer == null) logger.LogError(SDL3.SDL_GetError() ?? "empty");
        return pointer;
    }
}