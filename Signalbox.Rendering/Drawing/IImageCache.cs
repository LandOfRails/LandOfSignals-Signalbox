using Signalbox.Engine.Utilities;

namespace Signalbox.Rendering.Drawing;

[Transient]
public interface IImageCache : IDisposable
{
    IImage? Get(object key);
    void Set(object key, IImage image);
    bool IsDirty(object key);
    void SetDirty(object key);
    void Clear();
    void SetDirtyAll(IEnumerable<object> keys);
    IDisposable SuspendSetDirtyCalls();
}
