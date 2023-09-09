namespace Unifico.Core;

public class ReversibleRing<T>
{
    private readonly ReadOnlyMemory<T> _values;
    private int _index = -1;
    private bool _reversed;

    public ReversibleRing(IEnumerable<T> values)
    {
        _values = new ReadOnlyMemory<T>(values.ToArray());
    }

    public int Count => _values.Length;

    public T Current => _index == -1 ? Next() : _values.Span[_index];

    private void AdjustIndex(int step)
    {
        var length = _values.Length;

        if (step > 0 || _index >= -step)
            _index = (_index + step) % length;
        else
            _index = length + step;
    }

    public void Skip()
    {
        var adjustment = _reversed ? -1 : 1;
        AdjustIndex(adjustment);
    }

    public void Reverse()
    {
        _reversed = !_reversed;
    }

    public T Next()
    {
        Skip();
        return Current;
    }
}