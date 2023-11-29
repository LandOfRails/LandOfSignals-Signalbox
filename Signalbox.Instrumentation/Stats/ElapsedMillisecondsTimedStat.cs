﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Signalbox.Instrumentation.Stats;

[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Dispose is used to measure")]
public class ElapsedMillisecondsTimedStat : AveragedStat
{
    private readonly Stopwatch _sw;
    private readonly DisposableCallback _disposableCallback;
    public ElapsedMillisecondsTimedStat()
        : base(10) // Average over 10 samples
    {
        _sw = new();
        _disposableCallback = new();
        _disposableCallback.Disposing += (o, e) => Stop();
    }
    public void Start() => _sw.Restart();
    public void Stop()
    {
        _sw.Stop();
        SetValue(_sw.ElapsedMilliseconds);
    }
    public IDisposable Measure()
    {
        Start();
        return _disposableCallback;
    }
    public override string GetDescription()
    {
        if (Value == null)
        {
            return "null";
        }
        if (Value < 0.01)
        {
            return "< 0.01ms";
        }
        return Math.Round(Value ?? 0, 2).ToString("0.00") + "ms";
    }
}
