using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Describes a unit to be built.
  /// </summary>
  [Serializable]
  public readonly struct UnitId
  {
    public readonly object? Kind;
    public readonly object? Key;

    [DebuggerStepThrough]
    public UnitId(object? kind, object? key)
    {
      if(kind is null && key is null) throw new ArgumentNullException(nameof(kind), $"Either {nameof(kind)} or {nameof(key)} should be provided");

      Kind = kind;
      Key  = key;
    }

    public override string ToString() => string.Format("{0}({1}, {2})", nameof(UnitId), Kind.ToLogString(), Key.ToLogString());

    #region Equality implementation

    [DebuggerStepThrough]
    public bool Equals(UnitId other) => Equals(Kind, other.Kind) && Equals(Key, other.Key);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => obj is UnitId other && Equals(other);

    [DebuggerStepThrough]
    public override int GetHashCode()
    {
      unchecked
      {
        return ((Kind is not null ? Kind.GetHashCode() : 0) * 397) ^ (Key is not null ? Key.GetHashCode() : 0);
      }
    }

    #endregion
  }
}
