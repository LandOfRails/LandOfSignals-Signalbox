﻿using System.Diagnostics;
using Signalbox.Engine.Utilities;

namespace LandOfSignals_Signalbox.Client;

public class BlazorTimer : ITimer
{
    private CancellationTokenSource? _cts;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private long _lastTick;

    public double Interval { get; set; }

    public long TimeSinceLastTick { get; private set; }

    public event EventHandler? Elapsed;

    public void Dispose()
    {
        Stop();
    }

    public void Start()
    {
        Stop();
        _cts = new CancellationTokenSource();
        _ = StartTimer();
    }

    private async Task StartTimer()
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(Interval));
        while (await timer.WaitForNextTickAsync(_cts!.Token))
        {
            var time = _stopwatch.ElapsedMilliseconds;
            TimeSinceLastTick = time - _lastTick;
            _lastTick = time;
            Elapsed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
    }
}
