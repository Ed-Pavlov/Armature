using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public class ParameterByWeakTypeMatcher : IUnitMatcher
  {
    private readonly object _parameterValue;

    [DebuggerStepThrough]
    public ParameterByWeakTypeMatcher(object parameterValue) => _parameterValue = parameterValue;

    public bool Matches(UnitInfo unitInfo) =>
      unitInfo.Id is ParameterInfo parameterInfo && unitInfo.Token == SpecialToken.ParameterValue && parameterInfo.ParameterType.IsInstanceOfType(_parameterValue);

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ParameterByWeakTypeMatcher matcher && Equals(_parameterValue, matcher._parameterValue);
    
    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _parameterValue.AsLogString());
  }
}