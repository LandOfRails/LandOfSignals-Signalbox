using Microsoft.AspNetCore.Components.Web;
using Signalbox.Instrumentation;
using Signalbox.Instrumentation.Stats;
using Signalbox.Rendering.Signalbox;
using Signalbox.Rendering.Skia;
using Signalbox.Rendering.UIFramework;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace LandOfSignals_Signalbox;

public partial class Game
{
    private ISignalbox _signalbox = null!;
    private IInteractionManager _interactionManager = null!;

    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SkiaSharp-OnPaintSurfaceFPS");
    private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-GameRender");

    protected override async Task OnInitializedAsync()
    {
        _signalbox = DI.ServiceLocator.GetService<ISignalbox>();
        _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

        await _signalbox.InitializeAsync(200, 200);
    }

    private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
    {
        using (_renderTime.Measure())
        {
            _signalbox.SetSize(e.Info.Width, e.Info.Height);
            if (e.Surface.Context is GRContext context && context != null)
            {
                // Set the context so all rendering happens in the same place
                _signalbox.SetContext(new SKContextWrapper(context));
            }
            _signalbox.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }

        _fps.Update();
    }

    private void OnPointerDown(PointerEventArgs e)
    {
        if (e.Buttons == 1)
        {
            _interactionManager.PointerClick((int)e.OffsetX, (int)e.OffsetY);
        }
        else if (e.Buttons == 2)
        {
            _interactionManager.PointerAlternateClick((int)e.OffsetX, (int)e.OffsetY);
        }
    }

    private void OnPointerMove(PointerEventArgs e)
    {
        if (e.Buttons == 1)
        {
            _interactionManager.PointerDrag((int)e.OffsetX, (int)e.OffsetY);
        }
        else if (e.Buttons == 2)
        {
            _interactionManager.PointerAlternateDrag((int)e.OffsetX, (int)e.OffsetY);
        }
        else
        {
            _interactionManager.PointerMove((int)e.OffsetX, (int)e.OffsetY);
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

        if (e.Touches.Length == 1)
        {
            _interactionManager.PointerDrag((int)touch.ClientX, (int)touch.ClientY);
        }
        else if (e.Touches.Length == 2)
        {
            _interactionManager.PointerAlternateDrag((int)touch.ClientX, (int)touch.ClientY);
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
