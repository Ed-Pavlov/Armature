using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  /// <summary>
  /// This build step matches with any parameter and its build action build unit represented 
  /// by <see cref="UnitInfo"/>(parameterInfo.Type, null). The default "auto wire" build step.
  /// </summary>
  public class AutowireParameterValueBuildStep : ParameterValueBuildStep
  {
    public AutowireParameterValueBuildStep(int matchingWeight) : base(GetBuildAction, matchingWeight)
    {}

    protected override bool Matches(ParameterInfo parameterInfo)
    {
      return true; // this build step matches any parameter
    }

    private static IBuildAction GetBuildAction(ParameterInfo parameterInfo)
    {
      return new RedirectTypeBuildAction(parameterInfo.ParameterType, null);
    }
  }
}