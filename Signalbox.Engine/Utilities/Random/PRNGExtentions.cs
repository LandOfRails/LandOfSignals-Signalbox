using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Utilities.Random;

public static class PRNGExtentions
{
    public static BasicPRNG GetPRNG(this ISeeded item) => new(item.Seed);
}
