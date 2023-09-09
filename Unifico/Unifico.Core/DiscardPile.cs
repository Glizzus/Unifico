namespace Unifico.Core;

/// <summary>
///     A pile that supports pushing items, and then replacing the entire pile with the top item.
///     This is a data structure similar to the way the discard pile works in UNO.
/// </summary>
public class DiscardPile<T>
{
    private List<T> _list = new();

    /// <summary>
    ///     An action that is invoked on the previous top item after a new item is pushed.
    /// </summary>
    public Action<T>? CleanupPrevious { get; init; }

    /// <summary>
    ///     The current top card on the discard pile.
    /// </summary>
    public T Top => _list[^1];

    /// <summary>
    ///     Adds a card to the top of this discard pile.
    /// </summary>
    public void Push(T item)
    {
        if (_list.Any()) CleanupPrevious?.Invoke(Top);
        _list.Add(item);
    }

    /// <summary>
    ///     Replace the discard pile with the top item on the discard pile.
    ///     The other items are returned in the order they were in the discard pile.
    /// </summary>
    /// <returns>The entire tail of the discard pile.</returns>
    public IList<T> HeadTail()
    {
        var head = Top;
        _list.RemoveAt(_list.Count - 1);
        var tail = _list;
        _list = new List<T> { head };
        return tail;
    }
}