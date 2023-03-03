using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Armature.Core;

public partial class BuildSession
{
  /// <summary>
  /// Represents the "building stack".
  /// It could be for example IA -> A -> IB -> B -> int. This stack means that for now Unit of type 'int' is the target unit
  /// but it is built in the "context" of the whole build stack.
  /// </summary>
  public readonly struct Stack
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

    /// <summary>
    /// Gets the tails of the stack. <see cref="TargetUnit"/> remains the same, in opposite to items accessed by <see cref="this[int]"/>
    /// </summary>
    /// <param name="startIndex">The index of the item which should became the very first item of the tail.</param>
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
  }
}
