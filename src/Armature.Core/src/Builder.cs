using System;
using System.Collections.Generic;
using System.Linq;


namespace Armature.Core
{
  /// <summary>
  ///   The builder of units. It is the convenient way to keep corresponding build plans (<see cref="BuildPlanCollection" />),
  ///   build stages, and parent builders to pass into <see cref="BuildSession" /> which can be instantiated independently.
  /// </summary>
  public class Builder : BuildPlanCollection
  {
    private readonly object[]   _stages;
    private readonly Builder[]? _parentBuilders;

    public Builder() => throw new ArgumentException("Provide stages");

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit.</param>
    public Builder(params object[] stages) : this(stages, Empty<Builder>.Array)
    {
      if(stages.Length == 0) throw new ArgumentNullException(nameof(stages));
    }

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit.</param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, tries to build a unit using
    ///   parent builders one by one in the order they passed into the constructor.
    /// </param>
    public Builder(object[] stages, params Builder[] parentBuilders)
    {
      if(stages is null) throw new ArgumentNullException(nameof(stages));

      if(parentBuilders is not null && parentBuilders.Any(_ => _ is null))
        throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

      if(stages.Length == 0)
        throw new ArgumentException("empty", nameof(stages));

      if(stages.Any(stage => stage is null))
        throw new ArgumentException("Should not contain null values", nameof(stages));

      if(stages.Length != stages.Distinct().Count())
        throw new ArgumentException("Should not contain duplicate values", nameof(stages));

      _stages         = stages;
      _parentBuilders = parentBuilders is null || parentBuilders.Length == 0 ? null : parentBuilders;
    }

    /// <summary>
    ///   Builds a unit represented by <see cref="UnitId" />
    /// </summary>
    /// <param name="unitId">The id of the unit to build.</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies.</param>
    /// <returns>Returns build result with <see cref="BuildResult.HasValue"/> set to false if unit is not built.</returns>

    //TODO: what about exceptions? if buildResult.HasValue == false does it mean that there is no a registration, or it can be some runtime problems? 
    public BuildResult BuildUnit(UnitId unitId, IPatternTreeNode? auxBuildPlans = null) 
      => new BuildSession(_stages, this, auxBuildPlans, _parentBuilders).BuildUnit(unitId);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
    ///   This can be useful to build all implementers of an interface.
    /// </summary>
    /// <param name="unitId">Building unit "id"</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
    /// <returns>Returns <see cref="Empty{BuildResult}.List"/> if no units were built. </returns>
    public IReadOnlyList<BuildResult> BuildAllUnits(UnitId unitId, IPatternTreeNode? auxBuildPlans = null)
      => new BuildSession(_stages, this, auxBuildPlans, _parentBuilders).BuildAllUnits(unitId);
  }
}
