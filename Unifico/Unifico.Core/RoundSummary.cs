namespace Unifico.Core;

// Disable warning for unused record
#pragma warning disable 8618

public class CardInfo
{
    public string Color { get; set; }

    public string Face { get; set; }
    public bool Wild { get; set; }
}

public class BeforeAfter
{
    public int Before { get; set; }
    public int After { get; set; }
}

public class PlayerInfo
{
    public string Name { get; set; }
    public BeforeAfter HandCount { get; set; }
}

/// <summary>
///     A summary of a round of UNO.
///     This is includes the state of the player before and after the round.
/// </summary>
public record RoundSummary
{
    /// <summary>
    ///     Information regarding the player.
    /// </summary>
    public PlayerInfo PlayerInfo { get; set; }

    /// <summary>
    ///     The card that was on top of the discard pile at the start of the round.
    /// </summary>
    public CardInfo Target { get; set; }

    /// <summary>
    ///     Whether or not there was a plus stack in place at the start of the round.
    /// </summary>
    public bool WasStack { get; set; }

    /// <summary>
    ///     The stack count at the beginning and the end of the round.
    ///     This is intended to only be present if <see cref="WasStack" /> is <code>true</code>.
    /// </summary>
    public BeforeAfter? StackCount { get; set; }

    /// <summary>
    /// </summary>
    public CardInfo? Played { get; set; }

    public int? AmountDraw { get; set; }
}