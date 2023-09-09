using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unifico.Core.Cards;

namespace Unifico.Core;

public class Game
{
    private const int UnoDeckSize = 108;
    private readonly Deck<Card> _deck;
    private readonly DiscardPile<Card> _discardPile;
    private readonly ReversibleRing<Player> _players;
    private readonly Rules _rules;
    private readonly StackJudge _stackJudge;

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

    private bool IsStack => _stackCount > 0;


    public string Name { get; init; }
    public TextWriter Output { get; init; } = Console.Out;

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

    /// <summary>
    ///     Handles when a player is unable to play a card.
    /// </summary>
    /// <param name="player">The player that failed to play.</param>
    /// <param name="isStack">Whether or not the player failed during a plus stack.</param>
    /// <returns>The amount of cards the player had to draw.</returns>
    private int HandlePlayFailure(Player player, bool isStack)
    {
        if (isStack)
        {
            var cards = ObtainCards(_stackCount);
            player.Hand.AddRange(cards);
            var amountDrawn = _stackCount;
            _stackCount = 0;
            return amountDrawn;
        }

        var newCard = ObtainCard();
        player.Hand.Add(newCard);
        return 1;
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

    private async Task InitializeGame()
    {
        _deck.Shuffle();
        InitialDeal();
        InitializeDiscardPile();
        await Output.WriteAsync("[");
    }

    private RoundSummary CreateRoundSummary()
    {
        return new RoundSummary
        {
            PlayerInfo = new PlayerInfo
            {
                Name = _players.Current.Name,
                HandCount = new BeforeAfter
                {
                    Before = _players.Current.Hand.Count()
                }
            },
            Target = new CardInfo
            {
                Color = _discardPile.Top.Color.ToString() ??
                        throw new InvalidDataException("The top card does not have a color"),
                Face = _discardPile.Top.Face.ToString(),
                Wild = _discardPile.Top.IsWild
            },
            WasStack = IsStack
        };
    }

    public double Entropy(Player player)
    {
        var count = player.Hand.Count();
        return player.Hand
            .GroupBy(c => c)
            .ToDictionary(group => group.Key, group => group.Count())
            .Select(frequency => frequency.Value / (double)count)
            .Aggregate<double, double>(0, (current, probability) => current - probability * Math.Log2(probability));
    }

    public Dictionary<Player, double> StartingEntropies()
    {
        var entropies = new Dictionary<Player, double>();
        for (var i = 0; i < _players.Count; i++)
        {
            var player = _players.Next();
            entropies.Add(player, Entropy(player));
        }

        return entropies;
    }

    public async Task<(Player, Dictionary<Player, double>)> Play()
    {
        await InitializeGame();
        var entropies = StartingEntropies();
        while (true)
        {
            var currentPlayer = _players.Next();
            var topCard = _discardPile.Top;

            var summary = CreateRoundSummary();
            if (IsStack) summary.StackCount = new BeforeAfter { Before = _stackCount };
            var card = currentPlayer.Play(topCard, IsStack, _stackJudge);
            if (card is null)
            {
                var amountDrawn = HandlePlayFailure(currentPlayer, IsStack);
                if (IsStack) summary.StackCount!.After = _stackCount;
                summary.AmountDraw = amountDrawn;

                summary.PlayerInfo.HandCount.After = currentPlayer.Hand.Count();
                var json2 = JsonSerializer.Serialize(summary, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });
                await Output.WriteAsync(json2);
                await Output.WriteAsync(",");
                continue;
            }

            if (card.IsWild)
                card.AssignColor(topCard.Color ?? throw new InvalidDataException("The top card does not have a color"));
            summary.Played = new CardInfo
            {
                Color = card.Color.ToString() ?? throw new InvalidDataException("The card does not have a color"),
                Face = card.Face.ToString(),
                Wild = card.IsWild
            };

            HandlePlaySuccess(card);

            summary.PlayerInfo.HandCount.After = currentPlayer.Hand.Count();
            var json = JsonSerializer.Serialize(summary, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });
            await Output.WriteAsync(json);
            if (!currentPlayer.HasWon)
            {
                await Output.WriteAsync(",");
                continue;
            }

            await Output.WriteAsync("]");
            await Output.DisposeAsync();
            return (currentPlayer, entropies);
        }
    }
}