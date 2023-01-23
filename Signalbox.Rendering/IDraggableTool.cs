﻿using Signalbox.Engine.Tools;

namespace Signalbox.Rendering;

public interface IDraggableTool : ITool
{
    void StartDrag(int x, int y);
    void ContinueDrag(int x, int y);
}
