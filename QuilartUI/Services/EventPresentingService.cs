using NatLib.Logging;
using QuilartUI.Interfaces;
using SDL;

namespace QuilartUI.Services;

public sealed class EventPresentingService : IQuilartService
{
    private Dictionary<SDL_EventType, object> EventObjects { get; } = new();
    
    public static ConsoleLogger Logger { get; } = new(typeof(EventPresentingService));
    
    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public void Exit()
    {
        throw new NotImplementedException();
    }
}