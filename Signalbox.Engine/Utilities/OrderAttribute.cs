namespace Signalbox.Engine.Utilities;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OrderAttribute : Attribute
{
    public int Order { get; set; }

    public OrderAttribute(int order)
    {
        Order = order;
    }
}
