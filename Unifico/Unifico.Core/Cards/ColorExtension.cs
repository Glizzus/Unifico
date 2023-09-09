namespace Unifico.Core.Cards;

internal static class ColorExtensions
{
    public static Color Random()
    {
        var colors = Enum.GetValues<Color>();
        return colors[System.Random.Shared.Next(colors.Length)];
    }
}