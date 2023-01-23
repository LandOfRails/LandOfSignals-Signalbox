namespace Signalbox.Engine.Tools;

public struct ExecuteInfo
{
    public readonly int FromColumn;
    public readonly int FromRow;

    public ExecuteInfo(int fromColumn, int fromRow)
    {
        FromColumn = fromColumn;
        FromRow = fromRow;
    }
}
