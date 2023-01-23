﻿using Signalbox.Rendering.Drawing;
using SkiaSharp;

namespace Signalbox.Rendering.Skia;

public class SKImageWrapper : IImage
{
    private readonly SKImage _image;

    public SKImageWrapper(SKImage sKImage)
    {
        _image = sKImage;
    }

    public SKImage Image => _image;

    public void Dispose()
    {
        _image.Dispose();
    }
}
