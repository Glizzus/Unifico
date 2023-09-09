// See https://aka.ms/new-console-template for more information

using Unifico.Core;
using Unifico.Core.GameMaster;
using Unifico.Core.Plugins;
using Unifico.Core.Strategy;

namespace Unifico.CLI;

internal static class Program
{
    private static Player SimplePlayer(string name)
    {
        return new Player(name, HandType.ListHand, new SimpleStrategy());
    }

    public static async Task Main(string[] args)
    {
        const string filePath =
            "C:/Users/Glizzus/Projects/unifico-c#/Unifico/Unifico.Plugins/PreferFaceStrategy.cs";
        var plugin = await PluginFactory.Load(filePath);
        if (plugin == null)
        {
            Console.WriteLine("Failed to load plugin");
            return;
        }

        var players = new List<Player>
        {
            SimplePlayer("Cal Crosby"),
            SimplePlayer("Payden Whiting"),
            SimplePlayer("Conner Runion"),
            SimplePlayer("Blake Collins"),
            new("Master Splinter", HandType.ListHand, plugin)
        };

        var master = new SingleThreadedGameMaster(players, 10, new Rules());
        await master.Run();
    }
}