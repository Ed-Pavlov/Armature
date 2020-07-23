using System;
using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches one <see cref="UnitInfo" /> with other.
  /// </summary>
  public class UnitInfoMatcher : IUnitMatcher
  {
    protected readonly UnitInfo UnitInfo;

    [DebuggerStepThrough]
    public UnitInfoMatcher([NotNull] UnitInfo unitInfo) => UnitInfo = unitInfo ?? throw new ArgumentNullException(nameof(unitInfo));

    public virtual bool Matches(UnitInfo unitInfo) => UnitInfo.Matches(unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), UnitInfo);

    #region Equality
    [DebuggerStepThrough]
    public virtual bool Equals(IUnitMatcher obj) => obj is UnitInfoMatcher other && UnitInfo.Equals(other.UnitInfo);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as UnitInfoMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => UnitInfo.GetHashCode();
    #endregion
  }
}