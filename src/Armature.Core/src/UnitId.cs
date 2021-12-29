using System;
using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Describes a unit to be built.
/// </summary>
[Serializable]
public readonly struct UnitId : ILogString
{
  public readonly object? Kind;
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
