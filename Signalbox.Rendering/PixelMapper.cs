﻿using Signalbox.Engine.StateManager;
using Signalbox.Engine.Storage;
using Signalbox.Engine.Utilities;

namespace Signalbox.Rendering;

public class PixelMapper : IPixelMapper, IInitializeAsync, ISignalboxState
{
    private int _columns;
    private int _rows;
    private bool _firstViewPortAdjustment = true;
    private int? _initialViewPortX;
    private int? _initialViewPortY;

    public int Columns => _columns;
    public int Rows => _rows;

    public int MaxGridWidth => _columns * this.CellSize;
    public int MaxGridHeight => _rows * this.CellSize;

    public float GameScale { get; private set; } = 1.0f;
    public int ViewPortX { get; private set; }
    public int ViewPortY { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ViewPortHeight { get; private set; }

    public int CellSize => (int)(40 * this.GameScale);

    public event EventHandler? ViewPortChanged;

    public Task InitializeAsync(int columns, int rows)
    {
        _columns = columns;
        _rows = rows;

        return Task.CompletedTask;
    }

    public void SetViewPortSize(int width, int height)
    {
        this.ViewPortWidth = width;
        this.ViewPortHeight = height;

        if (_firstViewPortAdjustment && _initialViewPortX.HasValue && _initialViewPortY.HasValue)
        {
            SetViewPort(_initialViewPortX.Value, _initialViewPortY.Value);
        }
        else if (_firstViewPortAdjustment)
        {
            SetViewPort((this.MaxGridWidth - width) / 2, (this.MaxGridHeight - height) / 2);
        }
        else
        {
            AdjustViewPort(0, 0);
        }

        _firstViewPortAdjustment = false;
    }

    public void SetViewPort(int x, int y)
    {
        int oldX = this.ViewPortX;
        int oldY = this.ViewPortY;
        this.ViewPortX = Math.Max(Math.Min(-x, 0), -1 * (this.MaxGridWidth - this.ViewPortWidth));
        this.ViewPortY = Math.Max(Math.Min(-y, 0), -1 * (this.MaxGridHeight - this.ViewPortHeight));

        if (this.ViewPortX != oldX || this.ViewPortY != oldY)
        {
            ViewPortChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void AdjustViewPort(int x, int y)
    {
        SetViewPort(-1 * (this.ViewPortX + x), -1 * (this.ViewPortY + y));
    }

    public (int, int) ViewPortPixelsToCoords(int x, int y)
    {
        return ((x - this.ViewPortX) / this.CellSize, (y - this.ViewPortY) / this.CellSize);
    }

    public (int, int, bool) CoordsToViewPortPixels(int column, int row)
    {
        int x = (column * this.CellSize) + this.ViewPortX;
        int y = (row * this.CellSize) + this.ViewPortY;
        bool onScreen = x > -this.CellSize &&
                        y > -this.CellSize &&
                        x <= this.ViewPortWidth &&
                        y <= this.ViewPortHeight;
        return (x, y, onScreen);
    }

    public (int, int) WorldPixelsToCoords(int x, int y)
    {
        return (x / this.CellSize, y / this.CellSize);
    }

    public (int, int) CoordsToWorldPixels(int column, int row)
    {
        return (column * this.CellSize, row * this.CellSize);
    }

    public IPixelMapper Snapshot()
    {
        return new PixelMapper()
        {
            _columns = _columns,
            _rows = _rows,
            ViewPortX = this.ViewPortX,
            ViewPortY = this.ViewPortY,
            ViewPortHeight = this.ViewPortHeight,
            ViewPortWidth = this.ViewPortWidth,
            GameScale = this.GameScale
        };
    }

    private (float, float) GetScaledViewPortSize()
        => GetScaledViewPortSize(this.GameScale);

    private (float, float) GetScaledViewPortSize(float scale)
        => (this.ViewPortWidth / scale,
            this.ViewPortHeight / scale);

    public bool AdjustGameScale(float delta)
    {
        float newGameScale = this.GameScale * delta;

        // Check to see if it is TOO FAR!
        if (newGameScale < 0.1 ||
            this.MaxGridWidth / this.GameScale * newGameScale < this.ViewPortWidth ||
            this.MaxGridHeight / this.GameScale * newGameScale < this.ViewPortHeight)
        {
            return false;
        }
        else if (newGameScale > 5)
        {
            newGameScale = 5.0f;
        }

        if (this.GameScale == newGameScale)
        {
            return false;
        }

        // Viewport X & Y will be negative, as they are canvas transations, so swap em!
        float currentX = -this.ViewPortX / this.GameScale;
        float currentY = -this.ViewPortY / this.GameScale;

        (float svpWidth, float svpHeight) = GetScaledViewPortSize();

        float currentCenterX = currentX + svpWidth / 2.0f;
        float currentCenterY = currentY + svpHeight / 2.0f;

        (float newSvpWidth, float newSvpHeight) = GetScaledViewPortSize(newGameScale);

        float newX = currentCenterX - newSvpWidth / 2.0f;
        float newY = currentCenterY - newSvpHeight / 2.0f;

        this.ViewPortX = -(int)Math.Round(newX * newGameScale);
        this.ViewPortY = -(int)Math.Round(newY * newGameScale);

        this.GameScale = newGameScale;

        AdjustViewPort(0, 0);

        ViewPortChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    public bool Load(ISignalboxStorage storage)
    {
        // We always return true from this method
        // because if the saved viewport isn't valid it's not worth
        // resetting the users signalbox over.

        var data = storage.Read(nameof(PixelMapper));

        if (data is null)
            return true;

        var bits = data.Split(",");
        if (bits.Length != 3)
            return true;

        if (!int.TryParse(bits[0], out var x) ||
            !int.TryParse(bits[1], out var y) ||
            !float.TryParse(bits[2], out var gameScale))
        {
            return true;
        }

        _initialViewPortX = -x;
        _initialViewPortY = -y;
        this.GameScale = gameScale;

        return true;
    }

    public void Save(ISignalboxStorage storage)
    {
        storage.Write(nameof(PixelMapper), $"{this.ViewPortX},{this.ViewPortY},{this.GameScale}");
    }

    public void Reset()
    {
    }
}
