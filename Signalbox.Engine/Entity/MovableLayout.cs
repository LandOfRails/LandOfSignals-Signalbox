using System.Collections;
using System.Collections.Immutable;
using Signalbox.Engine.MainManager;
using Signalbox.Engine.StateManager;
using Signalbox.Engine.Storage;
using Signalbox.Engine.Tracks;
using Signalbox.Engine.Trains;

namespace Signalbox.Engine.Entity;

public class MovableLayout : IMovableLayout, ISignalboxState, ISignalboxStep
{
    private ImmutableList<IMovable> _movables = ImmutableList<IMovable>.Empty;
    private Dictionary<Track, (Train, float)> _lastTrackLeases = new();
    private readonly ILayout _layout;
    private readonly IEntityCollectionSerializer _gameSerializer;
    private readonly Train _reservedTrain;

    public int Count => _movables.Count;

    public IMovable this[int index] => _movables[index];

    public MovableLayout(ILayout layout, IEntityCollectionSerializer gameSerializer)
    {
        _layout = layout;
        _gameSerializer = gameSerializer;
        _reservedTrain = new(0);
    }

    public int IndexOf(IMovable movable)
        => _movables.IndexOf(movable);

    public IEnumerable<(Track, Train, float)> LastTrackLeases => _lastTrackLeases.Select(kvp => (kvp.Key, kvp.Value.Item1, kvp.Value.Item2));

    public void Add(IMovable movable)
        => _movables = _movables.Add(movable);

    public void Remove(IMovable movable)
        => _movables = _movables.Remove(movable);

    public bool Load(ISignalboxStorage storage)
    {
        var entitiesString = storage.Read(nameof(IMovableLayout));
        if (entitiesString is null)
            return false;

        var entities = _gameSerializer.Deserialize(entitiesString);

        var movables = entities.OfType<IMovable>();

        if (movables is null)
            return false;

        _movables = ImmutableList.CreateRange(movables);

        return true;
    }

    public void Save(ISignalboxStorage storage)
    {
        var entities = _gameSerializer.Serialize(_movables);
        storage.Write(nameof(IMovableLayout), entities);
    }

    public void Reset()
        => _movables = _movables.Clear();

    public void Update(long timeSinceLastTick)
    {
    }

    public IMovable? GetAt(int column, int row)
    {
        foreach (var movable in _movables)
        {
            if (movable is not Train train)
            {
                continue;
            }

            if (train.Column == column && train.Row == row)
            {
                return train;
            }
        }
        return null;
    }

    public IEnumerator<IMovable> GetEnumerator()
        => _movables.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _movables.GetEnumerator();
}
