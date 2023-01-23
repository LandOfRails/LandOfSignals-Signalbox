using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Trains;

public interface IMovable : IEntity
{
    Guid UniqueID { get; }
    float Angle { get; }
    float RelativeLeft { get; }
    float RelativeTop { get; }
    bool Follow { get; }

    void SetAngle(float angle);
}
