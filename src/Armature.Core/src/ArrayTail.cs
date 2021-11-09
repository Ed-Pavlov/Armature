using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
///   Data structure used to take a tail of the collection w/o memory allocations
/// </summary>
/// <remarks>It implements <see cref="IEnumerable{T}"/> for rare and mostly debugging cases, use it wisely</remarks>
public readonly struct ArrayTail<T> : IEnumerable<T>
{
  private readonly IList<T> _array;
  private readonly int      _startIndex;

  [DebuggerStepThrough]
  public ArrayTail(IList<T> array, int startIndex)
  {
    if(array is null) throw new ArgumentNullException(nameof(array));
    if(startIndex < 0 || startIndex >= array.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));

    _array      = array;
    _startIndex = startIndex;
    Length      = _array.Count - _startIndex;
  }

  public int Length { get; }

  public T this[int index]
  {
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _array[_startIndex + index];
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ArrayTail<T> GetTail(int startIndex) => new(_array, _startIndex + startIndex);

  public override string ToString()
  {
    var sb = new StringBuilder();

    var i = 0;

    for(; i < Length - 1; i++)
    {
      sb.Append(this[i].ToHoconString());
      sb.Append(", ");
    }

    sb.Append(this[i].ToHoconString());
    return sb.ToString();
  }
  public IEnumerator<T>   GetEnumerator() => new Enumerator(_array, _startIndex);
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private struct Enumerator : IEnumerator<T?>
  {
    private readonly IList<T> _array;
    private readonly int      _startIndex;
    private          int      _iterator;
    private          T?       _current;

    public Enumerator(IList<T> array, int startIndex) : this()
    {
      _array      = array;
      _startIndex = startIndex;
      _iterator   = startIndex - 1;
      _current    = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() //TODO: check if it inlined, if no, return logic with exception
    {
      var result = ++_iterator < _array.Count;
      _current = result ? _array[_iterator] : default;
      return result;
    }

    public T? Current => _current;

    public void Reset()   => _iterator = _startIndex - 1;
    public void Dispose() { }

    object? IEnumerator.Current => Current;
  }
}