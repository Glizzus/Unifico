using Unifico.Core;
using Unifico.Core.GameMaster;
using Unifico.Core.Hand;
using Unifico.Core.Plugins;
using Unifico.Core.Strategy;
using YamlDotNet.Serialization;

namespace Unifico.CLI;

public class StrategyConfig
{
    public BuiltinStrategy? Builtin { get; set; }
    public string? Path { get; set; }

    public async Task<IStrategy> ToStrategy()
    {
        return (Builtin, Path) switch
        {
            (not null, not null) => throw new Exception("Both builtin and path specified"),
            ({ } builtin, null) => StrategyFactory.Create(builtin),
            (null, { } path) => await PluginFactory.Create(path) ??
                                throw new FileNotFoundException("Could not load plugin"),
            (null, null) => throw new Exception("No strategy specified")
        };
    }
}

public class PlayerConfig
{
    public string Name { get; set; } = null!;

    public HandType HandType { get; set; } = HandType.ListHand;

    public StrategyConfig Strategy { get; set; }

    public async Task<Player> ToPlayer()
    {
        var strategy = await Strategy.ToStrategy();
        return new Player(Name, HandType, strategy);
    }
}

public class Config
{
    public Rules Rules { get; set; } = null!;
    public int NumberOfGames { get; set; }
    public int NumberOfThreads { get; set; } = 1;

    public IEnumerable<PlayerConfig> Players { get; set; } = null!;

    public static async Task<Config> FromYaml(string path)
    {
        var deserializer = new DeserializerBuilder().Build();
        var text = await File.ReadAllTextAsync(path) ?? throw new Exception("Could not read config file");
        var config = deserializer.Deserialize<Config>(text);
        return config;
    }

    public async Task<BaseGameMaster> ToGameMaster()
    {
        return new GameMasterBuilder()
            .WithPlayers(await Task.WhenAll(Players.Select(player => player.ToPlayer())))
            .WithRules(Rules)
            .WithNumberOfGames(NumberOfGames)
            .WithNumberOfThreads(NumberOfThreads)
            .Build();
    }
}