using System;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Matches with a <see cref="ParameterInfo"/> with a given name
  /// </summary>
  public class NamedParameterValueBuildStep : ParameterValueBuildStep
  {
    private readonly string _parameterName;

    public NamedParameterValueBuildStep(int matchingWeight, [NotNull] string parameterName, [NotNull] Func<ParameterInfo, IBuildAction> getBuildAction)
      : base(getBuildAction, matchingWeight)
    {
      if (parameterName == null) throw new ArgumentNullException("parameterName");
      _parameterName = parameterName;
    }

    protected override bool Matches(ParameterInfo parameterInfo)
    {
      return _parameterName == parameterInfo.Name;
    }
  }
}