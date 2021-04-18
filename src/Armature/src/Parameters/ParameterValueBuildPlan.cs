using Armature.Core;


namespace Armature
{
  public class ParameterValueBuildPlan : BuildValuePlan, IParameterValueBuildPlan
  {
    public ParameterValueBuildPlan(IUnitPattern parameterPattern, IBuildAction getValueAction, int weight)
      : base(parameterPattern, getValueAction, weight) { }
  }
}
