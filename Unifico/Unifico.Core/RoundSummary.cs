namespace Unifico.Core;

public class CardInfo
{
    public string Color { get; set; }
    public string Face { get; set; }
    public bool Wild { get; set; }
}

public class BeforeAfter
{
    public int Before { get; set; }
    public int After { get; set; }
}

public class PlayerInfo
{
    public string Name { get; set; }
    public BeforeAfter HandCount { get; set; }
}

public record RoundSummary
{
    public PlayerInfo PlayerInfo { get; set; }
    public CardInfo Target { get; set; }
    public bool WasStack { get; set; }
    public BeforeAfter? StackCount { get; set; }
    public CardInfo? Played { get; set; }
    public int? AmountDraw { get; set; }
}