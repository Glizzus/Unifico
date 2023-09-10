using System.Diagnostics;

namespace Unifico.CLI;

internal static class Program
{
    public static async Task Main()
    {
        const string path = "C:/Users/Glizzus/Projects/unifico-c#/Config.yaml";
        var config = await Config.FromYaml(path);
        var master = await config.ToGameMaster();

        var watch = Stopwatch.StartNew();
        await master.Run();
        watch.Stop();
        Console.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds}ms");
    }
}