using QuilartUI.Interfaces;

namespace QuilartUI.Services;

public class FrameLimiterService : QuilartService
{
    public int TargetFrameRate
    {
        get;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Target frame rate must be greater than zero.");
            field = value;
            _targetMs = 1000 / field;
        }
    }
    
    public long LastTicks => _lastTicks;
    
    public long TargetMs => _targetMs;

    private long _lastTicks;
    private long _targetMs;
    
    public override void Initialize()
    {
        TargetFrameRate = 60;
        Logger.LogTrace("FrameLimiter initialized.");
    }

    public void Wait()
    {
        var elapsed = Environment.TickCount64 - _lastTicks;
        var sleep = Convert.ToInt32(_targetMs - elapsed);
        
        if (sleep > 0) Thread.Sleep(sleep);

        _lastTicks = Environment.TickCount64;
    }

    public override void Exit()
    {
        Logger.LogTrace("FrameLimiter exited.");
    }
}