using System.Collections.Concurrent;

namespace Unifico.Core.GameMaster;

/// <summary>
///     A game master that runs games in parallel.
/// </summary>
public class MultiThreadedGameMaster : BaseGameMaster
{
    /// <summary>
    ///     Constructs a new multi-threaded game master.
    /// </summary>
    /// <param name="players">The players of the games.</param>
    /// <param name="numberOfGames">The amount of games to play.</param>
    /// <param name="rules">The rules of the games.</param>
    /// <param name="numberOfThreads">The maximum amount of threads to use.</param>
    public MultiThreadedGameMaster(IEnumerable<Player> players, int numberOfGames, Rules rules, int numberOfThreads) :
        base(players,
            numberOfGames, rules)
    {
        NumberOfThreads = numberOfThreads;
    }

    /// <summary>
    ///     The maximum amount of threads to use.
    /// </summary>
    public int NumberOfThreads { get; }

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