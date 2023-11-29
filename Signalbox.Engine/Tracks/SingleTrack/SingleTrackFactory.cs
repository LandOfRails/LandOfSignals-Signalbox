using System.Diagnostics.CodeAnalysis;
using Signalbox.Engine.Entity;
using Signalbox.Engine.Map;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Tracks.SingleTrack;

[Order(3)]
public class SingleTrackFactory : IStaticEntityFactory<Track>
{
    private readonly IMap _map;
    private readonly ILayout<Track> _trackLayout;

    public SingleTrackFactory(IMap map, ILayout<Track> trackLayout)
    {
        _map = map;
        _trackLayout = trackLayout;
    }

    public bool TryCreateEntity(int column, int row, int fromColumn, int fromRow, [NotNullWhen(returnValue: true)] out Track? entity)
    {
        entity = null;

        // this factory is never used to override
        if (_trackLayout.TryGet(column, row, out _))
        {
            return false;
        }

        entity = new SingleTrack();
        return true;
    }

    public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
    {
        yield return new SingleTrack { Direction = SingleTrackDirection.Horizontal };
        var neighbours = track.GetAllNeighbors();
        if (neighbours.Up is not null || neighbours.Down is not null)
        {
            yield return new SingleTrack { Direction = SingleTrackDirection.Vertical };
        }
        if (neighbours.Up is not null && neighbours.Left is not null)
        {
            yield return new SingleTrack { Direction = SingleTrackDirection.LeftUp };
        }
        if (neighbours.Up is not null && neighbours.Right is not null)
        {
            yield return new SingleTrack { Direction = SingleTrackDirection.RightUp };
        }
        if (neighbours.Down is not null && neighbours.Left is not null)
        {
            yield return new SingleTrack { Direction = SingleTrackDirection.LeftDown };
        }
        if (neighbours.Down is not null && neighbours.Right is not null)
        {
            yield return new SingleTrack { Direction = SingleTrackDirection.RightDown };
        }
    }
}
