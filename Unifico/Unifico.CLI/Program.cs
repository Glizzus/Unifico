// See https://aka.ms/new-console-template for more information

using Unifico.Core.GameMaster;

namespace Unifico.CLI;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        const string path = "C:/Users/Glizzus/Projects/unifico-c#/Config.yaml";
        var config = await Config.FromYaml(path);
        var players = await Task.WhenAll(config.Players.Select(player => player.ToPlayer()));
        var master = new MultiThreadedGameMaster(players, config.NumberOfThreads, config.NumberOfGames, config.Rules);
        await master.Run();
    }
}