using System.Collections;
using Unifico.Core.Cards;

namespace Unifico.Core.Hand;

public class DictionaryHand : IHand
{
    private readonly Dictionary<Card, int> _dictionary = new();

    public IEnumerator<Card> GetEnumerator()
    {
        foreach (var pair in _dictionary)
            for (var j = 0; j < pair.Value; j++)
                yield return pair.Key;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Card card)
    {
        if (_dictionary.ContainsKey(card))
            _dictionary[card]++;
        else
            _dictionary[card] = 1;
    }

    public void AddRange(IEnumerable<Card> cards)
    {
        foreach (var card in cards) Add(card);
    }

    public Card? Remove(Card card)
    {
        if (!_dictionary.ContainsKey(card) || _dictionary[card] == 0) return null;
        _dictionary[card]--;
        return card;
    }

    public bool Contains(Card card)
    {
        return _dictionary.ContainsKey(card);
    }
}