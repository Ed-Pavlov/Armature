using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Gets a default parameter value of the method parameter.
  /// </summary>
  public record GetParameterDefaultValue : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if(buildSession.GetUnitUnderConstruction().Kind is ParameterInfo {HasDefaultValue: true} parameterInfo)
        buildSession.BuildResult = new BuildResult(parameterInfo.DefaultValue);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
