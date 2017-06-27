using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Matches with a <see cref="ParameterInfo"/> with a given <see cref="Type"/>
  /// </summary>
  public class StrictParameterTypeValueBuildStep : ParameterValueBuildStep
  {
    private readonly Type _parameterType;

    public StrictParameterTypeValueBuildStep(int matchingWeight, [NotNull] Type parameterType, [NotNull] Func<ParameterInfo, IBuildAction> getBuildAction)
      : base(getBuildAction, matchingWeight)
    {
      if (parameterType == null) throw new ArgumentNullException("parameterType");
      _parameterType = parameterType;
    }

    protected override bool Matches(ParameterInfo parameterInfo)
    {
      var matches = _parameterType == parameterInfo.ParameterType;

      using (Log.Block(GetType().Name))
      {
        Log.Info("Match={0}", matches);
        Log.Info("MatchType={0}", _parameterType);
        Log.Info("ParameterType={0}", parameterInfo.ParameterType);
      }

      return matches;
    }
  }
}