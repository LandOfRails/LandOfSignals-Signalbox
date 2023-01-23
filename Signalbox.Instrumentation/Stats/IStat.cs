namespace Signalbox.Instrumentation.Stats;

public interface IStat
{
    string GetDescription();

    bool ShouldShow();
}
