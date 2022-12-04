using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Armature.Core.Annotations;

namespace Armature.Core;

public partial class BuildSession
{
  /// <summary>
  /// Data structure used to take a tail of the collection w/o memory allocations
  /// </summary>
  /// <remarks>It implements <see cref="IEnumerable{T}"/> for rare and mostly debugging cases, use it wisely</remarks>
  public readonly struct Stack : IEnumerable<UnitId>
  {
    private readonly IReadOnlyList<UnitId> _array;
    private readonly int                   _startIndex;

    [DebuggerStepThrough]
    public Stack() => throw new ArgumentException("Use constructor with parameters");

    [DebuggerStepThrough]
    public Stack(IReadOnlyList<UnitId> array) : this(array, 0, GetTargetUnit(array)) { }

    [DebuggerStepThrough]
    private Stack(IReadOnlyList<UnitId> array, int startIndex, UnitId targetUnit)
    {
      if(startIndex < 0 || startIndex > array.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));

      _array      = array;
      TargetUnit  = targetUnit;
      _startIndex = startIndex;
    }

    public int Length => _array.Count - _startIndex;

    public UnitId this[int index]
    {
      [DebuggerStepThrough] [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
        var reverseIndex = _array.Count - 1 - (_startIndex + index);
        return _array[reverseIndex];
      }
    }

    /// <summary>
    /// The unit to be built
    /// </summary>
    public UnitId TargetUnit
    {
      [DebuggerStepThrough] [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Stack GetTail(int startIndex) => new(_array, _startIndex + startIndex, TargetUnit);

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

    private static UnitId GetTargetUnit(IReadOnlyList<UnitId> array)
    {
      if(array is null) throw new ArgumentNullException(nameof(array));
      return array[array.Count - 1];
    }

    public IEnumerator<UnitId> GetEnumerator() => new Enumerator(this);

    private struct Enumerator : IEnumerator<UnitId>
    {
      private readonly Stack _buildStack;

      private bool   _disposed;
      private int    _iterator;
      private UnitId _current;

      public Enumerator(Stack stack)
      {
        _buildStack = stack;
        Reset();
      }

      public bool MoveNext()
      {
        if(_disposed) throw new ObjectDisposedException(nameof(Enumerator));

        if(_iterator >= _buildStack.Length)
        {
          _current = default;
          return false;
        }

        _current = _buildStack[_iterator++];
        return true;
      }

      public UnitId Current
      {
        get
        {
          if(_disposed) throw new ObjectDisposedException(nameof(Enumerator));
          return _current;
        }
      }

      [WithoutTest]
      public void Reset()
      {
        _current  = default;
        _iterator = 0;
      }

      [WithoutTest]
      public void Dispose() => _disposed = true;

      object IEnumerator.Current => Current;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
