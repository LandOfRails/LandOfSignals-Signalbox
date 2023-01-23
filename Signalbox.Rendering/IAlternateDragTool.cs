namespace Signalbox.Rendering;

public interface IAlternateDragTool
{
    void StartDrag(int x, int y);
    void ContinueDrag(int x, int y);
}
