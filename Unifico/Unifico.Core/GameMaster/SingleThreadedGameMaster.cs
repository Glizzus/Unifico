namespace Unifico.Core.GameMaster;

public class SingleThreadedGameMaster : BaseGameMaster
{
    public SingleThreadedGameMaster(IEnumerable<Player> players, int numberOfGames, Rules rules) : base(players,
        numberOfGames, rules)
    {
    }

    public override async Task Run()
    {
        for (var i = 0; i < NumberOfGames; i++)
        {
            var clonedPlayers = Players.Select(player => player.Clone());
            var game = new Game(clonedPlayers, Rules)
            {
                Name = $"Game {i}",
                Output = new StreamWriter($"../Game {i}.txt")
            };
            await game.Play();
        }
    }
}