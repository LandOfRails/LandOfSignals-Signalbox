using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Storage;

public interface IEntityCollectionSerializer
{
    IEnumerable<IEntity> Deserialize(string lines);
    string Serialize(IEnumerable<IEntity> tracks);
}
