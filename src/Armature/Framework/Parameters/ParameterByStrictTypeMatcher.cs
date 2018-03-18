using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Parameters
{
  public class ParameterByStrictTypeMatcher : IUnitMatcher
  {
    private readonly Type _parameterType;

    [DebuggerStepThrough]
    public ParameterByStrictTypeMatcher([NotNull] Type parameterType) => _parameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));

    public bool Matches(UnitInfo unitInfo) =>
      unitInfo.Id is ParameterInfo parameterInfo && unitInfo.Token == SpecialToken.InjectValue && parameterInfo.ParameterType == _parameterType;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _parameterType.AsLogString());

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ParameterByStrictTypeMatcher matcher && _parameterType == matcher._parameterType;

    public override bool Equals(object obj) => Equals(obj as ParameterByStrictTypeMatcher);

    public override int GetHashCode() => _parameterType.GetHashCode();
    #endregion
  }
}