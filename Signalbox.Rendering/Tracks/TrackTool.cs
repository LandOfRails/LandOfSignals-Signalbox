﻿using Signalbox.Engine.Entity;
using Signalbox.Engine.Tools;
using Signalbox.Engine.Tracks;
using Signalbox.Engine.Utilities;

namespace Signalbox.Rendering.Tracks;

[Order(10)]
public class TrackTool : ITool
{
    private readonly ILayout<Track> _entityCollection;
    private readonly IEnumerable<IStaticEntityFactory<Track>> _entityFactories;

    public ToolMode Mode => ToolMode.Build;
    public string Name => "Track";

    public TrackTool(ILayout<Track> trackLayout, IEnumerable<IStaticEntityFactory<Track>> entityFactories)
    {
        _entityCollection = trackLayout;
        _entityFactories = entityFactories;
    }

    public void Execute(int column, int row, ExecuteInfo info)
    {
        if (info.FromColumn == 0 && _entityCollection.TryGet(column, row, out var track))
        {
            _entityCollection.SelectedEntity = track;
        }
        else
        {
            _entityCollection.Add(column, row, _entityFactories, info.FromColumn, info.FromRow);
            _entityCollection.SelectedEntity = null;
        }
    }

    public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row);
}
