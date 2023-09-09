namespace Unifico.Core;

/// <summary>
///     A judge that determines whether a card can be stacked on another card.
///     This is used to abstract away the rules of stacking cards from the player.
/// </summary>
public class StackJudge
{
    private readonly Func<Card, Card, bool> _stackFunction;

    public StackJudge(PlusStackConvention convention)
    {
        _stackFunction = PlusStackFunction(convention);
    }

    /// <summary>
    ///     Indicates whether the given card can be stacked on the given target card.
    /// </summary>
    /// <param name="stacker">The card that is attempting to be played.</param>
    /// <param name="target">The card that is the target to be stacked upon.</param>
    /// <param name="isStack">Indicates whether there is a Plus Stack in place.</param>
    /// <returns>A boolean indicating whether the card can stack on the target card.</returns>
    public bool CanStack(Card stacker, Card target, bool isStack)
    {
        if (!isStack) return stacker.Color == target.Color || stacker.IsWild || stacker.Face == target.Face;
        return _stackFunction(stacker, target);
    }

    private static bool ConservativeStack(Card stacker, Card target)
    {
        return stacker.Face == target.Face || stacker.Face == Face.PlusFour;
    }

    private static Func<Card, Card, bool> PlusStackFunction(PlusStackConvention convention)
    {
        return convention switch
        {
            PlusStackConvention.Banned => (_, _) => false,
            PlusStackConvention.Conservative => ConservativeStack,
            PlusStackConvention.Liberal => (stacker, target) =>
                ConservativeStack(stacker, target) || stacker.Color == target.Color,
            _ => throw new NotImplementedException()
        };
    }
}