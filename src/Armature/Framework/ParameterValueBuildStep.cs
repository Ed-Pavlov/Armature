using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public abstract class ParameterValueBuildStep : LeafBuildStep
  {
    private readonly IBuildAction _buildAction;

    protected ParameterValueBuildStep(IBuildAction buildAction, int weight) : base(weight)
    {
      _buildAction = buildAction;
    }

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      return Matches(unitInfo)
        ? new StagedBuildAction(BuildStage.Create, _buildAction)
        : null;
    }

    private bool Matches(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;
      if (parameterInfo == null || !Equals(unitInfo.Token, SpecialToken.BuildParameterValue))
        return false;

      var matches = Matches(parameterInfo);
      Log.Verbose("{0}: {1}", GetType().Name, matches ? "matches" : "does not match");
      
      return matches;
    }

    protected abstract bool Matches(ParameterInfo parameterInfo);
  }
}