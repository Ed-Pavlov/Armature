using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework
{
  /// <summary>
  /// This build step applyes to the parameter marked with InjectAttribute and build unit represented by UnitInfo(parameterInfo.Type, injectPointId as Token)
  /// </summary>
  public class BuildValueForInjectPointParameterUsingTokenBuildStep : LeafBuildStep
  {
    public BuildValueForInjectPointParameterUsingTokenBuildStep(int weight) : base(weight)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.BuildParameterValue)) return null;
      
      var parameterInfo = (ParameterInfo)unitInfo.Id;
      var injectAttribute = parameterInfo
        .GetCustomAttributes(typeof(InjectAttribute), true)
        .OfType<InjectAttribute>()
        .SingleOrDefault();

      var matches = injectAttribute != null && injectAttribute.InjectionPointId != null;
      Log.Verbose("{0}: {1}", GetType().Name, matches ? "matches" : "does not match");

      return matches
        ? new StagedBuildAction(BuildStage.Create, new RedirectTypeBuildAction(parameterInfo.ParameterType, injectAttribute.InjectionPointId))
        : null;
    }
  }
}