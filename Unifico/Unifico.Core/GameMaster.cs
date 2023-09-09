using System.Net.Mime;

namespace Unifico.Core;

public class GameMaster
{
    public IEnumerable<Player> Players { get; init; }
    public int NumberOfThreads { get; init; }
    public int NumberOfGames { get; init; }
    public Rules Rules { get; init; }
    
    public GameMaster(IEnumerable<Player> players, int numberOfThreads, int numberOfGames, Rules rules)
    {
        Players = players;
        NumberOfThreads = numberOfThreads;
        NumberOfGames = numberOfGames;
        Rules = rules;
    }
    
    public async Task Run()
    {
        var semaphore = new SemaphoreSlim(NumberOfThreads);
        var tasks = new List<Task>();
        for (var i = 0; i < NumberOfGames; i++)
        {
            await semaphore.WaitAsync();
            var gameNumber = i;
            var task = Task.Run(() =>
            {
                try
                {
                    var clonedPlayers = Players.Select(player => player.Clone());
                    var game = new Game(clonedPlayers, Rules)
                    {
                        Name = $"Game {gameNumber}",
                        Output = new StreamWriter($"../Game {gameNumber}.txt")
                    };
                    game.Play();
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