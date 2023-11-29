using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Tracks;

public class TrackNeighbors
{
    public Track? Left { get; }
    public Track? Up { get; }
    public Track? Right { get; }
    public Track? Down { get; }

    public IEnumerable<Track> All
    {
        get
        {
            if (Up is not null) yield return Up;
            if (Left is not null) yield return Left;
            if (Right is not null) yield return Right;
            if (Down is not null) yield return Down;
        }
    }

    public int Count => (Up == null ? 0 : 1) +
            (Down == null ? 0 : 1) +
            (Right == null ? 0 : 1) +
            (Left == null ? 0 : 1);

    public bool Contains(Track otherTrack)
    {
        return otherTrack != null &&
            (Left == otherTrack ||
            Right == otherTrack ||
            Up == otherTrack ||
            Down == otherTrack);
    }

    public TrackNeighbors(Track? left, Track? up, Track? right, Track? down)
    {
        Left = left;
        Up = up;
        Right = right;
        Down = down;
    }

    public static TrackNeighbors GetConnectedNeighbours(ILayout trackLayout, int column, int row, bool emptyIsConsideredConnected = false, bool ignoreCurrent = false)
    {
        trackLayout.TryGet(column, row, out Track? current);

        bool isConnectedLeft;
        bool isConnectedUp;
        bool isConnectedRight;
        bool isConnectedDown;
        if (ignoreCurrent || current is null)
        {
            isConnectedLeft = emptyIsConsideredConnected;
            isConnectedUp = emptyIsConsideredConnected;
            isConnectedRight = emptyIsConsideredConnected;
            isConnectedDown = emptyIsConsideredConnected;
        }
        else
        {
            isConnectedLeft = current.IsConnectedLeft();
            isConnectedUp = current.IsConnectedUp();
            isConnectedRight = current.IsConnectedRight();
            isConnectedDown = current.IsConnectedDown();
        }

        return new(
            trackLayout.TryGet(column - 1, row, out Track? left) && isConnectedLeft && left.IsConnectedRight() ? left : null,
            trackLayout.TryGet(column, row - 1, out Track? up) && isConnectedUp && up.IsConnectedDown() ? up : null,
            trackLayout.TryGet(column + 1, row, out Track? right) && isConnectedRight && right.IsConnectedLeft() ? right : null,
            trackLayout.TryGet(column, row + 1, out Track? down) && isConnectedDown && down.IsConnectedUp() ? down : null
            );
    }

    public static TrackNeighbors GetAllNeighbours(ILayout trackLayout, int column, int row)
    {
        return new(
            trackLayout.TryGet(column - 1, row, out Track? left) ? left : null,
            trackLayout.TryGet(column, row - 1, out Track? up) ? up : null,
            trackLayout.TryGet(column + 1, row, out Track? right) ? right : null,
            trackLayout.TryGet(column, row + 1, out Track? down) ? down : null
            );
    }
}
