using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using Resharper.Annotations;

namespace Armature
{
  public class CreationTuner<T> : UnitSequenceExtensibility
  {
    private readonly object _token;

    public CreationTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [CanBeNull] object token) : base(unitSequenceMatcher) => _token = token;

    public Tuner ByDefault()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(Match.Type<T>(_token));

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return new Tuner(sequenceMatcher);
    }
    
    public Tuner ByReflection()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(Match.Type<T>(_token));

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new Tuner(sequenceMatcher);
    }
  }
}