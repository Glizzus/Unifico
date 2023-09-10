using Unifico.Core.Plugins;

namespace Unifico.Core.Strategy;

public static class StrategyFactory
{
    public static IStrategy Create(BuiltinStrategy strategy)
    {
        return strategy switch
        {
            BuiltinStrategy.Simple => new SimpleStrategy(),
            _ => throw new NotImplementedException()
        };
    }

    public static async Task<IStrategy?> Create(string path)
    {
        return await PluginFactory.Create(path);
    }
}