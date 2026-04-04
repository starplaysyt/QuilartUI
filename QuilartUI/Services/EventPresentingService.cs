using NatLib.Logging;
using QuilartUI.Interfaces;
using SDL;

namespace QuilartUI.Services;

public sealed class EventPresentingService : QuilartService
{
    private Dictionary<SDL_EventType, object> EventObjects { get; } = new();
    
    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    public override void Exit()
    {
        throw new NotImplementedException();
    }
}