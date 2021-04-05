using System;
using Armature;
using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public static class Extension
  {
    /// <summary>
    ///   Specifies what unit should be built to fill <see cref="Maybe{T}" /> value.
    /// </summary>
    public static TreatingTuner<T> TreatMaybeValue<T>(this TreatingTuner<Maybe<T>> treatingTuner)
    {
      var treat       = treatingTuner.AsExtensibility<IUnitSequenceExtensibility>();
      var uniqueToken = Guid.NewGuid();
      treat.UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new BuildMaybeAction<T>(uniqueToken));

      return new TreatingTuner<T>(treat.UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(new WildcardUnitSequenceMatcher(Match.Type<T>(uniqueToken), 0)));
    }

    /// <summary>
    ///   Specifies that value from built <see cref="Maybe{T}" /> should be used as a unit.
    /// </summary>
    public static TreatingTuner<Maybe<T>> AsMaybeValueOf<T>(this TreatingTuner<T> treatingTuner)
    {
      var treat = treatingTuner.AsExtensibility<IUnitSequenceExtensibility>();

      return new TreatingTuner<Maybe<T>>(treat.UnitSequenceMatcher.AddBuildAction(BuildStage.Initialize, new GetMaybeValueBuildAction<T>()));
    }
  }
}
