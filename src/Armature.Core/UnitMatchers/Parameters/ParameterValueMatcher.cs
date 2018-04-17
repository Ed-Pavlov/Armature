using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers.Parameters
{
  public class ParameterValueMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ParameterValueMatcher();

    private ParameterValueMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && unitInfo.Id is ParameterInfo;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => 0;
    #endregion
  }
}