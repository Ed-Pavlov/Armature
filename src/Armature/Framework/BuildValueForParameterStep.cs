using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class BuildValueForParameterStep : LeafBuildStep
  {
    // it has no children and no state, so use a singleton
    public static readonly IBuildStep Instance = new BuildValueForParameterStep();

    private BuildValueForParameterStep() : base(0)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.BuildParameterValue)) return null;

      var parameterInfo = (ParameterInfo)unitInfo.Id;
      return new StagedBuildAction(BuildStage.Create, new RedirectTypeBuildAction(parameterInfo.ParameterType, null));
    }
  }
}