using Armature.Core;


namespace Armature
{
  public class ParameterValueBuildPlan : BuildValuePlan, IParameterValueBuildPlan
  {
    public ParameterValueBuildPlan(IUnitIdMatcher parameterMatcher, IBuildAction getValueAction, int weight)
      : base(parameterMatcher, getValueAction, weight) { }
  }
}
