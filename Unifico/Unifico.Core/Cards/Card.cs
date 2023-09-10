using System.Diagnostics;

namespace Unifico.Core.Cards;

/// <summary>
///     An UNO card.
///     These have a <see cref="Face" /> and a <see cref="Color" />.
///     Note that cards can be wild, in which case they have no color.
///     Wild cards, however, must be assigned a color before they can be played.
/// </summary>
public class Card
{
    private Color? _color;

    public Color? Color
    {
        get => _color;
        init => _color = value;
    }

    public Face Face { get; init; }

    /// <summary>
    ///     Indicates whether this card is wild.
    ///     This is to keep track of whether the card is a wild
    ///     card even after it has been assigned a color.
    /// </summary>
    public bool IsWild { get; init; }

    /// <summary>
    ///     Assigns a color to this card.
    ///     This should only be called on unassigned wild cards.
    /// </summary>
    /// <param name="color"></param>
    /// <exception cref="CardException">If attempting to assign a color to a non-unassigned wild card.</exception>
    public void AssignColor(Color color)
    {
        if (!IsWild || _color is not null)
            throw new CardException("Cannot assign color to a non-unassigned wild card.");
        _color = color;
    }

    /// <summary>
    ///     Unassigns the color of this card if it is a wild card.
    ///     If it is not a wild card, this does nothing.
    ///     This is intended to be used when reusing wild cards and
    ///     putting them back in the deck.
    /// </summary>
    public void UnassignIfWild()
    {
        if (!IsWild)
            return;
        _color = null;
    }

    /// <summary>
    ///     Gets the <see cref="CardType"/> of this card.
    /// </summary>
    /// <returns>This card's <see cref="CardType"/></returns>
    /// <exception cref="CardException">If this card isn't wild and has a null color.</exception>
    public CardType GetCardType()
    {
        return this switch
        {
            { IsWild: true, Color: null } => CardType.UnassignedWild,
            { IsWild: true, Color: not null } => CardType.AssignedWild,
            { IsWild: false, Color: not null } => CardType.Basic,
            { IsWild: false, Color: null } => throw new CardException("Somehow a non-wild card has a null color.")
        };
    }

    public override string ToString()
    {
        return $"{Color} {Face}";
    }
}