using System.Collections;

namespace Unifico.Core;

public interface IHand : IEnumerable<Card>
{
    public void Add(Card card);
    public void AddRange(IEnumerable<Card> cards);
    public void Remove(Card card);

    public IHand Clone();
}

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

    public void Remove(Card card)
    {
        _cards.Remove(card);
    }
    
    public IHand Clone()
    {
        var hand = new ListHand();
        hand.AddRange(_cards);
        return hand;
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