using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Armature.Core.Common
{
  /// <summary>
  ///   Data structure used to take a tail of the collection w/o memory allocations
  /// </summary>
  public readonly struct ArrayTail<T>
  {
    private readonly IList<T> _array;
    private readonly int _startIndex;

    [DebuggerStepThrough]
    public ArrayTail(IList<T> array, int startIndex)
    {
      if (array is null) throw new ArgumentNullException(nameof(array));
      if (startIndex < 0 || startIndex >= array.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));

      _array = array;
      _startIndex = startIndex;
      Length = _array.Count - _startIndex;
    }

    public int Length { get; }

    public T this[int index] { [DebuggerStepThrough] get => _array[_startIndex + index]; }

    [DebuggerStepThrough]
    public ArrayTail<T> GetTail(int startIndex) => new(_array, _startIndex + startIndex);
  }
}