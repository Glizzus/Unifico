using System.Diagnostics;

namespace Unifico.Core;

public class Game
{
    private const int UnoDeckSize = 108;
    private readonly Deck<Card> _deck;
    private readonly DiscardPile<Card> _discardPile;
    private readonly ReversibleRing<Player> _players;
    private readonly Rules _rules;
    private readonly StackJudge _stackJudge;

    
    public string Name { get; init; }
    public TextWriter Output { get; init; } = Console.Out;

    private int _stackCount;

    public Game(IEnumerable<Player> players, Rules rules)
    {
        _players = new ReversibleRing<Player>(players);
        _deck = GenerateGoldenDeck();
        _discardPile = new DiscardPile<Card>
        {
            CleanupPrevious = card => card.UnassignIfWild()
        };
        _rules = rules;
        _stackJudge = new StackJudge(_rules.PlusStackConvention);
    }
    
    private static Deck<Card> GenerateGoldenDeck()
    {
        var deck = new Deck<Card>(UnoDeckSize);
        foreach (var color in Enum.GetValues<Color>())
        {
            deck.Add(new Card
            {
                Color = color,
                Face = Face.Zero
            });
            foreach (var face in new[]
                     {
                         Face.One, Face.Two, Face.Three, Face.Four, Face.Five, Face.Six, Face.Seven, Face.Eight,
                         Face.Nine, Face.Skip, Face.Reverse, Face.PlusTwo
                     })
                for (var i = 0; i < 2; i++)
                    deck.Add(new Card
                    {
                        Color = color,
                        Face = face
                    });
        }

        foreach (var face in new[] { Face.Wild, Face.PlusFour })
            for (var i = 0; i < 4; i++)
                deck.Add(new Card
                {
                    Color = null,
                    Face = face,
                    IsWild = true
                });
        return deck;
    }

    private void InitialDeal()
    {
        for (var i = 0; i < _players.Count; i++)
        {
            var player = _players.Next();
            var cards = _deck.DrawMany(7) ??
                        throw new UnreachableException("The deck is not supposed to be empty during initialization.");
            player.Hand.AddRange(cards);
        }
    }

    private void InitializeDiscardPile()
    {
        while (true)
        {
            _deck.Shuffle();
            var card = _deck.Draw() ??
                       throw new UnreachableException("The deck is not supposed to be empty during initialization.");
            var cardType = card.GetCardType();
            switch (cardType)
            {
                case CardType.Basic:
                    _discardPile.Push(card);
                    return;
                case CardType.UnassignedWild:
                    switch (card.Face)
                    {
                        case Face.PlusFour:
                            _deck.Add(card);
                            continue;
                        case Face.Wild:
                        {
                            var randomColor = RandomUtils.Choice<Color>();
                            card.AssignColor(randomColor);
                            _discardPile.Push(card);
                            return;
                        }
                        case Face.PlusTwo:
                            _stackCount += 2;
                            return;
                        case Face.Reverse:
                            _players.Reverse();
                            return;
                        case Face.Skip:
                            _players.Skip();
                            return;
                        case Face.Zero:
                        case Face.One:
                        case Face.Two:
                        case Face.Three:
                        case Face.Four:
                        case Face.Five:
                        case Face.Six:
                        case Face.Seven:
                        case Face.Eight:
                        case Face.Nine:
                        default:
                            throw new UnreachableException("Only Wild and PlusFour cards can be unassigned.");
                    }
                case CardType.AssignedWild:
                    throw new UnreachableException("A card can not be an assigned wild card from the deck.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    ///     Obtains a number of cards from the deck.
    ///     This handles rebalancing the deck if it is empty.
    /// </summary>
    /// <param name="count">The amount of cards to retrieve</param>
    /// <returns>An IEnumerable of cards</returns>
    private IEnumerable<Card> ObtainCards(int count)
    {
        if (_deck.Count >= count) return _deck.DrawMany(count)!;
        var existingCards = _deck.DrawMany(_deck.Count)!;
        Rebalance();
        var restOfCards = _deck.DrawMany(count - existingCards.Count())!;
        return existingCards.Concat(restOfCards);
    }

    /// <summary>
    ///     Obtains a card from the deck.
    ///     This handles rebalancing the deck if it is empty.
    /// </summary>
    /// <returns>A guaranteed card.</returns>
    private Card ObtainCard()
    {
        if (_deck.Count == 0) Rebalance();
        return _deck.Draw()!;
    }

    private void Rebalance()
    {
        var cards = _discardPile.HeadTail();
        // TODO: Optimize the discard pile so that all of the wild cards are in a bucket.
        foreach (var card in cards) card.UnassignIfWild();
        _deck.AddMany(cards);
    }

    private void HandlePlayFailure(Player player, bool isStack)
    {
        if (isStack)
        {
            Output.WriteLine($"{player.Name} is forced to draw {_stackCount} cards.");
            var cards = ObtainCards(_stackCount);
            player.Hand.AddRange(cards);
            _stackCount = 0;
        }
        else
        {
            Output.WriteLine($"{player.Name} is forced to draw a card.");
            var newCard = ObtainCard();
            player.Hand.Add(newCard);
        }
    }

    private void HandlePlaySuccess(Card card)
    {
        _discardPile.Push(card);
        switch (card)
        {
            case { Face: Face.Skip }:
                _players.Skip();
                break;
            case { Face: Face.Reverse }:
                _players.Reverse();
                break;
            case { Face: Face.PlusTwo } or { Face: Face.PlusFour }:
                _stackCount += card.Face.PlusStackValue();
                break;
        }
    }

    public void Play()
    {
        _deck.Shuffle();
        InitialDeal();
        InitializeDiscardPile();
        while (true)
        {
            Output.WriteLine();
            var currentPlayer = _players.Next();
            Output.WriteLine($"It is {currentPlayer.Name}'s turn.");
            var topCard = _discardPile.Top;
            Output.WriteLine($"The top card is {topCard}.");
            var isStack = _stackCount > 0;
            Output.WriteLine(isStack ? $"The stack count is {_stackCount}." : "There is no stack");
            var card = currentPlayer.Play(topCard, isStack, _stackJudge);
            if (card is null)
            {
                Output.WriteLine($"{currentPlayer.Name} has no cards to play.");
                HandlePlayFailure(currentPlayer, isStack);
                continue;
            }

            Output.WriteLine($"{currentPlayer.Name} played {card}.");
            if (card.IsWild)
                card.AssignColor(topCard.Color ?? throw new InvalidDataException("The top card does not have a color"));
            HandlePlaySuccess(card);
            if (currentPlayer.HasWon)
            {
                Output.WriteLine($"{currentPlayer.Name} won!");
                Output.Flush();
                Output.Close();
                return;
            }

            Output.WriteLine();
        }
    }
}