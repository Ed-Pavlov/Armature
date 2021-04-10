using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches one <see cref="UnitId" /> with stored Unit id and token.
  /// </summary>
  public record UnitInfoMatcher : IUnitMatcher
  {
    private readonly object? _unitId;
    private readonly object? _token;

    [DebuggerStepThrough]
    public UnitInfoMatcher(UnitId unitId) : this(unitId.Kind, unitId.Key) { }

    [DebuggerStepThrough]
    public UnitInfoMatcher(object? unitId, object? token)
    {
      if(unitId is null && token is null) throw new ArgumentNullException(nameof(unitId), @"Either id or token should be provided");
      _unitId = unitId;
      _token  = token;
    }

    /// <inheritdoc />
    /// <remarks>Matching, unlike equality, takes into consideration <see cref="Armature.Core.Token.Any"/>.</remarks>
    public virtual bool Matches(UnitId unit)
    {
      if(ReferenceEquals(null, unit)) return false;
      return Equals(_unitId, unit.Kind) && _token.Matches(unit.Key);
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}({1}:{2})", GetType().GetShortName(), _unitId.ToLogString(), _token.ToLogString());
  }
}
