using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Parameters
{
  public class ParameterByWeakTypeMatcher : IUnitMatcher
  {
    private readonly object _parameterValue;

    [DebuggerStepThrough]
    public ParameterByWeakTypeMatcher([CanBeNull] object parameterValue) => _parameterValue = parameterValue;

    public bool Matches(UnitInfo unitInfo) =>
      unitInfo.Id is ParameterInfo parameterInfo && unitInfo.Token == SpecialToken.InjectValue && parameterInfo.ParameterType.IsInstanceOfType(_parameterValue);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _parameterValue.AsLogString());

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ParameterByWeakTypeMatcher matcher && Equals(_parameterValue, matcher._parameterValue);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as ParameterByWeakTypeMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _parameterValue != null ? _parameterValue.GetHashCode() : 0;
    #endregion
  }
}