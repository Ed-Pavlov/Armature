using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public interface IParameterValueBuildPlan : IBuildPlan{}
  
  public class ParameterValueBuildPlan : BuildValuePlan, IParameterValueBuildPlan
  {
    public ParameterValueBuildPlan([NotNull] IUnitMatcher parameterMatcher, [NotNull] IBuildAction getValueAction, int weight) : base(parameterMatcher, getValueAction, weight)
    {}
  }
}