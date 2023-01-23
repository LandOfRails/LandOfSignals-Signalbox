using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.Tracks;

public interface ITrackPathBuilder
{
    IPath BuildHorizontalTrackPath();
    IPath BuildHorizontalPlankPath();
    IPath BuildCornerTrackPath();
    IPath BuildCornerPlankPath();
    IPath BuildCornerPlankPath(int plankCount);
}
