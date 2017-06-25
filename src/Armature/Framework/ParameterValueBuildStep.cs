using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public abstract class ParameterValueBuildStep : LeafBuildStep
  {
    private readonly Func<ParameterInfo, IBuildAction> _getBuildAction;

    protected ParameterValueBuildStep(Func<ParameterInfo, IBuildAction> getBuildAction, int weight) : base(weight)
    {
      _getBuildAction = getBuildAction;
    }

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;
      if (parameterInfo == null || !Equals(unitInfo.Token, SpecialToken.BuildParameterValue))
        return null;

      var matches = Matches(parameterInfo);
      Log.Verbose("{0}: {1}", GetType().Name, matches ? "matches" : "does not match");
      
      return matches 
        ? new StagedBuildAction(BuildStage.Create, _getBuildAction(parameterInfo)) 
        : null;
    }

    protected abstract bool Matches(ParameterInfo parameterInfo);
  }
}