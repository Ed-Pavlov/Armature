using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByStrictTypeMatcher : IUnitMatcher
  {
    private readonly Type _parameterType;

    [DebuggerStepThrough]
    public ParameterByStrictTypeMatcher([NotNull] Type parameterType)
    {
      if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));

      _parameterType = parameterType;
    }

    public bool Matches(UnitInfo unitInfo) =>
      unitInfo.Id is ParameterInfo parameterInfo && unitInfo.Token == SpecialToken.ParameterValue && parameterInfo.ParameterType == _parameterType;

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ParameterByStrictTypeMatcher matcher && _parameterType == matcher._parameterType;
  }
}