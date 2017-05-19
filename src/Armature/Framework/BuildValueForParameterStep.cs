using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class BuildValueForParameterStep : LeafBuildStep
  {
    public BuildValueForParameterStep(int weight) : base(weight)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.BuildParameterValue)) return null;

      var parameterInfo = (ParameterInfo)unitInfo.Id;
      return new StagedBuildAction(BuildStage.Create, new RedirectTypeBuildAction(parameterInfo.ParameterType, null));
    }
  }
}