namespace Signalbox.Engine.Map;

public interface IMap : IEnumerable<Tile>
{
    event EventHandler CollectionChanged;
    Tile Get(int column, int row);
}
