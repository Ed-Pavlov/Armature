using System;
using System.Diagnostics;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Matches one <see cref="F:Armature.Core.UnitInfoMatcher.UnitInfo" /> with other.
  /// </summary>
  public class UnitInfoMatcher : IUnitMatcher
  {
    protected readonly UnitInfo UnitInfo;

    [DebuggerStepThrough]
    public UnitInfoMatcher([NotNull] UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      UnitInfo = unitInfo;
    }

    public virtual bool Matches(UnitInfo unitInfo) => UnitInfo.Equals(unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), UnitInfo.AsLogString());

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