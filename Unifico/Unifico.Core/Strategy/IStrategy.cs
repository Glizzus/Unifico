using Unifico.Core.Cards;
using Unifico.Core.Hand;

namespace Unifico.Core.Strategy;

/// <summary>
///     A strategy for playing cards.
/// </summary>
public interface IStrategy
{
    /// <summary>
    ///     Attempt to play a card from the hand.
    ///     Note that the game will perform its own verifications with the
    /// </summary>
    /// <param name="hand">The player's hand including all of their cards.</param>
    /// <param name="topCard">The card which they are attempting to play on.</param>
    /// <param name="isStack">Whether or not there is plus stack in progress.</param>
    /// <param name="stackJudge">A StackJudge that indicates a card stacks on the <paramref name="topCard" /></param>
    /// <returns></returns>
    Card? Play(IHand hand, Card topCard, bool isStack, StackJudge stackJudge);
}