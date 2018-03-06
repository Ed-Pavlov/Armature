using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <inheritdoc />
  /// <summary>
  /// Matches one <see cref="F:Armature.Core.UnitInfoMatcher.UnitInfo" /> with other.
  /// </summary>
  public class UnitInfoMatcher : IUnitMatcher
  {
    protected readonly UnitInfo UnitInfo;

    [DebuggerStepThrough]
    public UnitInfoMatcher([NotNull] UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      UnitInfo = unitInfo;
    }

    public virtual bool Matches(UnitInfo unitInfo)
    {
      return UnitInfo.Equals(unitInfo);
    }

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other)
    {
      return Equals(other as UnitInfoMatcher);
    }
    
    [DebuggerStepThrough]
    public override bool Equals(object obj)
    {
      return Equals(obj as UnitInfoMatcher);
    }

    [DebuggerStepThrough]
    private bool Equals(UnitInfoMatcher other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return UnitInfo == other.UnitInfo;
    }
    
    [DebuggerStepThrough]
    public override int GetHashCode()
    {
      return UnitInfo.GetHashCode();
    }

    [DebuggerStepThrough]
    public static bool operator ==(UnitInfoMatcher left, UnitInfoMatcher right)
    {
      return Equals(left, right);
    }

    [DebuggerStepThrough]
    public static bool operator !=(UnitInfoMatcher left, UnitInfoMatcher right)
    {
      return !Equals(left, right);
    }

    public override string ToString()
    {
      return string.Format("{0}: Unit={1}", GetType().Name, UnitInfo);
    }
  }
}