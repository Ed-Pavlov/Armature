using System.Diagnostics;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyParameterMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyParameterMatcher();

    private AnyParameterMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.ParameterValue && unitInfo.Id is ParameterInfo;

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);
  }
}