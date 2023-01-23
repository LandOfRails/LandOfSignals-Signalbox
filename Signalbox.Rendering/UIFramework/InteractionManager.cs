using Signalbox.Engine.MainManager;
using Signalbox.Engine.Tools;
using Signalbox.Rendering.Signalbox;

namespace Signalbox.Rendering.UIFramework;

public class InteractionManager : IInteractionManager
{
    private readonly IEnumerable<IInteractionHandler> _handler;
    private readonly ISignalbox _signalbox;
    private readonly IPixelMapper _pixelMapper;
    private readonly ISignalboxManager _signalboxManager;
    private readonly IAlternateDragTool _alternateDragTool;
    private IInteractionHandler? _capturedHandler;
    private ITool? _capturedTool;
    private bool _hasDragged;
    private int _lastToolColumn;
    private int _lastToolRow;

    public InteractionManager(IEnumerable<IInteractionHandler> handlers, ISignalbox signalbox, IPixelMapper pixelMapper, ISignalboxManager signalboxManager, IAlternateDragTool alternateDragTool)
    {
        _handler = handlers.Reverse().ToArray();
        _signalbox = signalbox;
        _pixelMapper = pixelMapper;
        _signalboxManager = signalboxManager;
        _alternateDragTool = alternateDragTool;
    }

    public bool PointerClick(int x, int y)
        => HandleInteraction(x, y, PointerAction.Click);

    public bool PointerMove(int x, int y)
        => HandleInteraction(x, y, PointerAction.Move);

    public bool PointerDrag(int x, int y)
        => HandleInteraction(x, y, PointerAction.Drag);

    public bool PointerAlternateClick(int x, int y)
        => HandleInteraction(x, y, PointerAction.AlternateClick);

    public bool PointerAlternateDrag(int x, int y)
        => HandleInteraction(x, y, PointerAction.AlternateDrag);

    public bool PointerZoomIn(int x, int y)
        => HandleInteraction(x, y, PointerAction.ZoomIn);

    public bool PointerZoomOut(int x, int y)
        => HandleInteraction(x, y, PointerAction.ZoomOut);

    public bool PointerRelease(int x, int y)
    {
        (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

        if (_capturedHandler is null &&
            !_hasDragged &&
            _signalboxManager.CurrentTool is not null &&
            _signalboxManager.CurrentTool.IsValid(column, row))
        {
            _signalboxManager.CurrentTool.Execute(column, row, new());
        }

        _hasDragged = false;
        _lastToolColumn = -1;
        _lastToolRow = -1;
        if (_capturedHandler != null || _capturedTool != null)
        {
            _capturedTool = null;
            _capturedHandler = null;
            return true;
        }
        return false;
    }

    private bool HandleInteraction(int x, int y, PointerAction action)
    {
        (int width, int height) = _signalbox.GetScreenSize();

        if (_capturedHandler != null)
        {
            _capturedHandler.HandlePointerAction(x, y, width, height, action);
            return true;
        }
        else if (_capturedTool != null)
        {
            ExecuteTool(_capturedTool, x, y, action);
            return true;
        }

        switch (action)
        {
            case PointerAction.Click:
            {
                bool preHandled = false;
                foreach (var handler in _handler.Where(handler => handler.PreHandleNextClick && action is PointerAction.Click))
                {
                    _capturedHandler = handler;
                    preHandled |= handler.HandlePointerAction(x, y, width, height, action);
                }
                if (preHandled)
                {
                    return true;
                }

                break;
            }
            case PointerAction.AlternateClick:
                _alternateDragTool.StartDrag(x, y);
                return true;
            case PointerAction.AlternateDrag:
                _hasDragged = true;
                _alternateDragTool.ContinueDrag(x, y);
                return true;
        }

        foreach (var handler in _handler.Where(handler => handler.HandlePointerAction(x, y, width, height, action)))
        {
            if (action is PointerAction.Click or PointerAction.Drag)
            {
                _capturedHandler = handler;
            }
            return true;
        }

        if (_signalboxManager.CurrentTool is null) return false;
        if (!ExecuteTool(_signalboxManager.CurrentTool, x, y, action)) return false;
        if (action is PointerAction.Click or PointerAction.Drag)
        {
            _capturedTool = _signalboxManager.CurrentTool;
        }

        return false;
    }

    private bool ExecuteTool(ITool tool, int x, int y, PointerAction action)
    {
        (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

        var inSameCell = (column == _lastToolColumn && row == _lastToolRow);

        _hasDragged = action switch
        {
            PointerAction.Click => false,
            PointerAction.Drag => true,
            _ => _hasDragged
        };

        try
        {
            if (!inSameCell && action is PointerAction.Drag && tool.IsValid(column, row))
            {
                tool.Execute(column, row, new(
                    fromColumn: _lastToolColumn,
                    fromRow: _lastToolRow));
                return true;
            }
            else if (tool is IDraggableTool draggableTool)
            {
                switch (action)
                {
                    case PointerAction.Click:
                        draggableTool.StartDrag(x, y);
                        return true;
                    case PointerAction.Drag:
                        draggableTool.ContinueDrag(x, y);
                        return true;
                }
            }
        }
        finally
        {
            if (_hasDragged)
            {
                _lastToolColumn = column;
                _lastToolRow = row;
            }
        }
        return false;
    }
}
