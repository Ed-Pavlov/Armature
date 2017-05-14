using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Armature.Common
{
  /// <summary>
  /// Data structure used to take a tail of the collection w/o memory allocations
  /// </summary>
  public struct ArrayTail<T>
  {
    private readonly IList<T> _array;
    private readonly int _startIndex;
    private readonly int _length;

    public ArrayTail([NotNull] IList<T> array, int startIndex)
    {
      if (array == null) throw new ArgumentNullException("array");
      if(startIndex < 0 || startIndex >= array.Count) throw new ArgumentOutOfRangeException("startIndex");

      _array = array;
      _startIndex = startIndex;
      _length = _array.Count - _startIndex;
    }

    public int Length
    {
      get { return _length; }
    }

    public T this[int index]
    {
      get { return _array[_startIndex + index]; }
    }

    public ArrayTail<T> GetTail(int startIndex)
    {
      return new ArrayTail<T>(_array, _startIndex + startIndex);
    }
  }

  public static class ArrayTail
  {
    public static ArrayTail<T> Of<T>(IList<T> array, int startIndex)
    {
      return new ArrayTail<T>(array, startIndex);
    }

    public static T GetLastItem<T>(this ArrayTail<T> arrayTail)
    {
      return arrayTail[arrayTail.Length - 1];
    }
  }
}