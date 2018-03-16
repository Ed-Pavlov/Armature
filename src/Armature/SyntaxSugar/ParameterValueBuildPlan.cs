using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class ParameterValueBuildPlan : IParameterValueBuildPlan
  {
    private readonly IUnitMatcher _parameterMatcher;
    private readonly int _weight;
    private readonly IBuildAction _buildAction;

    public ParameterValueBuildPlan([NotNull] IUnitMatcher parameterMatcher, [NotNull] IBuildAction buildAction, int weight)
    {
      if (parameterMatcher is null) throw new ArgumentNullException(nameof(parameterMatcher));
      if (buildAction is null) throw new ArgumentNullException(nameof(buildAction));

      _parameterMatcher = parameterMatcher;
      _weight = weight;
      _buildAction = buildAction;
    }

    void IParameterValueBuildPlan.Register(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(_parameterMatcher, _weight))
        .AddBuildAction(BuildStage.Create, _buildAction);
  }
}