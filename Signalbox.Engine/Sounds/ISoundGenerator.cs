namespace Signalbox.Engine.Sounds;

public interface ISoundGenerator : IDisposable
{
    bool IsRunning { get; }
    void Start();
    void Stop();
}
