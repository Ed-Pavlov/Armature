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
  /// See <see cref="BuildStackPatternExtension.UseBuildAction"/> for details.</param>
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

  /// <inheritdoc />
  public BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null)
    => new BuildSession(_buildStages, this, auxBuildStackPatternTree, _parentBuilders).BuildUnit(unitId);

  /// <inheritdoc />
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null)
    => new BuildSession(_buildStages, this, auxBuildStackPatternTree, _parentBuilders).BuildAllUnits(unitId);
}
