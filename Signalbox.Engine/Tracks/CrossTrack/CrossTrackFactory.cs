using System.Diagnostics.CodeAnalysis;
using Signalbox.Engine.Entity;
using Signalbox.Engine.Map;
using Signalbox.Engine.Tracks.SingleTrack;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Tracks.CrossTrack;

[Order(1)]
public class CrossTrackFactory : IStaticEntityFactory<Track>
{
    private readonly IMap _map;
    private readonly ILayout _layout;

    public CrossTrackFactory(IMap map, ILayout layout)
    {
        _map = map;
        _layout = layout;
    }

    public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
    {
        var neighbours = track.GetAllNeighbors();
        if (neighbours.Count == 4)
        {
            yield return new CrossTrack();
        }
    }

    public bool TryCreateEntity(int column, int row, int fromColumn, int fromRow, [NotNullWhen(true)] out Track? entity)
    {
        var neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column, row, emptyIsConsideredConnected: true, ignoreCurrent: fromColumn != 0);

        // if they click and its the perfect spot for a cross track, just do it
        if (neighbours.Count == 4)
        {
            entity = new CrossTrack();
            return true;
        }

        if (fromColumn != 0)
        {
            // if they're dragging, we're looking for them to complete an intersection
            neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column - 1, row);
            if (neighbours.Count == 3 && neighbours.Right is null)
            {
                entity = new SingleTrack.SingleTrack { Direction = SingleTrackDirection.Horizontal };
                _layout.Set(column - 1, row, new CrossTrack());
                return true;
            }

            neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column, row - 1);
            if (neighbours.Count == 3 && neighbours.Down is null)
            {
                entity = new SingleTrack.SingleTrack { Direction = SingleTrackDirection.Vertical };
                _layout.Set(column, row - 1, new CrossTrack());
                return true;
            }

            neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column + 1, row);
            if (neighbours.Count == 3 && neighbours.Left is null)
            {
                entity = new SingleTrack.SingleTrack { Direction = SingleTrackDirection.Horizontal };
                _layout.Set(column + 1, row, new CrossTrack());
                return true;
            }

            neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column, row + 1);
            if (neighbours.Count == 3 && neighbours.Up is null)
            {
                entity = new SingleTrack.SingleTrack { Direction = SingleTrackDirection.Vertical };
                _layout.Set(column, row + 1, new CrossTrack());
                return true;
            }
        }

        entity = null;
        return false;
    }
}
