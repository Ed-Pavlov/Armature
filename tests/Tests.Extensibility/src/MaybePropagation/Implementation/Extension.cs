using System;
using Armature;
using Armature.Core;
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
      var treat     = treatingTuner.AsExtensibility<IUnitSequenceExtensibility>();
      var uniqueKey = Guid.NewGuid();
      treat.PatternTreeNode.UseBuildAction(BuildStage.Create, new BuildMaybeAction<T>(uniqueKey));

      return new TreatingTuner<T>(
        treat.PatternTreeNode.GetOrAddNode(new FindUnitMatches(new Pattern(typeof(T), uniqueKey), 0)));
    }

    /// <summary>
    ///   Specifies that value from built <see cref="Maybe{T}" /> should be used as a unit.
    /// </summary>
    public static TreatingTuner<Maybe<T>> AsMaybeValueOf<T>(this TreatingTuner<T> treatingTuner)
    {
      var treat = treatingTuner.AsExtensibility<IUnitSequenceExtensibility>();
      return new TreatingTuner<Maybe<T>>(treat.PatternTreeNode.UseBuildAction(BuildStage.Initialize, new GetMaybeValueBuildAction<T>()));
    }
  }
}
