using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class WeakParameterTypeValueBuildStep : ParameterValueBuildStep
  {
    private readonly object _parameterValue;

    public WeakParameterTypeValueBuildStep(int weight, [NotNull] object parameterValue)
      : base(new SingletonBuildAction(parameterValue), weight)
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