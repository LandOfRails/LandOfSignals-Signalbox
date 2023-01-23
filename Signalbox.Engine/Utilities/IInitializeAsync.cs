namespace Signalbox.Engine.Utilities;

public interface IInitializeAsync
{
    Task InitializeAsync(int columns, int rows);
}
