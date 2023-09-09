namespace Unifico.Core.Cards;

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