namespace Unifico.Core.Tests;

public class ReversibleRingTests
{
    [Test]
    public void NextShouldReturnNextValue()
    {
        var strings = new[] { "a", "b", "c" };
        var ring = new ReversibleRing<string>(strings);

        Assert.AreEqual("a", ring.Current);
        Assert.AreEqual("b", ring.Next());
        Assert.AreEqual("c", ring.Next());
        Assert.AreEqual("a", ring.Next());
    }

    [Test]
    public void SkipShouldSkipValue()
    {
        var strings = new[] { "a", "b", "c" };
        var ring = new ReversibleRing<string>(strings);

        Assert.AreEqual("a", ring.Current);
        ring.Skip();
        Assert.AreEqual("b", ring.Current);
        ring.Skip();
        Assert.AreEqual("c", ring.Current);
        ring.Skip();
        Assert.AreEqual("b", ring.Next());
    }

    [Test]
    public void ReverseShouldReverseDirection()
    {
        var strings = new[] { "a", "b", "c" };
        var ring = new ReversibleRing<string>(strings);

        Assert.AreEqual("a", ring.Current);
        ring.Reverse();
        Assert.AreEqual("c", ring.Next());
        Assert.AreEqual("b", ring.Next());
        Assert.AreEqual("a", ring.Next());
    }
}