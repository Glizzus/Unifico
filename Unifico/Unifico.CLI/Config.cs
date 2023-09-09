using Unifico.Core;
using Unifico.Core.GameMaster;
using Unifico.Core.Hand;
using Unifico.Core.Plugins;
using Unifico.Core.Strategy;
using YamlDotNet.Serialization;

namespace Unifico.CLI;

public class PlayerConfig
{
    public string Name { get; set; } = null!;

    public string? Strategy { get; set; }
    public string? StrategyPath { get; set; }

    public async Task<Player> ToPlayer()
    {
        if (StrategyPath != null)
        {
            var plugin = await PluginFactory.Create(StrategyPath) ??
                         throw new FileNotFoundException("Could not load plugin");
            return new Player(Name, HandType.ListHand, plugin);
        }

        if (Strategy != null)
            return new Player(Name, HandType.ListHand, StrategyFactory.Create(Strategy));
        throw new Exception("No strategy specified");
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