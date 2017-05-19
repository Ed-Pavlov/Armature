using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// The "dependency injection builder" contains the set of build plans and method <see cref="BuildUnit"/>
  ///
  /// Building a unit it goes over all "build stages" for each stage it get a build step if any and executes it <see cref=".ctor"/> for details
  /// </summary>
  public class Builder : BuildPlansCollection
  {
    private readonly Builder _parentBuilder;
    private readonly IEnumerable<object> _stages;

    /// <param name="stages">The ordered collection of build stages all of which are performed to build a unit</param>
    /// <param name="parentBuilder">If unit is not built and parentBuilder is set, trying to build a unit using parent builder. <see cref="BuildUnit"/></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Builder([NotNull] IEnumerable<object> stages, [CanBeNull] Builder parentBuilder)
    {
      if (stages == null) throw new ArgumentNullException("stages");

      var array = stages.ToArray();
      if(array.Length == 0)
        throw new ArgumentException("empty", "stages");

      _stages = array;
      _parentBuilder = parentBuilder;
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

      if(_parentBuilder == null)
        throw new ArmatureException("Can't build unit").AddData("unitInfo", unitInfo);

      return _parentBuilder.BuildUnit(unitInfo, sessionRules);
    }
  }
}