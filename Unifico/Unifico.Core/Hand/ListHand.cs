using System.Collections;
using Unifico.Core.Cards;

namespace Unifico.Core.Hand;

public class ListHand : IHand
{
    private readonly List<Card> _cards = new();

    public void Add(Card card)
    {
        _cards.Add(card);
    }

    public void AddRange(IEnumerable<Card> cards)
    {
        _cards.AddRange(cards);
    }

    public Card? Remove(Card card)
    {
        return _cards.Remove(card) ? card : null;
    }

    public IHand Clone()
    {
        var hand = new ListHand();
        hand.AddRange(_cards);
        return hand;
    }

    public bool Contains(Card card)
    {
        return _cards.Contains(card);
    }

    public IEnumerator<Card> GetEnumerator()
    {
        return _cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}