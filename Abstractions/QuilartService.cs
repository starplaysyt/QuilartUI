using NatLib.Logging;

namespace QuilartUI.Abstractions;

public abstract class QuilartService
{
    protected ConsoleLogger Logger { get; }

    protected QuilartService()
    {
        Logger = LoggerFactory.Create(GetType());
    }

    public abstract void Initialize();

    public abstract void Exit();
}