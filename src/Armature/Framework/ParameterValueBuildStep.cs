using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public abstract class ParameterValueBuildStep : LeafBuildStep
  {
    private readonly Func<ParameterInfo, IBuildAction> _getBuildAction;

    protected ParameterValueBuildStep(Func<ParameterInfo, IBuildAction> getBuildAction, int matchingWeight) : base(matchingWeight)
    {
      _getBuildAction = getBuildAction;
    }

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.BuildParameterValue))
        return null;
      
      var parameterInfo = (ParameterInfo)unitInfo.Id;
      var matches = Matches(parameterInfo);
      Log.Verbose("{0}: {1}", GetType().Name, matches ? "matches" : "does not match");
      
      return matches 
        ? new StagedBuildAction(BuildStage.Create, _getBuildAction(parameterInfo)) 
        : null;
    }

    protected abstract bool Matches(ParameterInfo parameterInfo);
  }
}