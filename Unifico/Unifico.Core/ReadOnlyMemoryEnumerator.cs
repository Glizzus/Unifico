using System.Collections;

namespace Unifico.Core;

public struct ReadOnlyMemoryEnumerator<T> : IEnumerator<T>
{
    private readonly ReadOnlyMemory<T> _memory;
    private int _index;

    public ReadOnlyMemoryEnumerator(ReadOnlyMemory<T> memory)
    {
        _memory = memory;
        Reset();
    }

    public bool MoveNext()
    {
        if (_index >= _memory.Length - 1) return false;
        _index++;
        return true;
    }

    public void Reset()
    {
        _index = -1;
    }

    public T Current => _index >= 0 ? _memory.Span[_index] : throw new InvalidOperationException();

    object IEnumerator.Current => Current!;

    public void Dispose()
    {
        // Nothing to dispose
    }
}