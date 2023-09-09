namespace Unifico.Core.GameMaster;

/// <summary>
///     The base class for all game masters.
///     A game master is responsible for running many games.
/// </summary>
public abstract class BaseGameMaster
{
    /// <summary>
    ///     Constructs a new game master.
    /// </summary>
    /// <param name="players">The players to be used in all of the games.</param>
    /// <param name="numberOfGames">The number of games to play.</param>
    /// <param name="rules">The rules for the games.</param>
    protected BaseGameMaster(IEnumerable<Player> players, int numberOfGames, Rules rules)
    {
        Players = players;
        NumberOfGames = numberOfGames;
        Rules = rules;
    }

    /// <summary>
    ///     The players to be used in all of the games.
    /// </summary>
    protected IEnumerable<Player> Players { get; }

    /// <summary>
    ///     The number of games to play.
    /// </summary>
    protected int NumberOfGames { get; }

    /// <summary>
    ///     The rules for the games.
    /// </summary>
    protected Rules Rules { get; }

    public abstract Task Run();
}