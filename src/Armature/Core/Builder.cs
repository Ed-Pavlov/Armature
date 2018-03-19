using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using Armature.Properties;

namespace Armature.Core
{
  /// <summary>
  ///   The "dependency injection builder" contains build steps for units and method <see cref="BuildUnit(Armature.Core.UnitInfo,Armature.Core.BuildPlansCollection)" />
  ///   Building a unit it goes over all "build stages", for each stage it gets a build step if any and executes it see
  ///   <see cref="Builder(System.Collections.Generic.IEnumerable{System.Object},Armature.Core.Builder[])" /> for details
  /// </summary>
  public class Builder : BuildPlansCollection
  {
    private readonly Builder[] _parentBuilders;
    private readonly IEnumerable<object> _stages;

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit</param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public Builder([NotNull] IEnumerable<object> stages, params Builder[] parentBuilders)
    {
      if (stages == null) throw new ArgumentNullException(nameof(stages));
      if (parentBuilders != null && parentBuilders.Any(_ => _ == null)) throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

      var array = stages.ToArray();
      if (array.Length == 0)
        throw new ArgumentException("empty", nameof(stages));
      if(array.Any(stage => stage == null))
        throw new ArgumentException("Contains null stage", nameof(stages));
      if(array.Length != array.Distinct().Count())
        throw new ArgumentException("Contains duplicates", nameof(stages));

      _stages = array;
      _parentBuilders = parentBuilders == null || parentBuilders.Length == 0 ? null : parentBuilders;
    }

    /// <summary>
    ///   Builds a unit represented by <see cref="UnitInfo" />
    /// </summary>
    /// <param name="unitInfo">Building unit "id"</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
    /// <returns>Returns an instance or null if null is registered as an unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public object BuildUnit(UnitInfo unitInfo, BuildPlansCollection auxBuildPlans = null) => 
      Build(unitInfo, auxBuildPlans, BuildSession.BuildUnit).Value;

    /// <summary>
    ///   Builds all units represented by <see cref="UnitInfo" />
    /// </summary>
    /// <param name="unitInfo">Building unit "id"</param>
    /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
    /// <returns>Returns an instance or null if null is registered as an unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public IReadOnlyList<object> BuildAllUnits(UnitInfo unitInfo, BuildPlansCollection auxBuildPlans = null)
    {
      var buildResult = Build(unitInfo, auxBuildPlans, BuildSession.BuildAllUnits);
      return buildResult.Select(_ => _.Value).ToArray();
    }
    
    private T Build<T>(UnitInfo unitInfo, BuildPlansCollection auxBuildPlans, Func<UnitInfo, IEnumerable<object>, BuildPlansCollection, BuildPlansCollection, T> build)
    {
      var buildResult = build(unitInfo, _stages, this, auxBuildPlans);
      if (buildResult != null)
        return buildResult;

      if (_parentBuilders != null)
        foreach (var parentBuilder in _parentBuilders)
          try
          {
            return parentBuilder.Build(unitInfo, auxBuildPlans, build);
          }
          catch (Exception)
          {
            // continue
          }

      throw new ArmatureException("Can't build unit").AddData("unitInfo", unitInfo);
    }
  }
}