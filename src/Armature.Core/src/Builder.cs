using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core.Sdk;


namespace Armature.Core;

/// <summary>
/// The builder of units. It is the convenient way to couple a build stack pattern tree, (<see cref="BuildStackPatternTree" />),
/// build stages, and parent builders together to pass into a <see cref="BuildSession" />.
/// </summary>
public class Builder : BuildStackPatternTree, IBuilder
{
  private readonly object[]    _buildStages;
  private readonly IBuilder[]? _parentBuilders;

  public Builder() => throw new ArgumentException("Provide stages");

  /// <param name="buildStages">The ordered collection of build stages all of which are performed to build a unit.</param>
  public Builder(params object[] buildStages) : this(buildStages, null)
  {
  }

  /// <param name="buildStages">The ordered collection of build stages all of which are performed to build a unit.
  /// See <see cref="BuildStackPatternTreeExtension.UseBuildAction"/> for details.</param>
  /// <param name="parentBuilders">
  /// If unit is not built and <paramref name="parentBuilders" /> are provided, tries to build an unit using
  /// parent builders one by one in the order they passed into the constructor.
  /// </param>
  public Builder(object[] buildStages, params IBuilder[]? parentBuilders)
  {
    _buildStages = buildStages ?? throw new ArgumentNullException(nameof(buildStages));
    if(buildStages is null) throw new ArgumentNullException(nameof(buildStages));
    if(buildStages.Length == 0) throw new ArgumentException("Should contain at least one build stage", nameof(buildStages));
    if(buildStages.Any(stage => stage is null)) throw new ArgumentException("Should not contain null values", nameof(buildStages));
    if(buildStages.Length != buildStages.Distinct().Count()) throw new ArgumentException("Should not contain duplicate values", nameof(buildStages));
    if(parentBuilders?.Any(_ => _ is null) == true) throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

    _parentBuilders = parentBuilders is {Length: > 0} ? parentBuilders : null;
  }

  /// <summary>
  /// Builds an unit represented by <see cref="UnitId" />
  /// </summary>
  /// <param name="unitId">The id of the unit to build.</param>
  /// <param name="auxBuildStackPatternTree">Additional build stack pattern tree containing build actions to build a unit or its dependencies.</param>
  /// <returns>Returns build result with <see cref="BuildResult.HasValue"/> set to false if unit is not built.</returns>
  public BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null)
    => new BuildSession(_buildStages, this, auxBuildStackPatternTree, _parentBuilders).BuildUnit(unitId);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">The id of the unit to build.</param>
  /// <param name="auxBuildStackPatternTree">Additional build stack pattern tree containing build actions to build a unit or its dependencies.</param>
  /// <returns>Returns <see cref="Empty{BuildResult}.List"/> if no units were built. </returns>
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null)
    => new BuildSession(_buildStages, this, auxBuildStackPatternTree, _parentBuilders).BuildAllUnits(unitId);
}
