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
                    await game.Play();
                }
                finally
                {
                    semaphore.Release();
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
}