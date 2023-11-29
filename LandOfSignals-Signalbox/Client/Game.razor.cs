using DI;
using Microsoft.AspNetCore.Components.Web;
using Signalbox.Instrumentation;
using Signalbox.Instrumentation.Stats;
using Signalbox.Rendering.Signalbox;
using Signalbox.Rendering.Skia;
using Signalbox.Rendering.UIFramework;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace LandOfSignals_Signalbox.Client;

public partial class Game
{
    private ISignalbox _game = null!;
    private IInteractionManager _interactionManager = null!;

    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SkiaSharp-OnPaintSurfaceFPS");
    private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-GameRender");

    protected override async Task OnInitializedAsync()
    {
        _game = ServiceLocator.GetService<ISignalbox>();
        _interactionManager = ServiceLocator.GetService<IInteractionManager>();

        await _game.InitializeAsync(200, 200);
    }

    private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
    {
        using (_renderTime.Measure())
        {
            _game.SetSize(e.Info.Width, e.Info.Height);
            if (e.Surface.Context is GRContext context && context != null)
            {
                // Set the context so all rendering happens in the same place
                _game.SetContext(new SKContextWrapper(context));
            }
            _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }

        _fps.Update();
    }

    private void OnPointerDown(PointerEventArgs e)
    {
        switch (e.Buttons)
        {
            case 1:
                _interactionManager.PointerClick((int)e.OffsetX, (int)e.OffsetY);
                break;
            case 2:
                _interactionManager.PointerAlternateClick((int)e.OffsetX, (int)e.OffsetY);
                break;
        }
    }

    private void OnPointerMove(PointerEventArgs e)
    {
        switch (e.Buttons)
        {
            case 1:
                _interactionManager.PointerDrag((int)e.OffsetX, (int)e.OffsetY);
                break;
            case 2:
                _interactionManager.PointerAlternateDrag((int)e.OffsetX, (int)e.OffsetY);
                break;
            default:
                _interactionManager.PointerMove((int)e.OffsetX, (int)e.OffsetY);
                break;
        }
    }

    private void OnPointerUp(PointerEventArgs e)
    {
        _interactionManager.PointerRelease((int)e.OffsetX, (int)e.OffsetY);
    }

    private void OnTouchStart(TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        if (touch is null)
            return;

        if (e.Touches.Length == 2)
        {
            _interactionManager.PointerAlternateClick((int)touch.ClientX, (int)touch.ClientY);
        }
    }

    private void OnTouchMove(TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        if (touch is null)
            return;

        switch (e.Touches.Length)
        {
            case 1:
                _interactionManager.PointerDrag((int)touch.ClientX, (int)touch.ClientY);
                break;
            case 2:
                _interactionManager.PointerAlternateDrag((int)touch.ClientX, (int)touch.ClientY);
                break;
        }
    }

    private void OnMouseWheel(WheelEventArgs e)
    {
        if (e.DeltaY < 0)
        {
            _interactionManager.PointerZoomIn((int)e.ClientX, (int)e.ClientY);
        }
        else
        {
            _interactionManager.PointerZoomOut((int)e.ClientX, (int)e.ClientY);
        }
    }
}
