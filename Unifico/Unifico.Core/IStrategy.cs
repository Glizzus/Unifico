namespace Unifico.Core;

public interface IStrategy
{
    Card? Play(IHand hand, Card topCard, bool isStack, StackJudge stackJudge);
}