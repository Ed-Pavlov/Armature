using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches one <see cref="UnitInfo" /> with stored Unit id and token.
  /// </summary>
  public record UnitInfoMatcher : IUnitMatcher
  {
    private readonly object  _unitId;
    private readonly object? _token;

    [DebuggerStepThrough]
    public UnitInfoMatcher(UnitInfo unitInfo)
    {
      _unitId = unitInfo.Id ?? throw new ArgumentNullException(nameof(unitInfo.Id));
      _token  = unitInfo.Token;
    }

    /// <inheritdoc />
    /// <remarks>Matching, unlike equality, takes into consideration <see cref="Armature.Core.Token.Any"/>.</remarks>
    public virtual bool Matches(UnitInfo unit)
    {
      if(ReferenceEquals(null, unit)) return false;
      return Equals(_unitId, unit.Id) && _token.Matches(unit.Token);
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}({1}:{2})", GetType().GetShortName(), _unitId.ToLogString(), _token.ToLogString());
  }
}
