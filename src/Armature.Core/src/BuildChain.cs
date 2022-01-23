using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Armature.Core.Annotations;

namespace Armature.Core;

/// <summary>
/// Data structure used to take a tail of the collection w/o memory allocations
/// </summary>
/// <remarks>It implements <see cref="IEnumerable{T}"/> for rare and mostly debugging cases, use it wisely</remarks>
public readonly struct BuildChain : IEnumerable<UnitId>
{
  private readonly IReadOnlyList<UnitId> _array;
  private readonly int                   _startIndex;

  [DebuggerStepThrough]
  public BuildChain() => throw new ArgumentException("Use constructor with parameters");
  [DebuggerStepThrough]
  public BuildChain(IReadOnlyList<UnitId> array, int startIndex)
  {
    if(array is null) throw new ArgumentNullException(nameof(array));
    if(startIndex < 0 || startIndex > array.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));

    _array      = array;
    _startIndex = startIndex;
  }

  public int Length => _array.Count - _startIndex;

  public UnitId this[int index]
  {
    [DebuggerStepThrough] [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _array[_startIndex + index];
  }

  /// <summary>
  /// The unit to be built, the last unit in the chain
  /// </summary>
  public UnitId TargetUnit
  {
    [DebuggerStepThrough] [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _array[0];
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public BuildChain GetTail(int startIndex) => new(_array, _startIndex + startIndex);

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

  public IEnumerator<UnitId> GetEnumerator() => new Enumerator(_array, _startIndex);

  private struct Enumerator : IEnumerator<UnitId>
  {
    private readonly IReadOnlyList<UnitId> _array;
    private readonly int                   _startIndex;
    private          int                   _iterator;

    public Enumerator(IReadOnlyList<UnitId> array, int startIndex) : this()
    {
      _array      = array;
      _startIndex = startIndex;
      Reset();
    }

    public bool MoveNext() => ++_iterator < _array.Count;

    public UnitId Current => _iterator < _array.Count ? _array[_iterator] : throw new InvalidOperationException("All items were enumerated");

    [WithoutTest]
    public void Reset() => _iterator = _startIndex - 1;

    [WithoutTest]
    public void Dispose() { }

    object IEnumerator.Current => Current;
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}