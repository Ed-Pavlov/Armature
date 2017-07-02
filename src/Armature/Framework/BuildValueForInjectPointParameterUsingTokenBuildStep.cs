using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework
{
  /// <summary>
  /// This build step matches with the parameter marked with <see cref="InjectAttribute"/> and its build action build unit represented 
  /// by <see cref="UnitInfo"/>(parameterInfo.Type, injectPointId as Token)
  /// </summary>
  public class BuildValueForInjectPointParameterUsingTokenBuildStep : LeafBuildStep
  {
    public BuildValueForInjectPointParameterUsingTokenBuildStep(int matchingWeight) : base(matchingWeight)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.BuildParameterValue))
      {
        Log.Trace("does not match unit");
        return null;
      }
      
      var parameterInfo = (ParameterInfo)unitInfo.Id;
      var injectAttribute = parameterInfo
        .GetCustomAttributes(typeof(InjectAttribute), true)
        .OfType<InjectAttribute>()
        .SingleOrDefault();

      var matches = injectAttribute != null && injectAttribute.InjectionPointId != null;
      Log.Verbose("{0} parameter {1}", matches ? "matches" : "does not match", parameterInfo);

      return matches
        ? new StagedBuildAction(BuildStage.Create, new RedirectTypeBuildAction(parameterInfo.ParameterType, injectAttribute.InjectionPointId))
        : null;
    }
  }
}