﻿using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.UIFramework;

public abstract partial class ButtonPanelBase : PanelBase
{
    private const int ButtonGap = 10;
    private const int ButtonLeft = 5;
    private int _buttonWidth = 60;

    protected abstract IEnumerable<ButtonBase> GetButtons();

    protected override bool HandlePointerAction(int x, int y, PointerAction action)
    {
        if (action is PointerAction.Click or PointerAction.Move)
        {
            x -= ButtonLeft;
            foreach (var button in GetButtons())
            {
                if (button.HandleMouseAction(x, y, action))
                {
                    OnChanged();
                    return true;
                }

                y -= button.Height + ButtonGap;
            }
        }
        return true;
    }

    protected override void Render(ICanvas canvas)
    {
        canvas.Translate(ButtonLeft, 0);

        foreach (var button in GetButtons().ToArray())
        {
            button.Width = _buttonWidth;

            using (canvas.Scope())
            {
                button.Render(canvas);
            }

            canvas.Translate(0, button.Height + ButtonGap);
        }
    }

    protected override void PreRender(ICanvas canvas)
    {
        _buttonWidth = 0;
        base.InnerHeight = 0;
        foreach (var button in GetButtons().ToArray())
        {
            _buttonWidth = Math.Max(_buttonWidth, button.GetMinimumWidth(canvas));
            base.InnerHeight += button.Height + ButtonGap;
        }

        base.InnerWidth = _buttonWidth + 10;
        base.InnerHeight = base.InnerHeight - ButtonGap;
    }
}
