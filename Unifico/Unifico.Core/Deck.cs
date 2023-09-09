namespace Unifico.Core;

public class Deck<T> where T : class
{
    private readonly List<T> _values;

    public Deck(int size)
    {
        _values = new List<T>(size);
    }

    public int Count => _values.Count;

    /// <summary>
    ///     Shuffles the deck.
    ///     This uses the classic Fisher-Yates shuffle.
    /// </summary>
    public void Shuffle()
    {
        RandomUtils.Shuffle(_values);
    }

    /// <summary>
    ///     Removes the last item from the deck and returns it.
    ///     This returns null if the deck is empty.
    /// </summary>
    /// <returns>The last item in the deck.</returns>
    public T? Draw()
    {
        if (_values.Count == 0) return null;
        var item = _values[^1];
        _values.RemoveAt(_values.Count - 1);
        return item;
    }

    public void Add(T value)
    {
        _values.Add(value);
    }

    public void AddMany(IEnumerable<T> values)
    {
        _values.AddRange(values);
    }

    /// <summary>
    ///     Removes the last <paramref name="count" /> items from the deck and returns them.
    ///     This returns null if the deck does not have enough items.
    /// </summary>
    /// <param name="count">The amount of items to draw from the deck.</param>
    /// <returns>An IEnumerable containing the </returns>
    public IList<T>? DrawMany(int count)
    {
        if (count > _values.Count) return null;
        var offset = _values.Count - count;
        var lastValues = _values.Skip(offset).ToList();
        _values.RemoveRange(offset, count);
        return lastValues;
    }
}