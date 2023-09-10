using System.Collections;
using Unifico.Core.Cards;

namespace Unifico.Core.Hand;

public class TabularHand : IHand
{
    private const int ColorsCount = 5;
    private const int FacesCount = 15;

    private readonly int[,] _cards = new int[ColorsCount, FacesCount];

    public void AddRange(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
            Add(card);
    }

    public Card? Remove(Card card)
    {
        var (colorIndex, faceIndex) = GetIndexes(card);
        if (_cards[colorIndex, faceIndex] == 0) return null;
        _cards[colorIndex, faceIndex]--;
        return card;
    }

    public bool Contains(Card card)
    {
        var (colorIndex, faceIndex) = GetIndexes(card);
        return _cards[colorIndex, faceIndex] > 0;
    }

    public IEnumerator<Card> GetEnumerator()
    {
        for (var i = 0; i < ColorsCount; i++)
        for (var j = 0; j < FacesCount; j++)
        for (var k = 0; k < _cards[i, j]; k++)
            yield return new Card
            {
                Color = (Color)i,
                Face = (Face)j,
                // If the color is 4, then the card is wild.
                IsWild = i == 4
            };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Card card)
    {
        var (colorIndex, faceIndex) = GetIndexes(card);
        _cards[colorIndex, faceIndex]++;
    }

    private static (int color, int face) GetIndexes(Card card)
    {
        // If the color is null, then the card must be wild.
        var colorIndex = card.Color is null ? 4 : (int)card.Color;
        var faceIndex = (int)card.Face;
        return (colorIndex, faceIndex);
    }
}