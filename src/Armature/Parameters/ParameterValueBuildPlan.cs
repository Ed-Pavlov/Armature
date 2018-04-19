using Armature.Core;
using Resharper.Annotations;

namespace Armature.Parameters
{
  public class ParameterValueBuildPlan : BuildValuePlan, IParameterValueBuildPlan
  {
    public ParameterValueBuildPlan([NotNull] IUnitMatcher parameterMatcher, [NotNull] IBuildAction getValueAction, int weight)
      : base(parameterMatcher, getValueAction, weight)
    {
    }
  }
}