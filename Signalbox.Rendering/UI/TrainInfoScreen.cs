﻿using Signalbox.Engine.Entity;
using Signalbox.Engine.MainManager;
using Signalbox.Engine.Trains;
using Signalbox.Engine.Utilities;
using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.Trains;
using Signalbox.Rendering.UIFramework;

namespace Signalbox.Rendering.UI;

[Order(20)]
public class TrainInfoScreen : PanelBase
{
    private const int TrainDisplayAreaWidth = 50;
    private const int PanelWidth = 280 + TrainDisplayAreaWidth;

    private readonly ITrainManager _trainManager;
    private readonly ISignalboxManager _signalboxManager;
    private readonly IMovableLayout _movableLayout;
    private readonly ITrainParameters _trainParameters;
    private readonly ITrainPainter _trainPainter;
    private readonly MultiButton _controlButton;
    private readonly MultiButton _actionButton;
    private readonly MultiButton _trainSelectionButton;

    protected override PanelPosition Position => PanelPosition.Right;
    protected override int Left => (PanelWidth + 55) * -1;
    protected override int Top => 50;
    protected override int InnerHeight => 30;
    protected override int InnerWidth => PanelWidth;
    protected override bool CanClose => true;
    protected override string? Title => "Info";

    public TrainInfoScreen(ITrainManager trainManager, ISignalboxManager signalboxManager, IMovableLayout movableLayout, ITrainParameters trainParameters, ITrainPainter trainPainter)
    {
        _trainManager = trainManager;
        _signalboxManager = signalboxManager;
        _movableLayout = movableLayout;
        _trainParameters = trainParameters;
        _trainPainter = trainPainter;
        _trainManager.Changed += (_, _) =>
        {
            Visible = _trainManager.CurrentTrain is not null;
            OnChanged();
        };
        _signalboxManager.Changed += (_, _) => OnChanged();
        _trainManager.CurrentTrainPropertyChanged += (_, _) => OnChanged();

        _controlButton = new(20, CreateButton(Picture.Play, () => _trainManager.CurrentTrain?.Stopped != true, () => _trainManager.CurrentTrain?.Start()), CreateButton(Picture.Pause, () => _trainManager.CurrentTrain?.Stopped == true, () => _trainManager.CurrentTrain?.Stop()));

        _actionButton = new(20, CreateButton(Picture.Eye, () => _trainManager.CurrentTrain?.Follow ?? false, () => _trainManager.ToggleFollow(_trainManager.CurrentTrain!)), CreateButton(Picture.Trash, () => false, () =>
        {
            _movableLayout.Remove(_trainManager.CurrentTrain!);
            Close();
        }));

        _trainSelectionButton = new(20, CreateButton(Picture.Left, () => false, () => _trainManager.PreviousTrain()), CreateButton(Picture.Right, () => false, () => _trainManager.NextTrain()));

        Visible = _trainManager.CurrentTrain is not null;
    }

    private static ButtonBase CreateButton(Picture picture, Func<bool> isActive, Action onClick)
        => new PictureButton(picture, 16, isActive, onClick)
        {
            TransparentBackground = true,
        };

    protected override void Close()
    {
        _trainManager.CurrentTrain = null;
    }

    protected override bool HandlePointerAction(int x, int y, PointerAction action)
    {
        y -= 30;
        if (_controlButton.HandleMouseAction(x, y, action))
        {
            return true;
        }

        x -= PanelWidth - 80;
        if (_actionButton.HandleMouseAction(x, y, action))
        {
            return true;
        }

        x -= 40;
        y += 40;
        _trainSelectionButton.HandleMouseAction(x, y, action);

        return true;
    }

    protected override void Render(ICanvas canvas)
    {
        var train = _trainManager.CurrentTrain ?? throw new NullReferenceException("Current train is null so we shouldn't be rendering");

        using (canvas.Scope())
        {
            canvas.Translate(TrainDisplayAreaWidth / 2, 5);
            canvas.Scale(0.5f, 0.5f);
            var palette = _trainPainter.GetPalette(train);
            TrainRenderer.RenderTrain(canvas, palette, _trainParameters, false);
        }

        using (canvas.Scope())
        {
            canvas.Translate(TrainDisplayAreaWidth + 10, 10);

            canvas.DrawText(train.Name, 0, 0, Brushes.Label);
        }

        canvas.Translate(0, 30);

        _controlButton.Render(canvas);

        canvas.Translate(PanelWidth - 80, 0);
        _actionButton.Render(canvas);

        canvas.Translate(40, -40);
        _trainSelectionButton.Render(canvas);
    }
}
