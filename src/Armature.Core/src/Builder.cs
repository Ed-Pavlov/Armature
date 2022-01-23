using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core.Sdk;


namespace Armature.Core;

/// <summary>
/// The builder of units. It is the convenient way to couple a build chain pattern tree, (<see cref="BuildChainPatternTree" />),
/// build stages, and parent builders together to pass into a <see cref="BuildSession" />, which could be used independently.
/// </summary>
public class Builder : BuildChainPatternTree, IBuilder
{
  private readonly object[]    _buildStages;
  private readonly IBuilder[]? _parentBuilders;

  public Builder() => throw new ArgumentException("Provide stages");

  /// <param name="buildStages">The ordered collection of build stages all of which are performed to build a unit.</param>
  public Builder(params object[] buildStages) : this(buildStages, Empty<Builder>.Array)
  {
    if(buildStages.Length == 0) throw new ArgumentNullException(nameof(buildStages));
  }

  /// <param name="buildStages">The ordered collection of build stages all of which are performed to build a unit.</param>
  /// <param name="parentBuilders">
  /// If unit is not built and <paramref name="parentBuilders" /> are provided, tries to build a unit using
  /// parent builders one by one in the order they passed into the constructor.
  /// </param>
  public Builder(object[] buildStages, params Builder[]? parentBuilders)
  {
    if(buildStages is null) throw new ArgumentNullException(nameof(buildStages));
    if(buildStages.Length == 0) throw new ArgumentException("Should contain at least one build stage", nameof(buildStages));
    if(buildStages.Any(stage => stage is null)) throw new ArgumentException("Should not contain null values", nameof(buildStages));
    if(buildStages.Length != buildStages.Distinct().Count()) throw new ArgumentException("Should not contain duplicate values", nameof(buildStages));
    if(parentBuilders?.Any(_ => _ is null) == true) throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

    _buildStages = buildStages;

    // ReSharper disable once CoVariantArrayConversion
    _parentBuilders = parentBuilders is null || parentBuilders.Length == 0 ? null : parentBuilders;
  }

  /// <summary>
  /// Builds a unit represented by <see cref="UnitId" />
  /// </summary>
  /// <param name="unitId">The id of the unit to build.</param>
  /// <param name="auxPatternTree">Additional build chain pattern tree containing build actions to build a unit or its dependencies.</param>
  /// <returns>Returns build result with <see cref="BuildResult.HasValue"/> set to false if unit is not built.</returns>
  public BuildResult BuildUnit(UnitId unitId, IBuildChainPattern? auxPatternTree = null)
    => new BuildSession(_buildStages, this, auxPatternTree, _parentBuilders).BuildUnit(unitId);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">Building unit "id"</param>
  /// <param name="auxBuildPlans">Additional build plans to build a unit or its dependencies</param>
  /// <returns>Returns <see cref="Empty{BuildResult}.List"/> if no units were built. </returns>
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, IBuildChainPattern? auxBuildPlans = null)
    => new BuildSession(_buildStages, this, auxBuildPlans, _parentBuilders).BuildAllUnits(unitId);
}