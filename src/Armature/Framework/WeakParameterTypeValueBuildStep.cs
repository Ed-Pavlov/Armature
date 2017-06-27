using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Matches with a <see cref="ParameterInfo"/> if a provided parameter value is assignable to <see cref="ParameterInfo.ParameterType"/> 
  /// </summary>
  public class WeakParameterTypeValueBuildStep : ParameterValueBuildStep
  {
    private readonly object _parameterValue;

    public WeakParameterTypeValueBuildStep(int matchingWeight, [NotNull] object parameterValue)
      : base(_ => new SingletonBuildAction(parameterValue), matchingWeight)
    {
      if (parameterValue == null) throw new ArgumentNullException("parameterValue");
      _parameterValue = parameterValue;
    }

    protected override bool Matches(ParameterInfo parameterInfo)
    {
      return parameterInfo.ParameterType.IsInstanceOfType(_parameterValue);
    }
  }
}