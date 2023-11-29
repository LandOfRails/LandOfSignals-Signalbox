using System.Diagnostics.CodeAnalysis;
using Signalbox.Engine.Entity;
using Signalbox.Engine.Storage;

namespace Signalbox.Engine.Trains;

public class TrainSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('|', 10);
        if (bits.Length != 10)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(Train)))
        {
            return false;
        }

        var i = 1;
        entity = new Train(int.Parse(bits[i++]))
        {
            Angle = float.Parse(bits[i++]),
            Follow = bool.Parse(bits[i++]),
            RelativeLeft = float.Parse(bits[i++]),
            RelativeTop = float.Parse(bits[i++]),
            Stopped = bool.Parse(bits[i++])
        };
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not Train train)
        {
            return false;
        }

        data = $"{nameof(Train)}|{train.Seed}|{train.Angle}|{train.Follow}|{train.RelativeLeft}|{train.RelativeTop}|{train.Stopped}";
        return true;
    }
}
