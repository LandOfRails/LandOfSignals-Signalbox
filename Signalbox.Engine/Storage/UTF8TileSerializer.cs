using System.Text;
using Signalbox.Engine.Map;

namespace Signalbox.Engine.Storage;

public class UTF8TileSerializer : ITileSerializer
{
    public IEnumerable<Tile> Deserialize(string[] lines)
    {
        var contentList = new List<Tile>();

        for (var r = 0; r < lines.Length; r++)
        {
            var line = lines[r];
            string[]? heights = line.Split(',');
            for (var c = 0; c < heights.Length; c++)
            {

                if (!int.TryParse(heights[c], out var height))
                {
                    throw new("Invalid height read from file");
                }

                contentList.Add(new Tile
                {
                    Row = r,
                    Column = c,
                });
            }
        }

        return contentList;
    }

    public string Serialize(IEnumerable<Tile> contentList)
    {
        if (!contentList.Any()) return string.Empty;

        var dict = contentList.ToDictionary(t => (t.Column, t.Row), t => t.Content);

        var sb = new StringBuilder();

        var maxColumn = contentList.Max(t => t.Column);
        var maxRow = contentList.Max(t => t.Row);

        for (var r = 0; r <= maxRow; r++)
        {
            var contents = new List<string>();
            for (var c = 0; c <= maxColumn; c++)
            {
                if (!dict.TryGetValue((c, r), out var content))
                {
                    content = string.Empty;
                }
                contents.Add(content);
            }

            sb.AppendLine(string.Join(',', contents.Select(h => h.ToString())));
        }

        return sb.ToString();
    }
}
