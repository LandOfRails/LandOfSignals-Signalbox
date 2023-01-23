namespace Signalbox.Engine.Tools;

public interface ICommand
{
    string Name { get; }
    void Execute();
}
