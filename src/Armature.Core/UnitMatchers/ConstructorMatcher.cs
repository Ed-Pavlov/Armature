using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  public class ConstructorMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Constructor && unitInfo.GetUnitTypeSafe() != null;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as ConstructorMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => 0;
    #endregion
  }
}