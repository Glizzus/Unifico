using System.Diagnostics;

namespace Unifico.Core;

/// <summary>
///     Represents a color of an UNO card.
/// </summary>
public enum Color
{
    Red,
    Green,
    Blue,
    Yellow
}

internal static class ColorExtensions
{
    public static Color Random()
    {
        var colors = Enum.GetValues<Color>();
        return colors[System.Random.Shared.Next(colors.Length)];
    }
}

/// <summary>
///     Represents a face of an UNO card.
/// </summary>
public enum Face
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Skip,
    Reverse,
    PlusTwo,
    Wild,
    PlusFour
}

/// <summary>
///     Provides extension methods for <see cref="Face" />.
/// </summary>
internal static class FaceExtensions
{
    /// <summary>
    ///     Indicates whether the specified <see cref="Face" /> is a number.
    /// </summary>
    /// <param name="face">The <see cref="Face" /> to check.</param>
    /// <returns>A boolean indicating whether the specified <see cref="Face" /> is a number.</returns>
    public static bool IsNumber(this Face face)
    {
        return face is >= Face.Zero and <= Face.Nine;
    }

    /// <summary>
    ///     Indicates whether the specified <see cref="Face" /> is a special card.
    ///     A special card is a card that is not a number.
    /// </summary>
    /// <param name="face">The <see cref="Face" /> to check.</param>
    /// <returns>A boolean indicating whether the specified <see cref="Face" /> is a special card.</returns>
    public static bool IsSpecial(this Face face)
    {
        return !face.IsNumber();
    }

    public static int PlusStackValue(this Face face)
    {
        return face switch
        {
            Face.PlusTwo => 2,
            Face.PlusFour => 4,
            _ => 0
        };
    }
}

public class Card
{
    private Color? _color;

    public Color? Color
    {
        get => _color;
        init => _color = value;
    }

    public Face Face { get; init; }
    public bool IsWild { get; init; } = false;

    public void AssignColor(Color color)
    {
        _color = color;
    }

    public void UnassignIfWild()
    {
        if (!IsWild)
            return;
        _color = null;
    }

    public CardType GetCardType()
    {
        return this switch
        {
            { IsWild: true, Color: null } => CardType.UnassignedWild,
            { IsWild: true, Color: not null } => CardType.AssignedWild,
            { IsWild: false, Color: not null } => CardType.Basic,
            { IsWild: false, Color: null } => throw new UnreachableException(
                "A card cannot be non-wild and have no color.")
        };
    }

    public override string ToString()
    {
        return $"{Color} {Face}";
    }
}