using System;
using Armature;
using Armature.Core;
using Armature.Sdk;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public static class Extension
  {
    /// <summary>
    /// Specifies what unit should be built to fill <see cref="Maybe{T}" /> value.
    /// </summary>
    public static IBuildingTuner<T> TreatMaybeValue<T>(this IBuildingTuner<Maybe<T>> buildingTuner)
    {
      var tuner = (ITunerInternal) buildingTuner;

      var uniqueTag = Guid.NewGuid();
      tuner.BuildBranch().UseBuildAction(new BuildMaybeAction<T>(uniqueTag), BuildStage.Create);

      var unitPattern = new UnitPattern(typeof(T), uniqueTag);

      IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, 0); //TODO: weight

      return new BuildingTuner<T>(tuner, CreateNode, unitPattern);
    }

    /// <summary>
    /// Specifies that value from built <see cref="Maybe{T}" /> should be used as a unit.
    /// </summary>
    public static IBuildingTuner<T> AsMaybeValueOf<T>(this IBuildingTuner<T> buildingTuner)
    {
      var tuner = (ITunerInternal) buildingTuner;
      tuner.BuildBranch().UseBuildAction(new GetMaybeValueBuildAction<T>(), BuildStage.Initialize);
      return buildingTuner;
    }
  }
}
