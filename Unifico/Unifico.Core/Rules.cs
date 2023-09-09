namespace Unifico.Core;

/// <summary>
///     The rules of UNO.
/// </summary>
public record Rules
{
    public static Rules Classic = new();

    public static Rules Official = new()
    {
        PlusStackConvention = PlusStackConvention.Banned,
        DrawUntilPlayable = false
    };

    public PlusStackConvention PlusStackConvention { get; init; } = PlusStackConvention.Conservative;
    public bool DrawUntilPlayable { get; init; }
}