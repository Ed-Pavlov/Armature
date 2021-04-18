using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core.Common;


namespace Armature.Core
{
  /// <summary>
  ///   The builder of units. It is the convenient way to keep corresponding build plans (<see cref="BuildPlansCollection" />),
  ///   build stages, and parent builders to pass into <see cref="BuildSession" /> which is instantiated independently.
  ///   Building a unit it goes over all "build stages", for each stage it gets a build action if any and executes it see
  ///   <see cref="Builder(object[], Builder[])" /> for details.
  /// </summary>
  public class Builder : BuildPlansCollection
  {
    private readonly Builder[]?          _parentBuilders;
    private readonly IEnumerable<object> _stages;

    public Builder() => throw new ArgumentException("Provide stages");

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit.</param>
    public Builder(params object[] stages) : this(stages, EmptyArray<Builder>.Instance)
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
        throw new ArgumentException("Contains null stage", nameof(stages));

      if(stages.Length != stages.Distinct().Count())
        throw new ArgumentException("Contains duplicates", nameof(stages));

      _stages         = stages;
      _parentBuilders = parentBuilders is null || parentBuilders.Length == 0 ? null : parentBuilders;
    }

    /// <summary>
    ///   Builds a unit represented by <see cref="UnitId" />
    /// </summary>
    /// <param name="unitId">The id of the unit to build.</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies.</param>
    /// <returns>Returns an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public BuildResult BuildUnit(UnitId unitId, BuildPlansCollection? auxBuildPlans = null)
      => Build(unitId, auxBuildPlans, BuildSession.BuildUnit, _parentBuilders);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitId" />
    /// </summary>
    /// <param name="unitId">Building unit "id"</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
    /// <returns>Returns an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public IReadOnlyList<object?>? BuildAllUnits(UnitId unitId, BuildPlansCollection? auxBuildPlans = null)
    {
      var buildResult = Build(unitId, auxBuildPlans, BuildSession.BuildAllUnits, _parentBuilders);

      return buildResult?.Select(_ => _.Value).ToArray();
    }

    private T? Build<T>(
      UnitId                                                                                        unitId,
      BuildPlansCollection?                                                                         auxBuildPlans,
      Func<UnitId, IEnumerable<object>, BuildPlansCollection, BuildPlansCollection?, Builder[]?, T> build,
      Builder[]?                                                                                    parentBuilders)
    {
      if(build is null) throw new ArgumentNullException(nameof(build));

      return build(unitId, _stages, this, auxBuildPlans, parentBuilders);
    }
  }
}
