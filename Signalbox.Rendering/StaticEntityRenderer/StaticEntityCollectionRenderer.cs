﻿using Signalbox.Engine.Entity;
using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.LayerRenderer.Bases;

namespace Signalbox.Rendering.StaticEntityRenderer;

public abstract class StaticEntityCollectionRenderer<T> : ICachableLayerRenderer where T : class, IStaticEntity
{
    private readonly ILayout<T> _layout;
    private readonly IEnumerable<IStaticEntityRenderer<T>> _renderers;
    private readonly IImageFactory _imageFactory;
    private readonly IImageCache _imageCache;

    public event EventHandler? Changed;

    public bool Enabled { get; set; } = true;
    public string Name { get; internal set; } = string.Empty;
    public bool IsScaledByHeight { get; set; }

    private int _lastCellSize;

    public StaticEntityCollectionRenderer(ILayout<T> layout, IEnumerable<IStaticEntityRenderer<T>> renderers, IImageFactory imageFactory, IImageCache imageCache)
    {
        _layout = layout;
        _renderers = renderers;
        _imageFactory = imageFactory;
        _imageCache = imageCache;

        layout.CollectionChanged += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
    {
        if (pixelMapper.CellSize != _lastCellSize)
        {
            _imageCache.Clear();
            _lastCellSize = pixelMapper.CellSize;
        }

        foreach (var entity in _layout)
        {
            var renderer = _renderers.FirstOrDefault(r => r.ShouldRender(entity));
            if (renderer is null)
            {
                // TODO: Fill with Red to indicate error?
                continue;
            }

            var (x, y, onScreen) = pixelMapper.CoordsToViewPortPixels(entity.Column, entity.Row);

            if (!onScreen)
            {
                continue;
            }

            var key = $"{entity.GetType().Name}.{entity.Identifier}";
            if (_imageCache.IsDirty(key))
            {
                using var imageCanvas = _imageFactory.CreateImageCanvas(pixelMapper.CellSize, pixelMapper.CellSize);

                var scale = pixelMapper.CellSize / 100.0f;

                imageCanvas.Canvas.Scale(scale, scale);

                renderer.Render(imageCanvas.Canvas, entity);

                _imageCache.Set(key, imageCanvas.Render());
            }

            using (canvas.Scope())
            {
                canvas.Translate(x, y);
                canvas.ClipRect(new(0, 0, pixelMapper.CellSize, pixelMapper.CellSize), false, false);
                canvas.DrawImage(_imageCache.Get(key)!, 0, 0);
            }
        }
    }
}
