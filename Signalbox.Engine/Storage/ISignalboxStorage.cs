namespace Signalbox.Engine.Storage;

public interface ISignalboxStorage
{
    string? Read(string key);
    void Write(string key, string value);
}
