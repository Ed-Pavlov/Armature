using System;
using System.Reflection;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class WeakParameterTypeValueBuildStep : ParameterValueBuildStep
  {
    private readonly object _parameterValue;

    public WeakParameterTypeValueBuildStep([NotNull] object parameterValue)
      : base(new SingletonBuildAction(parameterValue), ParameterValueBuildActionWeight.FreeValueResolver)
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