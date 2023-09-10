using Unifico.Core.Cards;

namespace Unifico.Core.Hand;

public interface IHand : IEnumerable<Card>
{
    public void Add(Card card);
    public void AddRange(IEnumerable<Card> cards);
    public Card? Remove(Card card);

    public bool Contains(Card card);
}