using Signalbox.Engine.Trains;

namespace Signalbox.Rendering.Trains;

public interface ITrainPainter
{
    TrainPalette GetPalette(Train train);
}
