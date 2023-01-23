using Signalbox.Engine.Tracks;
using Signalbox.Engine.Tracks.CrossTrack;
using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.StaticEntityRenderer;

namespace Signalbox.Rendering.Tracks;

public class CrossTrackRenderer : SpecializedEntityRenderer<CrossTrack, Track>
{
    private readonly SingleTrackRenderer _trackRenderer;

    public CrossTrackRenderer(SingleTrackRenderer trackRenderer)
    {
        _trackRenderer = trackRenderer;
    }

    protected override void Render(ICanvas canvas, CrossTrack item)
    {
        _trackRenderer.DrawCross(canvas);
    }
}
