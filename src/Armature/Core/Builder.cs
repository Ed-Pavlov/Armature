using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// The "dependency injection builder" contains build steps for units and method <see cref="BuildUnit"/>
  ///
  /// Building a unit it goes over all "build stages", for each stage it gets a build step if any and executes it 
  /// <see cref="Builder(IEnumerable{object}, Builder[])"/> for details
  /// </summary>
  public class Builder : BuildPlansCollection
  {
    private readonly Builder[] _parentBuilders;
    private readonly IEnumerable<object> _stages;

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit</param>
    /// <param name="parentBuilders">If unit is not built and parentBuilder is set, trying to build a unit using parent builders one by one
    /// in the order how the were passed into constructor </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Builder([NotNull] IEnumerable<object> stages, params Builder[] parentBuilders)
    {
      if (stages == null) throw new ArgumentNullException("stages");
      if (parentBuilders != null && parentBuilders.Any(_ => _ == null)) throw new ArgumentException("Should not contain null values", "parentBuilders");
      
      var array = stages.ToArray();
      if(array.Length == 0)
        throw new ArgumentException("empty", "stages");

      _stages = array;
      _parentBuilders = parentBuilders == null || parentBuilders.Length == 0 ? null : parentBuilders;
    }

    /// <summary>
    /// Builds a unit represented by <see cref="UnitInfo"/>
    /// </summary>
    /// <returns>Returns an instance of unit or null if unit is null.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    public object BuildUnit(UnitInfo unitInfo, BuildPlansCollection sessionRules = null)
    {
      var buildSession = new BuildSession(_stages, this, sessionRules);

      var buildResult = buildSession.BuildUnit(unitInfo);
      if (buildResult != null)
        return buildResult.Value;

      if (_parentBuilders != null)
        foreach (var parentBuilder in _parentBuilders)
        {
          try
          {
            return parentBuilder.BuildUnit(unitInfo, sessionRules);
          }
          catch (Exception)
          {
            // continue
          }
        }
      
      throw new ArmatureException("Can't build unit").AddData("unitInfo", unitInfo);
    }
  }
}