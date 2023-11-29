﻿using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.UIFramework;

public class MultiButton : ButtonBase
{
    private readonly int _buttonSize;
    private readonly ButtonBase[] _buttons;

    public MultiButton(int buttonSize, params ButtonBase[] buttons)
    {
        _buttonSize = buttonSize;
        _buttons = buttons;

        Height = _buttonSize;
        Width = _buttonSize * _buttons.Length;
    }

    public override int GetMinimumWidth(ICanvas canvas) => _buttonSize * _buttons.Length;

    public override bool HandleMouseAction(int x, int y, PointerAction action)
    {
        if (action is PointerAction.Click or PointerAction.Move)
        {
            foreach (var button in _buttons)
            {
                if (button.HandleMouseAction(x, y, action))
                {
                    return true;
                }

                x -= button.Width;
            }
        }
        return false;
    }

    public override void Render(ICanvas canvas)
    {
        using (canvas.Scope())
        {
            foreach (var button in _buttons)
            {
                button.Width = _buttonSize;
                button.Height = _buttonSize;

                button.Render(canvas);
                canvas.Translate(_buttonSize, 0);
            }
        }
    }

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        throw new InvalidOperationException("This should never be hit");
    }
}
