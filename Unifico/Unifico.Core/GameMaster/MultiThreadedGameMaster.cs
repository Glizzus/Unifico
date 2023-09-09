using System.Collections.Concurrent;

namespace Unifico.Core.GameMaster;

public class MultiThreadedGameMaster : BaseGameMaster
{
    public MultiThreadedGameMaster(IEnumerable<Player> players, int numberOfThreads, int numberOfGames, Rules rules) :
        base(players,
            numberOfGames, rules)
    {
        NumberOfThreads = numberOfThreads;
    }

    public int NumberOfThreads { get; init; }

    public override async Task Run()
    {
        var winMap =
            new ConcurrentDictionary<string, int>(
                Players.Select(player => new KeyValuePair<string, int>(player.Name, 0)));
        var semaphore = new SemaphoreSlim(NumberOfThreads);
        var tasks = new List<Task>();
        for (var i = 0; i < NumberOfGames; i++)
        {
            await semaphore.WaitAsync();
            var gameNumber = i;
            var task = Task.Run(async () =>
            {
                try
                {
                    var clonedPlayers = Players.Select(player => player.Clone());
                    var game = new Game(clonedPlayers, Rules)
                    {
                        Name = $"Game {gameNumber}",
                        Output = new StreamWriter($"../Game {gameNumber}.txt")
                    };
                    var (winner, entropies) = await game.Play();
                    winMap.AddOrUpdate(winner.Name, 1, (_, count) => count + 1);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        foreach (var pair in winMap) Console.WriteLine($"{pair.Key} won {pair.Value} times");
    }
}