namespace Unifico.Core.GameMaster;

public class SingleThreadedGameMaster : BaseGameMaster
{
    public SingleThreadedGameMaster(IEnumerable<Player> players, int numberOfGames, Rules rules) : base(players,
        numberOfGames, rules)
    {
    }

    public override async Task Run()
    {
        var winMap =
            new Dictionary<string, int>(Players.Select(player => new KeyValuePair<string, int>(player.Name, 0)));
        for (var i = 0; i < NumberOfGames; i++)
        {
            var clonedPlayers = Players.Select(player => player.Clone());
            var game = new Game(clonedPlayers, Rules)
            {
                Name = $"Game {i}",
                Output = new StreamWriter($"../Game {i}.txt")
            };
            var (winner, entropies) = await game.Play();
            winMap[winner.Name]++;
        }

        foreach (var pair in winMap) Console.WriteLine($"{pair.Key} won {pair.Value} times");
    }
}