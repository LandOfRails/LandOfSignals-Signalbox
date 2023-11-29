namespace Signalbox.Engine.Trains;

public class TrainPosition
{
    public float RelativeLeft { get; set; }
    public float RelativeTop { get; set; }
    public float Angle { get; set; }
    public float Distance { get; set; }
    public int Column { get; internal set; }
    public int Row { get; internal set; }

    public TrainPosition(float relativeLeft, float relativeTop, float angle, float distance)
    {
        RelativeLeft = relativeLeft;
        RelativeTop = relativeTop;
        Angle = angle;
        Distance = distance;
    }

    public TrainPosition(int column, int row, float relativeLeft, float relativeTop, float angle, float distance)
        : this(relativeLeft, relativeTop, angle, distance)
    {
        Column = column;
        Row = row;
    }

    internal TrainPosition Clone() => new(Column, Row, RelativeLeft, RelativeTop, Angle, Distance);
}
