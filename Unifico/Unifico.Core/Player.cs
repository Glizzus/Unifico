using Unifico.Core.Cards;
using Unifico.Core.Hand;
using Unifico.Core.Strategy;

namespace Unifico.Core;

/// <summary>
///     A player in the UNO game.
///     The player holds a hand and has a strategy for playing cards.
/// </summary>
public class Player
{
    // We use an enum so that we can defer the creation of the hand until we need it.
    // This means that we can clone the player without having to clone the hand.
    private readonly HandType _handType;

    public Player(string name, HandType handType, IStrategy strategy)
    {
        Name = name;
        _handType = handType;
        Hand = HandFactory.Create(handType);
        Strategy = strategy;
    }

    public string Name { get; init; }
    public IHand Hand { get; init; }
    private IStrategy Strategy { get; }

    /// <summary>
    ///     Indicates whether or not the player has won the game.
    /// </summary>
    public bool HasWon => !Hand.Any();

    /// <summary>
    ///     Plays a card from the player's hand.
    /// </summary>
    /// <param name="topCard">The card that the player is playing against.</param>
    /// <param name="isStack">Whether or not there is a stack in effect.</param>
    /// <param name="stackJudge">The StackJudge to be passed to the strategy to ensure validity of the strategy.</param>
    /// <returns>A Card if the strategy returns a valid card, or null otherwise.</returns>
    public Card? Play(Card topCard, bool isStack, StackJudge stackJudge)
    {
        return Strategy.Play(Hand, topCard, isStack, stackJudge);
    }

    /// <summary>
    ///     Constructs a new player with the same name, hand type, and strategy.
    /// </summary>
    /// <returns>A new player with the same properties as this player.</returns>
    public Player Clone()
    {
        return new Player(Name, _handType, Strategy);
    }
}