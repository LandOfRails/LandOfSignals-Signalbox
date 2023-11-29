namespace Signalbox.Engine.Trains;

internal static class TrainNames
{
    public static string GetName(int seed) => s_names[Math.Abs(seed) % s_names.Length];

    private static readonly string[] s_names = {
            "Test",
    };
}
