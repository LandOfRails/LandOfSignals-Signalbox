using System.ComponentModel;
using Signalbox.Engine.Entity;

namespace Signalbox.Engine.Trains;

public class Train : IMovable, INotifyPropertyChanged, ISeeded
{
    private bool _collisionAhead;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Train(int seed)
    {
        this.Seed = seed;
        this.UniqueID = Guid.NewGuid();
        this.Name = TrainNames.GetName(seed);
        this.RelativeLeft = 0.5f;
        this.RelativeTop = 0.5f;
    }

    private Train(Train other)
    {
        this.Seed = other.Seed;
        this.UniqueID = other.UniqueID;
        this.Column = other.Column;
        this.Name = other.Name;
        this.Row = other.Row;
        this.Angle = other.Angle;
        this.RelativeLeft = other.RelativeLeft;
        this.RelativeTop = other.RelativeTop;
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
        this.Angle = angle;
    }

    public Train Clone()
    {
        return new(this);
    }

    public void Start() => this.Stopped = false;

    public void Stop() => this.Stopped = true;

    internal void Pause() => _collisionAhead = true;

    internal void Resume() => _collisionAhead = false;

    public override string ToString() => $"Train {this.UniqueID} [Column: {this.Column} | Row: {this.Row} | Left: {this.RelativeLeft} | Top: {this.RelativeTop} | Angle: {this.Angle}]";

    internal TrainPosition GetPosition() => new(this.Column, this.Row, this.RelativeLeft, this.RelativeTop, this.Angle, 0);

    public void ApplyStep(TrainPosition newPosition)
    {
        this.Column = newPosition.Column;
        this.Row = newPosition.Row;
        this.Angle = newPosition.Angle;
        this.RelativeLeft = newPosition.RelativeLeft;
        this.RelativeTop = newPosition.RelativeTop;
    }
}
