using Unifico.Core.Cards;
using Unifico.Core.Hand;

namespace Unifico.Core.Strategy;

public class SimpleStrategy : IStrategy
{
    public Card? Play(IHand hand, Card topCard, bool isStack, StackJudge stackJudge)
    {
        var card = hand.FirstOrDefault(card => stackJudge.CanStack(card, topCard, isStack));
        if (card == null)
            return null;
        hand.Remove(card);
        return card;
    }
}