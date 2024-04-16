using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;
using WeightOf = Armature.Sdk.WeightOf;

namespace Armature;

public class BuildingOpenGenericTuner : BuildingTuner<object?>
{
  [DebuggerStepThrough]
  public BuildingOpenGenericTuner(ITuner parent, CreateNode createNode, IUnitPattern unitPattern)
    : base(parent, createNode, unitPattern) { }

  /// <summary>
  /// Build an object of the specified <paramref name="openGenericType"/> instead.
  /// </summary>
  public override ICreationTuner As(Type openGenericType, object? tag = null)
  {
    BuildStackPatternSubtree().UseBuildAction(new RedirectOpenGenericType(openGenericType, tag), BuildStage.Create);

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.OpenGenericPattern + WeightOf.BuildStackPattern.IfFirstUnit);
    return new BuildingOpenGenericTuner(this, CreateNode, unitPattern);
  }
}