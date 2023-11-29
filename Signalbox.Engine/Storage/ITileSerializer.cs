using Signalbox.Engine.Map;

namespace Signalbox.Engine.Storage;

public interface ITileSerializer
{
    IEnumerable<Tile> Deserialize(string[] lines);
    string Serialize(IEnumerable<Tile> contentList);
}
