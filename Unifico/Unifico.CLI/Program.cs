// See https://aka.ms/new-console-template for more information

using Unifico.Core;

namespace Unifico.CLI;

internal static class Application
{
    private static Player SimplePlayer(string name)
    {
        return new Player(name, HandType.ListHand, new SimpleStrategy());
    }

    public static async Task Main(string[] args)
    {
        var players = new List<Player>
        {
            SimplePlayer("Cal Crosby"),
            SimplePlayer("Payden Whiting"),
            SimplePlayer("Conner Runion"),
            SimplePlayer("Blake Collins")
        };

        var master = new GameMaster(players, 4, 10, new Rules());
        await master.Run();
    }
}

internal class SimpleStrategy : IStrategy
{
    public Card? Play(IHand hand, Card topCard, bool isStack, StackJudge stackJudge)
    {
        var card = hand.FirstOrDefault(card => stackJudge.CanStack(card, topCard, isStack));
        if (card == null)
            return null;
        hand.Remove(card);
        return card;
    }
}