using System.Reflection;

namespace Armature.Core.BuildActions.Parameter
{
  public class GetDefaultParameterValueBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new GetDefaultParameterValueBuildAction();
    
    public void Process(IBuildSession buildSession)
    {
      if(buildSession.GetUnitUnderConstruction().Id is ParameterInfo parameterInfo && parameterInfo.HasDefaultValue)
        buildSession.BuildResult = new BuildResult(parameterInfo.DefaultValue);
    }

    public void PostProcess(IBuildSession buildSession) { }
  }
}