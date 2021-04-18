using Armature.Core;


namespace Armature
{
  public class ParameterValueBuildPlan : BuildValuePlan, IParameterValueBuildPlan
  {
    public ParameterValueBuildPlan(IUnitIdPattern parameterPattern, IBuildAction getValueAction, int weight)
      : base(parameterPattern, getValueAction, weight) { }
  }
}
