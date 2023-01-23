using System.Text;

namespace Signalbox.Engine.Storage;

public class UTF8TileSerializer : ITileSerializer
{
    public IEnumerable<Map.Tile> Deserialize(string[] lines)
    {
        var contentList = new List<Map.Tile>();

        for (int r = 0; r < lines.Length; r++)
        {
            string? line = lines[r];
            string[]? heights = line.Split(',');
            for (int c = 0; c < heights.Length; c++)
            {

                if (!int.TryParse(heights[c], out int height))
                {
                    throw new("Invalid height read from file");
                }

                contentList.Add(new Map.Tile
                {
                    Row = r,
                    Column = c,
                });
            }
        }

        return contentList;
    }

    public string Serialize(IEnumerable<Map.Tile> contentList)
    {
        if (!contentList.Any()) return string.Empty;

        var dict = contentList.ToDictionary(t => (t.Column, t.Row), t => t.Content);

        var sb = new StringBuilder();

        int maxColumn = contentList.Max(t => t.Column);
        int maxRow = contentList.Max(t => t.Row);

        for (int r = 0; r <= maxRow; r++)
        {
            var contents = new List<string>();
            for (int c = 0; c <= maxColumn; c++)
            {
                if (!dict.TryGetValue((c, r), out string content))
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
