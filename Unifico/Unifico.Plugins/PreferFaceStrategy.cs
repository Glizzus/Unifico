using Unifico.Core;
using Unifico.Core.Cards;
using Unifico.Core.Hand;
using Unifico.Core.Plugins;

namespace Unifico.Plugins;

public class PreferFaceStrategy : IStrategyPlugin
{
    public Card? Play(IHand hand, Card topCard, bool isStack, StackJudge stackJudge)
    {
        foreach (var card in hand)
            if (card.Face == topCard.Face)
            {
                if (!stackJudge.CanStack(card, topCard, isStack)) continue;
                hand.Remove(card);
                return card;
            }

        foreach (var card in hand)
        {
            if (!stackJudge.CanStack(card, topCard, isStack)) continue;
            hand.Remove(card);
            return card;
        }

        return null;
    }
}