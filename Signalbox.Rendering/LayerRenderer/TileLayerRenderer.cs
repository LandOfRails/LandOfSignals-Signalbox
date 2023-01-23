using Signalbox.Engine.Utilities;
using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.LayerRenderer.Bases;
using Signalbox.Rendering.Map;

namespace Signalbox.Rendering.LayerRenderer;

[Order(0)]
// Tile layer has its own efficient caching, so doesn't need to be an ICachedLayerRenderer
public class TileLayerRenderer : ILayerRenderer
{
    private readonly IMapRenderer _mapRenderer;

    public TileLayerRenderer(IMapRenderer mapRenderer)
    {
        _mapRenderer = mapRenderer;
    }

    public bool Enabled { get; set; } = true;
    public string Name => "Tile";

    public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
    {
        (int topLeftColumn, int topLeftRow) = pixelMapper.ViewPortPixelsToCoords(0, 0);
        (int bottomRightColumn, int bottomRightRow) = pixelMapper.ViewPortPixelsToCoords(pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight);

        bottomRightColumn += 1;
        bottomRightRow += 1;

        var source = new Rectangle(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow);

        (int destinationTopLeftX, int destinationTopLeftY, _) = pixelMapper.CoordsToViewPortPixels(topLeftColumn, topLeftRow);
        (int destinationBottomRightX, int destinationBottomRightY, _) = pixelMapper.CoordsToViewPortPixels(bottomRightColumn, bottomRightRow);

        var destination = new Rectangle(destinationTopLeftX, destinationTopLeftY, destinationBottomRightX, destinationBottomRightY);

        canvas.DrawImage(_mapRenderer.GetMapImage(), source, destination);
    }
}
