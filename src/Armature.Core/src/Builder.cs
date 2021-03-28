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
  ///   <see cref="Builder(IEnumerable{object}, Builder[])" /> for details.
  /// </summary>
  public class Builder : BuildPlansCollection
  {
    private readonly Builder[]? _parentBuilders;
    private readonly IEnumerable<object> _stages;

    public Builder() => throw new ArgumentException("Use constructor with parameters");

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit</param>
    public Builder(params object[] stages) : this(stages, EmptyArray<Builder>.Instance)
    {
      if (stages is null || stages.Length == 0) throw new ArgumentNullException(nameof(stages));
    }

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit</param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public Builder(IEnumerable<object> stages, params Builder[] parentBuilders)
    {
      if (stages == null) throw new ArgumentNullException(nameof(stages));
      if (parentBuilders != null && parentBuilders.Any(_ => _ == null)) throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

      var array = stages.ToArray();
      if (array.Length == 0) throw new ArgumentException("empty", nameof(stages));
      if (array.Any(stage => stage == null)) throw new ArgumentException("Contains null stage", nameof(stages));
      if (array.Length != array.Distinct().Count()) throw new ArgumentException("Contains duplicates", nameof(stages));

      _stages = array;
      _parentBuilders = parentBuilders == null || parentBuilders.Length == 0 ? null : parentBuilders;
    }

    /// <summary>
    ///   Builds a unit represented by <see cref="UnitInfo" />
    /// </summary>
    /// <param name="unitInfo">Building unit "id"</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
    /// <returns>Returns an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public BuildResult? BuildUnit(UnitInfo unitInfo, BuildPlansCollection? auxBuildPlans = null) 
      => BuildSession.BuildUnit(unitInfo, _stages, this, auxBuildPlans, _parentBuilders);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitInfo" />
    /// </summary>
    /// <param name="unitInfo">Building unit "id"</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
    /// <returns>Returns an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public IReadOnlyList<object?>? BuildAllUnits(UnitInfo unitInfo, BuildPlansCollection? auxBuildPlans = null) 
      => BuildSession.BuildAllUnits(unitInfo, _stages, this, auxBuildPlans, _parentBuilders)?.Select(_ => _.Value).ToArray();
  }
}