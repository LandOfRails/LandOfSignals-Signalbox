using System.Diagnostics.CodeAnalysis;
using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Storage;

public interface IEntitySerializer
{
    bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data);

    bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity);
}
