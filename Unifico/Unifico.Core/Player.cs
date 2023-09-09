namespace Unifico.Core;

public class Player
{
    public Player(string name, HandType handType, IStrategy strategy)
    {
        Name = name;
        _handType = handType;
        Hand = handType.Create();
        Strategy = strategy;
    }

    public string Name { get; init; }
    public IHand Hand { get; init; }
    private readonly HandType _handType;
    public IStrategy Strategy { get; init; }

    public bool HasWon => !Hand.Any();

    public Card? Play(Card topCard, bool isStack, StackJudge stackJudge)
    {
        return Strategy.Play(Hand, topCard, isStack, stackJudge);
    }
    
    public Player Clone()
    {
        return new Player(Name, _handType, Strategy);
    }
}