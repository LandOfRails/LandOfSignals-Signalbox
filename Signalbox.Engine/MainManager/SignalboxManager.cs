using Signalbox.Engine.Tools;
using Signalbox.Engine.Utilities;
using Signalbox.Instrumentation;
using Signalbox.Instrumentation.Stats;

namespace Signalbox.Engine.MainManager;

public class SignalboxManager : ISignalboxManager, IInitializeAsync
{
    private const int GameLoopInterval = 16;

    private bool _buildMode;
    private ITool _currentTool;
    private readonly ITool _defaultTool;
    private readonly ITimer _gameLoopTimer;
    private readonly IEnumerable<ISignalboxStep> _gameSteps;
    private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Game-LoopStepTime");

    public event EventHandler? Changed;

    public bool BuildMode
    {
        get => _buildMode;
        set
        {
            _buildMode = value;
            _currentTool = _defaultTool;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public ITool CurrentTool
    {
        get => _currentTool;
        set
        {
            _currentTool = value;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public SignalboxManager(IEnumerable<ITool> tools, IEnumerable<ISignalboxStep> gameSteps, ITimer timer)
    {
        _defaultTool = tools.First();
        _currentTool = _defaultTool;

        _gameLoopTimer = timer;
        _gameSteps = gameSteps;

        _gameLoopTimer.Interval = GameLoopInterval;
        _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _gameLoopTimer.Start();

        return Task.CompletedTask;
    }

    public void GameLoopStep()
    {
        if (_buildMode) return;

        using (_gameUpdateTime.Measure())
        {
            var timeSinceLastTick = _gameLoopTimer?.TimeSinceLastTick ?? 16;
            foreach (var gameStep in _gameSteps)
            {
                gameStep.Update(timeSinceLastTick);
            }
        }
    }

    private void GameLoopTimerElapsed(object? sender, EventArgs e)
        => GameLoopStep();

    public void Dispose()
    {
        _gameLoopTimer.Dispose();
    }
}
