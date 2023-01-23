﻿using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.UIFramework;
using SkiaSharp;

namespace Signalbox.Rendering.Skia;

public class SKCanvasWrapper : ICanvas
{
    private static readonly Dictionary<PaintBrush, SKPaint> s_paintCache = new();

    private static readonly SKPaint s_noAntialiasPaint = new()
    {
        IsAntialias = false,
        IsDither = false
    };

    private readonly SkiaSharp.SKCanvas _canvas;

    public SKCanvasWrapper(SkiaSharp.SKCanvas canvas)
    {
        _canvas = canvas;
    }
    private static SKPaint GetSKPaint(PaintBrush paint)
    {
        if (!s_paintCache.TryGetValue(paint, out SKPaint? skPaint))
        {
            skPaint = paint.ToSkia();
            s_paintCache.Add(paint, skPaint);
        }
        return skPaint;
    }

    public void Clear(Color color)
        => _canvas.Clear(color.ToSkia());

    public void ClipRect(Rectangle rect, bool antialias, bool exclude)
        => _canvas.ClipRect(rect.ToSkia(), exclude ? SKClipOperation.Difference : SKClipOperation.Intersect, antialias: antialias);

    public void Dispose()
    {
        ((IDisposable)_canvas).Dispose();
    }

    public void DrawPicture(Picture picture, float x, float y, float size)
    {
        var skPicture = picture.ToSkia();

        _canvas.Save();
        float scaleFactor = size / Math.Max(skPicture.CullRect.Width, skPicture.CullRect.Height);
        _canvas.Scale(scaleFactor, scaleFactor, x, y);
        _canvas.DrawPicture(picture.ToSkia());
        _canvas.Restore();
    }

    public void DrawImage(IImage image, int x, int y)
        => _canvas.DrawImage(image.ToSkia(), x, y);

    public void DrawImage(IImage image, Rectangle sourceRectangle, Rectangle destinationRectangle)
        => _canvas.DrawImage(image.ToSkia(), sourceRectangle.ToSkia(), destinationRectangle.ToSkia(), s_noAntialiasPaint);

    public void DrawCircle(float x, float y, float radius, PaintBrush paint)
        => _canvas.DrawCircle(x, y, radius, GetSKPaint(paint));

    public void DrawLine(float x1, float y1, float x2, float y2, PaintBrush paint)
        => _canvas.DrawLine(x1, y1, x2, y2, GetSKPaint(paint));

    public void DrawPath(IPath trackPath, PaintBrush paint)
        => _canvas.DrawPath(trackPath.ToSkia(), GetSKPaint(paint));

    public void DrawRect(float x, float y, float width, float height, PaintBrush paint)
        => _canvas.DrawRect(x, y, width, height, GetSKPaint(paint));

    public void DrawRoundRect(float x, float y, float width, float height, float radiusX, float radiusY, PaintBrush paint)
        => _canvas.DrawRoundRect(x, y, width, height, radiusX, radiusY, GetSKPaint(paint));

    public void DrawText(string text, float x, float y, PaintBrush paint)
        => _canvas.DrawText(text, x, y, GetSKPaint(paint));

    public void DrawGradientRect(float x, float y, float width, float height, Color start, Color end)
        => DrawVerticalGradientRect(x, y, width, height, new[] { start, end, start });

    public void DrawVerticalGradientRect(float x, float y, float width, float height, IEnumerable<Color> colours)
        => DrawGradientRect(x, y, x, y + height, width, height, colours);

    public void DrawHorizontalGradientRect(float x, float y, float width, float height, IEnumerable<Color> colours)
        => DrawGradientRect(x, y, x + width, y, width, height, colours);

    private void DrawGradientRect(float x, float y, float endX, float endY, float width, float height, IEnumerable<Color> colours)
    {
        var shader = SKShader.CreateLinearGradient(new(x, y),
                                                 new(endX, endY),
                                                 colours.Select(RenderingExtensions.ToSkia).ToArray(),
                                                 SKShaderTileMode.Clamp);
        using var paint = new SKPaint
        {
            Shader = shader
        };
        _canvas.DrawRect(x, y, width, height, paint);
    }

    public void DrawGradientCircle(float x, float y, float width, float height, float circleX, float circleY, float radius, IEnumerable<Color> colours)
    {
        var shader = SKShader.CreateRadialGradient(new(circleX, circleY),
                                                   radius,
                                                   colours.Select(RenderingExtensions.ToSkia).ToArray(),
                                                   SKShaderTileMode.Clamp);

        using var paint = new SKPaint
        {
            Shader = shader
        };
        _canvas.DrawRect(x, y, width, height, paint);
    }

    public void Restore()
        => _canvas.Restore();

    public void Scale(float scaleX, float scaleY)
        => _canvas.Scale(scaleX, scaleY);

    public void Scale(float scaleX, float scaleY, float x, float y)
        => _canvas.Scale(scaleX, scaleY, x, y);

    public void RotateDegrees(float degrees, float x, float y)
        => _canvas.RotateDegrees(degrees, x, y);

    public void RotateDegrees(float degrees)
        => _canvas.RotateDegrees(degrees);

    public void Save()
        => _canvas.Save();

    public void Translate(float x, float y)
        => _canvas.Translate(x, y);

    public float MeasureText(string text, PaintBrush paint)
        => GetSKPaint(paint).MeasureText(text);
}
