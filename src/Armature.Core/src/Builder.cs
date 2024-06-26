﻿using System;
using System.Collections.Generic;
using System.Linq;
using BeatyBit.Armature.Core.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature.Core;

/// <summary>
/// The builder of units. It is a convenient way to couple a build stack pattern tree, (<see cref="BuildStackPatternTree" />),
/// build stages, and parent builders together to pass into a <see cref="BuildSession" />.
///
/// But you can use your own implementation of <see cref="IBuilder"/>.
/// </summary>
public class Builder : BuildStackPatternTree, IBuilder
{
  private readonly object[]    _buildStages;
  private readonly IBuilder[]? _parentBuilders;

  private string? _hoconString;

  [PublicAPI]
  public Builder() : base("Error") => throw new ArgumentException("Provide stages");

  /// <param name="name">Used in logs and for debugging.</param>
  /// <param name="buildStages">The ordered collection of build stages, all of which are performed to build a unit.</param>
  public Builder(string name, params object[] buildStages) : this(name, buildStages, null)
  {
  }

  /// <param name="name">Used in logs and for debugging.</param>
  /// <param name="buildStages">The ordered collection of build stages, all of which are performed to build a unit.
  /// See <see cref="BuildStackPatternExtension.UseBuildAction"/> for details.</param>
  /// <param name="parentBuilders">
  /// If a unit is not built and <paramref name="parentBuilders" /> are provided, tries to build a unit using
  /// parent builders one by one in the order they passed into the constructor.
  /// </param>
  public Builder(string name, object[] buildStages, params IBuilder[]? parentBuilders) : base(name)
  {
    Name         = name        ?? throw new ArgumentNullException(nameof(name));
    _buildStages = buildStages ?? throw new ArgumentNullException(nameof(buildStages));
    if(buildStages is null) throw new ArgumentNullException(nameof(buildStages));
    if(buildStages.Length == 0) throw new ArgumentException("Should contain at least one build stage", nameof(buildStages));
    if(buildStages.Any(stage => stage is null)) throw new ArgumentException("Should not contain null values", nameof(buildStages));
    if(buildStages.Length != buildStages.Distinct().Count()) throw new ArgumentException("Should not contain duplicate values", nameof(buildStages));
    if(parentBuilders?.Any(_ => _ is null) == true) throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

    _parentBuilders = parentBuilders is {Length: > 0} ? parentBuilders : null;
  }

  /// <summary>
  /// Used in logs and for debugging.
  /// </summary>
  public string Name { get; }

  /// <inheritdoc cref="IBuilder" />
  public BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null, bool engageParentBuilders = true)
    => new BuildSession(_buildStages, this, auxBuildStackPatternTree, _parentBuilders).BuildUnit(unitId, engageParentBuilders);

  /// <inheritdoc cref="IBuilder" />
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null, bool engageParentBuilders = true)
    => new BuildSession(_buildStages, this, auxBuildStackPatternTree, _parentBuilders).BuildAllUnits(unitId, engageParentBuilders);

  public override string  ToHoconString() => _hoconString ??= Name.QuoteIfNeeded();
}
