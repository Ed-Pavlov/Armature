using System;
using System.Diagnostics;
using BeatyBit.Armature.Core.Annotations;

namespace BeatyBit.Armature.Core;

/// <summary>
/// An Id of the Unit to be built.
/// </summary>
[Serializable]
public readonly struct UnitId : ILogString
{
  /// <summary>
  /// The part of an <see cref="UnitId"/>. It could be any object e.g. a <see cref="Type"/>, or a string constant.
  /// Whatever you find suitable to identify a Unit during tuning up the build process. See <see cref="IUnitPattern"/> and its implementations for details.
  /// </summary>
  public readonly object? Kind;

  /// <summary>
  /// The part of an <see cref="UnitId"/>. Two Units of the same <see cref="Kind"/> but with different <see cref="Tag"/> are different Units and could be
  /// built in different ways. It allows distinguishing e.g. two implementations of the same interface, etc.
  /// </summary>
  public readonly object? Tag;

  [DebuggerStepThrough]
  public UnitId(object? kind, object? tag)
  {
    if(kind is null && tag is null) throw new ArgumentNullException(nameof(kind), $"Either {nameof(kind)} or {nameof(tag)} should be provided");

    Kind = kind;
    Tag  = tag;
  }

  public override string ToString()      => ToHoconString();
  public          string ToHoconString() => $"{{ kind: {Kind.ToHoconString()}, tag: {Tag.ToHoconString()}}}";

  #region Equality implementation

  [DebuggerStepThrough]
  public bool Equals(UnitId other) => Equals(Kind, other.Kind) && Equals(Tag, other.Tag);

  [DebuggerStepThrough]
  public override bool Equals(object obj) => obj is UnitId other && Equals(other);

  [WithoutTest]
  [DebuggerStepThrough]
  public override int GetHashCode()
  {
    unchecked
    {
      return ((Kind is not null ? Kind.GetHashCode() : 0) * 397) ^ (Tag is not null ? Tag.GetHashCode() : 0);
    }
  }

  #endregion
}

/// <summary>
/// Syntax sugar for increasing code readability. 'Unit.Of(' looks cleaner than 'new UnitId('
/// </summary>
public static class Unit
{
  public static UnitId Of(object? kind, object? tag = null) => new(kind, tag);
}
