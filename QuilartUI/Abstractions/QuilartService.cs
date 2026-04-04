using NatLib.Logging;
using QuilartUI.Events;

namespace QuilartUI.Interfaces;

public abstract class QuilartService
{
    protected ConsoleLogger Logger { get; }

    protected QuilartService()
    {
        Logger = new ConsoleLogger(GetType());
    }
    
    public abstract void Initialize();

    public abstract void Exit();
}