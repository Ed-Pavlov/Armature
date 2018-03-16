using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByNameMatcher : IUnitMatcher
  {
    private readonly string _parameterName;

    [DebuggerStepThrough]
    public ParameterByNameMatcher([NotNull] string parameterName) => _parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));

    public bool Matches(UnitInfo unitInfo) => unitInfo.Id is ParameterInfo parameterInfo && unitInfo.Token == SpecialToken.ParameterValue && parameterInfo.Name == _parameterName;

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ParameterByNameMatcher matcher && _parameterName == matcher._parameterName;
    
    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _parameterName);
  }
}