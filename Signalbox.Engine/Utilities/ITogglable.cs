namespace Signalbox.Engine.Utilities;

public interface ITogglable
{
    string Name { get; }
    bool Enabled { get; set; }
}
