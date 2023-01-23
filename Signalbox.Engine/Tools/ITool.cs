﻿namespace Signalbox.Engine.Tools;

public interface ITool
{
    ToolMode Mode { get; }

    string Name { get; }

    void Execute(int column, int row, ExecuteInfo info);

    bool IsValid(int column, int row);
}
