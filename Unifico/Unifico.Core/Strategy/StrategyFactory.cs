using Unifico.Core.Strategy;

public static class StrategyFactory
{
    public static IStrategy Create(string strategy)
    {
        return strategy switch
        {
            "simple" => new SimpleStrategy(),
            _ => throw new NotImplementedException()
        };
    }
}