namespace Unifico.Core.GameMaster;

public abstract class BaseGameMaster
{
    protected BaseGameMaster(IEnumerable<Player> players, int numberOfGames, Rules rules)
    {
        Players = players;
        NumberOfGames = numberOfGames;
        Rules = rules;
    }

    protected IEnumerable<Player> Players { get; }
    protected int NumberOfGames { get; }
    protected Rules Rules { get; }

    public abstract Task Run();
}