namespace Signalbox.Engine.Map;

internal static class DefaultGenerator
{
    public static Dictionary<(int x, int y), string> GenerateDefaultMap(int width, int height)
    {
        var points = new Dictionary<(int x, int y), string>();
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                points.Add((i, j), string.Empty);
            }
        }
        return points;
    }
}
