namespace Unifico.CLI;

internal static class Program
{
    public static async Task Main()
    {
        const string path = "C:/Users/Glizzus/Projects/unifico-c#/Config.yaml";
        var config = await Config.FromYaml(path);
        var master = await config.ToGameMaster();
        await master.Run();
    }
}