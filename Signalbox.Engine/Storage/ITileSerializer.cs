namespace Signalbox.Engine.Storage;

public interface ITileSerializer
{
    IEnumerable<Map.Tile> Deserialize(string[] lines);
    string Serialize(IEnumerable<Map.Tile> contentList);
}
