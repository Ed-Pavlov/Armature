using Armature.Core;


namespace Armature
{
  public class ParameterValueBuildPlan : BuildValuePlan, IParameterValueBuildPlan
  {
    public ParameterValueBuildPlan(IUnitMatcher parameterMatcher, IBuildAction getValueAction, int weight)
      : base(parameterMatcher, getValueAction, weight) { }
  }
}
