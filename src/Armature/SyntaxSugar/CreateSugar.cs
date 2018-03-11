using Armature.Core;
using Armature.Extensibility;
using Armature.Framework;
using Armature.Framework.BuildActions;
using JetBrains.Annotations;

namespace Armature
{
  public class CreateSugar<T> : UnitSequenceExtensibility
  {
    private readonly object _token;

    public CreateSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [CanBeNull] object token) : base(unitSequenceMatcher) => _token = token;

    public AdjusterSugar ByDefault()
    {
      var sequenceMatcher = new WeakUnitSequenceMatcher(Match.Type<T>(_token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);

      UnitSequenceMatcher
        .AddOrGetUnitMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, Default.CreationBuildAction, 0);
      return new AdjusterSugar(sequenceMatcher);
    }
    
    public AdjusterSugar ByReflection()
    {
      var sequenceMatcher = new WeakUnitSequenceMatcher(Match.Type<T>(_token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);

      UnitSequenceMatcher
        .AddOrGetUnitMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance, 0);
      return new AdjusterSugar(sequenceMatcher);
    }
  }
}