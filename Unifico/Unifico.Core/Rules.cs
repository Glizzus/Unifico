namespace Unifico.Core;

/// <summary>
///     The rules of UNO.
/// </summary>
public record Rules
{
    /// <summary>
    ///     The classic house rules of UNO.
    ///     Plus Stacking is allowed to a conservative extent.
    /// </summary>
    public static readonly Rules Classic = new();

    /// <summary>
    ///     The official rules of UNO.
    ///     Plus Stacking is banned.
    /// </summary>
    public static readonly Rules Official = new()
    {
        PlusStackConvention = PlusStackConvention.Banned,
        DrawUntilPlayable = false
    };

    /// <summary>
    ///     The way that Plus Stacking is handled.
    /// </summary>
    public PlusStackConvention PlusStackConvention { get; init; } = PlusStackConvention.Conservative;


    /// <summary>
    ///     Whether or not a player can must draw until they have a playable card.
    /// </summary>
    public bool DrawUntilPlayable { get; init; }
}