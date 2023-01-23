namespace Signalbox.Engine.MainManager;

public interface ISignalboxStep
{
    void Update(long timeSinceLastTick);
}
