using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Parameter
{
  public class GetDefaultParameterValueBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new GetDefaultParameterValueBuildAction();

    public void Process(IBuildSession buildSession)
    {
      if(buildSession.GetUnitUnderConstruction().Kind is ParameterInfo {HasDefaultValue: true} parameterInfo)
        buildSession.BuildResult = new BuildResult(parameterInfo.DefaultValue);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
