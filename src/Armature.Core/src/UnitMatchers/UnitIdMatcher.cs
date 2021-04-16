using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Matches one <see cref="UnitId" /> with stored Unit id and key.
  /// </summary>
  public record UnitIdMatcher : IUnitIdMatcher
  {
    private readonly object? _unitId;
    private readonly object? _key;

    [DebuggerStepThrough]
    public UnitIdMatcher(UnitId unitId) : this(unitId.Kind, unitId.Key) { }

    [DebuggerStepThrough]
    public UnitIdMatcher(object? unitId, object? key)
    {
      if(unitId is null && key is null) throw new ArgumentNullException(nameof(unitId), @"Either id or key should be provided");
      _unitId = unitId;
      _key  = key;
    }

    /// <inheritdoc />
    /// <remarks>Matching, unlike equality, takes into consideration <see cref="UnitKey.Any"/>.</remarks>
    public virtual bool Matches(UnitId unit)
    {
      if(ReferenceEquals(null, unit)) return false;
      return Equals(_unitId, unit.Kind) && _key.Matches(unit.Key);
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}({1}:{2})", GetType().GetShortName(), _unitId.ToLogString(), _key.ToLogString());
  }
}
