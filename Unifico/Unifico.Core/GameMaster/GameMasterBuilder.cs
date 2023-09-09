namespace Unifico.Core.GameMaster;

/// <summary>
///     Builds an instance of <see cref="GameMaster" />.
/// </summary>
public class GameMasterBuilder
{
    private int _numberOfGames;
    private int _numberOfThreads = 1;
    private IEnumerable<Player>? _players;
    private Rules _rules = Rules.Classic;

    public GameMasterBuilder WithPlayers(IEnumerable<Player> players)
    {
        _players = players;
        return this;
    }

    public GameMasterBuilder WithNumberOfGames(int numberOfGames)
    {
        _numberOfGames = numberOfGames;
        return this;
    }

    public GameMasterBuilder WithRules(Rules rules)
    {
        _rules = rules;
        return this;
    }

    public GameMasterBuilder WithNumberOfThreads(int numberOfThreads)
    {
        _numberOfThreads = numberOfThreads;
        return this;
    }

    public BaseGameMaster Build()
    {
        if (_players == null || _players.Count() < 2) throw new ArgumentException("There must be at least two players");
        if (_numberOfGames == 0) throw new ArgumentException("There must be at least one game");
        if (_numberOfThreads == 1) return new SingleThreadedGameMaster(_players, _numberOfGames, _rules);
        return new MultiThreadedGameMaster(_players, _numberOfGames, _rules, _numberOfThreads);
    }
}