using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches one <see cref="UnitInfo" /> with other.
  /// </summary>
  public record UnitInfoMatcher : IUnitMatcher
  {
    protected readonly UnitInfo UnitInfo;

    [DebuggerStepThrough]
    public UnitInfoMatcher(UnitInfo unitInfo) => UnitInfo = unitInfo ?? throw new ArgumentNullException(nameof(unitInfo));

    public virtual bool Matches(UnitInfo unitInfo) => UnitInfo.Matches(unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), UnitInfo);
  }
}
