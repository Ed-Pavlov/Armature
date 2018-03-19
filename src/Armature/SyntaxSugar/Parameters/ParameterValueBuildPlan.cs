using Armature.Core;
using Armature.Properties;

namespace Armature
{
  public class ParameterValueBuildPlan : BuildValuePlan
  {
    public ParameterValueBuildPlan([NotNull] IUnitMatcher parameterMatcher, [NotNull] IBuildAction getValueAction, int weight) : base(parameterMatcher, getValueAction, weight)
    {}
  }
}