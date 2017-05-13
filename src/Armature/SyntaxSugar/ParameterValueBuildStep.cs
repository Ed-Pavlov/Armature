using System.Reflection;
using Armature.Core;
using Armature.Framework;
using Armature.Logging;

namespace Armature
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
        ? new StagedBuildAction(BuildStage.Create,_buildAction)
        : null;
    }

    private bool Matches(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;

      using (Log.Block(GetType().Name))
      {
        return parameterInfo != null && Equals(unitInfo.Token, SpecialToken.BuildParameterValue) && Matches(parameterInfo);

//        this.LogBuildStepMatch(buildSequence);
      }
    }

    protected abstract bool Matches(ParameterInfo parameterInfo);
  }
}