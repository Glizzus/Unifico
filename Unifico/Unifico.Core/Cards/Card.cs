using System.Diagnostics;

namespace Unifico.Core.Cards;

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