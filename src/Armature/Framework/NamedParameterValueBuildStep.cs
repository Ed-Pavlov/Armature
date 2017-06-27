using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
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
      var matches = _parameterName == parameterInfo.Name;

      if(!matches)
      {
        Log.Info("Does not match");
        Log.Info("MatchName={0}", _parameterName);
        Log.Info("ParameterName={0}", parameterInfo.Name);
      }

      return matches;
    }
  }
}