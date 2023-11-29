namespace Signalbox.Instrumentation.Stats;

public class CountStat : IStat
{
    private readonly string _thing;
    public CountStat(string nameOfThingYouAreCounting)
    {
        _thing = nameOfThingYouAreCounting;
    }
    public int Value { get; private set; }
    public void Add() => Value++;
    public void Set(int value) => Value = value;
    public string GetDescription() => Value + ' ' + _thing;
    public bool ShouldShow() => true;
}
