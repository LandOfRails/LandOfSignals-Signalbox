using System.Diagnostics.CodeAnalysis;
using Signalbox.Engine.Entity;
using Signalbox.Engine.Map;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Tracks.SingleTrack;

[Order(4)]
public class SignalFactory : IStaticEntityFactory<Track>
{
    private readonly IMap _map;

    public SignalFactory(IMap map)
    {
        _map = map;
    }

    public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
    {
        var neighbours = track.GetAllNeighbors();
        if (neighbours.Left is not null || neighbours.Right is not null)
        {
            yield return new Signal() { Direction = SingleTrackDirection.Horizontal, SignalState = SignalState.Go };
            yield return new Signal() { Direction = SingleTrackDirection.Horizontal, SignalState = SignalState.TemporaryStop };
            yield return new Signal() { Direction = SingleTrackDirection.Horizontal, SignalState = SignalState.Stop };
        }
        if (neighbours.Up is not null || neighbours.Down is not null)
        {
            yield return new Signal() { Direction = SingleTrackDirection.Vertical, SignalState = SignalState.Go };
            yield return new Signal() { Direction = SingleTrackDirection.Vertical, SignalState = SignalState.TemporaryStop };
            yield return new Signal() { Direction = SingleTrackDirection.Vertical, SignalState = SignalState.Stop };
        }
    }

    public bool TryCreateEntity(int column, int row, int fromColumn, int fromRow, [NotNullWhen(returnValue: true)] out Track? entity)
    {
        // never automatically draw a signal

        entity = null;
        return false;
    }
}
