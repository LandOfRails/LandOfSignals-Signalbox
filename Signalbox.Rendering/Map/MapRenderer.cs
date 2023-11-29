using Signalbox.Engine.Map;
using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.Map;

public class MapRenderer : IMapRenderer
{
    private readonly IMap _map;
    private readonly IImageFactory _imageFactory;
    private readonly IImageCache _imageCache;
    private readonly IPixelMapper _pixelMapper;

    public event EventHandler? Changed;

    public MapRenderer(IMap map, IImageFactory imageFactory, IImageCache imageCache, IPixelMapper pixelMapper)
    {
        _map = map;
        _imageFactory = imageFactory;
        _imageCache = imageCache;
        _pixelMapper = pixelMapper;

        _map.CollectionChanged += (_, _) => _imageCache.SetDirty(this);
    }
    public IImage GetMapImage()
    {
        if (_imageCache.IsDirty(this))
        {
            var width = _pixelMapper.Columns;
            var _height = _pixelMapper.Rows;

            using var textureImage = _imageFactory.CreateImageCanvas(width, _height);

            foreach (var tile in _map)
            {
                var colour = GetTileColour(tile);
                var paint = GetPaint(colour);

                textureImage.Canvas.DrawRect(tile.Column, tile.Row, 1, 1, paint);
            }

            _imageCache.Set(this, textureImage.Render());

            Changed?.Invoke(this, EventArgs.Empty);
        }

        return _imageCache.Get(this)!;
    }
    private static PaintBrush GetPaint(Color colour)
        => new()
        {
            Style = PaintStyle.Fill,
            Color = colour
        };

    public static Color GetTileColour(Tile tile) => string.IsNullOrWhiteSpace(tile.Content) ? Colors.Empty : Colors.Black;
}
