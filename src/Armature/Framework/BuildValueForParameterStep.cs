using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  /// <summary>
  /// This build step matches with any parameter and its build action build unit represented 
  /// by <see cref="UnitInfo"/>(parameterInfo.Type, null). The default "auto wire" build step.
  /// </summary>
  public class BuildValueForParameterStep : LeafBuildStep
  {
    public BuildValueForParameterStep(int matchingWeight) : base(matchingWeight)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.BuildParameterValue)) return null;

      var parameterInfo = (ParameterInfo)unitInfo.Id;
      return new StagedBuildAction(BuildStage.Create, new RedirectTypeBuildAction(parameterInfo.ParameterType, null));
    }
  }
}