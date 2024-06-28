using System;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public static class Extension
  {
    /// <summary>
    /// Specifies what unit should be built to fill <see cref="Maybe{T}" /> value.
    /// </summary>
    public static IBuildingTuner<T> TreatMaybeValue<T>(this IBuildingTuner<Maybe<T>> buildingTuner)
    {
      var tuner = (ITuner) buildingTuner;

      var uniqueTag = Guid.NewGuid();
      tuner.Apply().UseBuildAction(new BuildMaybeAction<T>(uniqueTag), BuildStage.Create);

      var unitPattern = new UnitPattern(typeof(T), uniqueTag);

      return new BuildingTuner<T>(tuner, CreateNode, unitPattern);

      IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildStackPattern.IfFirstUnit);
    }

    /// <summary>
    /// Specifies that value from built <see cref="Maybe{T}" /> should be used as a unit.
    /// </summary>
    public static IBuildingTuner<T> AsMaybeValueOf<T>(this IBuildingTuner<T> buildingTuner)
    {
      var tuner = (ITuner) buildingTuner;
      tuner.Apply().UseBuildAction(new GetMaybeValueBuildAction<T>(), BuildStage.Initialize);
      return buildingTuner;
    }
  }
}
