using System.ComponentModel;
using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Trains;

public class Train : IMovable, INotifyPropertyChanged, ISeeded
{
    private bool _collisionAhead;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Train(int seed)
    {
        Seed = seed;
        UniqueID = Guid.NewGuid();
        Name = TrainNames.GetName(seed);
        RelativeLeft = 0.5f;
        RelativeTop = 0.5f;
    }

    private Train(Train other)
    {
        Seed = other.Seed;
        UniqueID = other.UniqueID;
        Column = other.Column;
        Name = other.Name;
        Row = other.Row;
        Angle = other.Angle;
        RelativeLeft = other.RelativeLeft;
        RelativeTop = other.RelativeTop;
    }

    public virtual Guid UniqueID { get; }

    public int Column { get; set; }
    public int Row { get; set; }
    public float Angle { get; set; }
    public float RelativeLeft { get; set; }
    public float RelativeTop { get; set; }

    public string Name { get; set; }
    public int Seed { get; }
    public virtual bool Stopped { get; set; }

    public bool Follow { get; set; }

    public void SetAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle > 360) angle -= 360;
        Angle = angle;
    }

    public Train Clone()
    {
        return new(this);
    }

    public void Start() => Stopped = false;

    public void Stop() => Stopped = true;

    internal void Pause() => _collisionAhead = true;

    internal void Resume() => _collisionAhead = false;

    public override string ToString() => $"Train {UniqueID} [Column: {Column} | Row: {Row} | Left: {RelativeLeft} | Top: {RelativeTop} | Angle: {Angle}]";

    internal TrainPosition GetPosition() => new(Column, Row, RelativeLeft, RelativeTop, Angle, 0);

    public void ApplyStep(TrainPosition newPosition)
    {
        Column = newPosition.Column;
        Row = newPosition.Row;
        Angle = newPosition.Angle;
        RelativeLeft = newPosition.RelativeLeft;
        RelativeTop = newPosition.RelativeTop;
    }
}
