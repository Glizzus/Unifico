using Unifico.Core.Cards;
using Unifico.Core.Hand;

namespace Unifico.Core.Strategy;

public interface IStrategy
{
    Card? Play(IHand hand, Card topCard, bool isStack, StackJudge stackJudge);
}