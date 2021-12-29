using System;
using Armature;
using Armature.Core;
using Armature.Extensibility;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public static class Extension
  {
    /// <summary>
    /// Specifies what unit should be built to fill <see cref="Maybe{T}" /> value.
    /// </summary>
    public static TreatingTuner<T> TreatMaybeValue<T>(this TreatingTuner<Maybe<T>> treatingTuner)
    {
      var treat     = treatingTuner.GetInternals();
      var uniqueTag = Guid.NewGuid();
      treat.Member1.UseBuildAction(new BuildMaybeAction<T>(uniqueTag), BuildStage.Create);

      return new TreatingTuner<T>(
        treat.Member1.GetOrAddNode(new SkipTillUnit(new UnitPattern(typeof(T), uniqueTag), 0)));
    }

    /// <summary>
    /// Specifies that value from built <see cref="Maybe{T}" /> should be used as a unit.
    /// </summary>
    public static TreatingTuner<Maybe<T>> AsMaybeValueOf<T>(this TreatingTuner<T> treatingTuner)
    {
      var treat = treatingTuner.GetInternals();
      return new TreatingTuner<Maybe<T>>(treat.Member1.UseBuildAction(new GetMaybeValueBuildAction<T>(), BuildStage.Initialize));
    }
  }
}