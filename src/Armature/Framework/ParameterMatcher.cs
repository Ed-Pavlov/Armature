using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public class ParameterMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ParameterMatcher();

    private ParameterMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.ParameterValue && unitInfo.Id is ParameterInfo;

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}