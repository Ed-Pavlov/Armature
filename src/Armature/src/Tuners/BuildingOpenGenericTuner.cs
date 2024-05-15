using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

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
    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.OpenGenericPattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
    return new BuildingOpenGenericTuner(this, CreateNode, unitPattern);
  }
}