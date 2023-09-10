namespace Unifico.Core.Hand;

public static class HandFactory
{
    public static IHand Create(HandType handType)
    {
        return handType switch
        {
            HandType.ListHand => new ListHand(),
            HandType.TabularHand => new TabularHand(),
            HandType.DictionaryHand => new DictionaryHand(),
            _ => throw new ArgumentOutOfRangeException(nameof(handType), handType, null)
        };
    }
}