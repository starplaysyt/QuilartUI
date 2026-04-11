using System.Diagnostics;
using QuilartUI.Abstractions;

namespace QuilartUI.Services;

public class FrameCounterService : QuilartService
{
    private readonly Stopwatch _stopwatch = new();
    private int _frameCount;
    private double _currentFps;
    private readonly double _updateInterval;

    public double CurrentFps => _currentFps;
    public double DeltaTime { get; private set; }

    private long _lastTick;

    public FrameCounterService()
    {
        _updateInterval = 0.5;
        _stopwatch.Start();
    }

    public bool Tick()
    {
        // Delta time
        long currentTick = Stopwatch.GetTimestamp();
        if (_lastTick != 0)
            DeltaTime = (currentTick - _lastTick) / (double)Stopwatch.Frequency;
        _lastTick = currentTick;

        // FPS
        _frameCount++;
        bool updated = false;

        if (_stopwatch.Elapsed.TotalSeconds >= _updateInterval)
        {
            _currentFps = _frameCount / _stopwatch.Elapsed.TotalSeconds;
            _frameCount = 0;
            _stopwatch.Restart();
            updated = true;
        }

        return updated;
    }

    public override string ToString() => $"FPS: {_currentFps:F1}";

    public override void Initialize()
    {

    }

    public override void Exit()
    {

    }
}