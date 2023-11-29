﻿using System.Text;
using Signalbox.Engine.MainManager;
using Signalbox.Engine.Trains;
using Signalbox.Engine.Utilities;
using Signalbox.Instrumentation;
using Signalbox.Instrumentation.Stats;
using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.LayerRenderer.Bases;
using Signalbox.Rendering.UIFramework;

namespace Signalbox.Rendering.Signalbox;

public class Signalbox : ISignalbox
{
    private int _width;
    private int _height;
    private int _screenWidth;
    private int _screenHeight;
    private readonly ISignalboxManager _signalboxManager;
    private readonly ITrainManager _trainManager;
    private readonly IEnumerable<ILayerRenderer> _boardRenderers;
    private readonly IPixelMapper _pixelMapper;
    private readonly IImageFactory _imageFactory;
    private readonly PerSecondTimedStat _skiaFps = InstrumentationBag.Add<PerSecondTimedStat>("Draw-FPS-Skia");
    private readonly ElapsedMillisecondsTimedStat _skiaDrawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Skia-AllUp");
    private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderLayerDrawTimes;
    private readonly Dictionary<IScreen, ElapsedMillisecondsTimedStat> _screenDrawTimes;
    private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderCacheDrawTimes;
    private readonly IEnumerable<IScreen> _screens;
    private readonly IImageCache _imageCache;
    private readonly IEnumerable<IInitializeAsync> _initializers;

    public Signalbox(ISignalboxManager signalboxManager,
                ITrainManager trainManager,
                IEnumerable<ILayerRenderer> boardRenderers,
                IPixelMapper pixelMapper,
                IImageFactory imageFactory,
                IEnumerable<IScreen> screens,
                IImageCache imageCache,
                IEnumerable<IInitializeAsync> initializers)
    {
        _signalboxManager = signalboxManager;
        _trainManager = trainManager;
        _boardRenderers = boardRenderers;
        _pixelMapper = pixelMapper;
        _imageFactory = imageFactory;
        _screens = screens;
        _imageCache = imageCache;
        _initializers = initializers;
        foreach (var screen in _screens)
        {
            screen.Changed += (s, e) => _imageCache.SetDirty(screen);
        }
        foreach (var renderer in _boardRenderers.OfType<ICachableLayerRenderer>())
        {
            renderer.Changed += (s, e) => _imageCache.SetDirty(renderer);
        }

        _renderLayerDrawTimes = _boardRenderers.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
        _screenDrawTimes = _screens.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
        _renderCacheDrawTimes = _boardRenderers.Where(x => x is ICachableLayerRenderer).ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Cache-" + x.Name.Replace(" ", "")));
        _pixelMapper.ViewPortChanged += (s, e) => _imageCache.SetDirtyAll(_boardRenderers);
    }

    public async Task InitializeAsync(int columns, int rows)
    {
        foreach (var initializer in _initializers)
        {
            await initializer.InitializeAsync(columns, rows);
        }
    }

    private static string GetLayerDiagnosticsName(ILayerRenderer layerRenderer)
    {
        var sb = new StringBuilder("Draw-Layer-");
        sb.Append(layerRenderer.Name.Replace(" ", ""));
        if (layerRenderer is ICachableLayerRenderer)
        {
            sb.Append("[Cached]");
        }
        return sb.ToString();
    }
    private static string GetLayerDiagnosticsName(IScreen screen)
    {
        var sb = new StringBuilder("Draw-Screen-");
        sb.Append(screen.GetType().Name.Replace(" ", ""));
        return sb.ToString();
    }

    public void SetSize(int width, int height)
    {
        _screenWidth = width;
        _screenHeight = height;

        _imageCache.SetDirtyAll(_screens);

        var (columns, rows) = _pixelMapper.ViewPortPixelsToCoords(width, height);
        columns = Math.Max(columns, 1);
        rows = Math.Max(rows, 1);

        (width, height, _) = _pixelMapper.CoordsToViewPortPixels(columns + 1, rows + 1);

        if (_width != width || _height != height)
        {
            _pixelMapper.SetViewPortSize(width, height);
            _width = width;
            _height = height;

            // strictly speaking this only needs to clear renderers, but we already cleared screens
            // so its easier just to call clear
            _imageCache.Clear();
        }
    }

    public (int Width, int Height) GetSize() => (_width, _height);

    public (int Width, int Height) GetScreenSize() => (_screenWidth, _screenHeight);

    public void SetContext(IContext context)
    {
        if (_imageFactory.SetContext(context))
        {
            _imageCache.Clear();
        }
    }

    public void Render(ICanvas canvas)
    {
        if (_width == 0 || _height == 0)
        {
            return;
        }

        AdjustViewPortIfNecessary();

        canvas.Clear(Colors.White);

        var pixelMapper = _pixelMapper.Snapshot();

        using (canvas.Scope())
        {
            RenderFrame(canvas, pixelMapper);
        }

        foreach (var screen in _screens)
        {
            using (_screenDrawTimes[screen].Measure())
            {
                using (canvas.Scope())
                {
                    screen.Render(canvas, _screenWidth, _screenHeight);
                }
            }
        }
    }

    private void RenderFrame(ICanvas canvas, IPixelMapper pixelMapper)
    {
        using (_skiaDrawTime.Measure())
        using (canvas.Scope())
        {
            foreach (var renderer in _boardRenderers)
            {
                if (!renderer.Enabled)
                {
                    continue;
                }
                using (canvas.Scope())
                {
                    RenderLayer(canvas, pixelMapper, renderer);
                }
            }
        }

        _skiaFps.Update();
    }

    private void RenderLayer(ICanvas canvas, IPixelMapper pixelMapper, ILayerRenderer renderer)
    {
        if (renderer is ICachableLayerRenderer)
        {
            if (_imageCache.IsDirty(renderer))
            {
                using (_renderCacheDrawTimes[renderer].Measure())
                {
                    using var imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);
                    renderer.Render(imageCanvas.Canvas, _width, _height, pixelMapper);
                    _imageCache.Set(renderer, imageCanvas.Render());
                }
            }

            using (_renderLayerDrawTimes[renderer].Measure())
            {
                canvas.DrawImage(_imageCache.Get(renderer)!, 0, 0);
            }
        }
        else
        {
            using (_renderLayerDrawTimes[renderer].Measure())
            {
                renderer.Render(canvas, _width, _height, pixelMapper);
            }
        }
    }

    public void AdjustViewPortIfNecessary()
    {
        if (_signalboxManager.BuildMode) return;

        if (!_trainManager.TryGetFollowTrainPosition(out var col, out var row)) return;

        var (x, y, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

        double easing = 10;
        var adjustX = Convert.ToInt32(((_pixelMapper.ViewPortWidth / 2) - x) / easing);
        var adjustY = Convert.ToInt32(((_pixelMapper.ViewPortHeight / 2) - y) / easing);

        if (adjustX != 0 || adjustY != 0)
        {
            _pixelMapper.AdjustViewPort(adjustX, adjustY);
        }
    }

    public void Dispose()
    {
        _imageCache.Dispose();
        _signalboxManager.Dispose();
    }
}
