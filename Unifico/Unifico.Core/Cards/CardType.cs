namespace Unifico.Core.Cards;

/// <summary>
///     The type of a card.
/// </summary>
public enum CardType
{
    /// <summary>
    ///     A card that is not a wild card.
    ///     Examples include Red Three, Blue Skip, and Yellow Seven.
    /// </summary>
    Basic,

    /// <summary>
    ///     A card that is a wild card and a player has assigned a color to it.
    ///     These cards should only exist on the top of the discard pile.
    /// </summary>
    AssignedWild,

    /// <summary>
    ///     A card that is a wild card and a player has not assigned a color to it.
    ///     These cards exist in the decks and in players' hands.
    ///     They should never be in the discard pile.
    /// </summary>
    UnassignedWild
}