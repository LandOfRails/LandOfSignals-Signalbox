using System.Collections;
using System.Collections.Immutable;
using Signalbox.Engine.StateManager;
using Signalbox.Engine.Storage;
using Signalbox.Engine.Trains;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Map;

public class Map : IMap, IInitializeAsync, ISignalboxState
{
    private ImmutableDictionary<(int, int), Tile> _map = ImmutableDictionary<(int, int), Tile>.Empty;
    private int _columns;
    private int _rows;
    private Guid UniqueID = Guid.NewGuid();

    public event EventHandler? CollectionChanged;

    void ISignalboxState.Reset()
        => Reset();

    public void Reset()
    {
        Dictionary<(int x, int y), string>? defaultMap = DefaultGenerator.GenerateDefaultMap(_columns, _rows);

        ImmutableDictionary<(int, int), Tile>.Builder builder = ImmutableDictionary.CreateBuilder<(int, int), Tile>();
        foreach ((int x, int y) coord in defaultMap.Keys)
        {
            builder.Add(coord, new()
            {
                Column = coord.x,
                Row = coord.y,
                Content = defaultMap[coord],
            });
        }
        _map = builder.ToImmutable();

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return _map.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Tile Get(int column, int row)
        => _map[(column, row)];

    public bool Load(ISignalboxStorage storage)
    {
        var uniqueID = storage.Read(nameof(Map));

        if (int.TryParse(uniqueID, out var id))
        {
            //TODO implement map loading thingy
            return true;
        }

        return false;
    }

    public void Save(ISignalboxStorage storage)
    {
        storage.Write(nameof(Map), UniqueID.ToString());
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _columns = columns;
        _rows = rows;

        return Task.CompletedTask;
    }
}
