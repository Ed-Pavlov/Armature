using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public class BuildingOpenGenericTuner : BuildingTuner<object?>
{
  [DebuggerStepThrough]
  public BuildingOpenGenericTuner(ITunerInternal parent, CreateNode createNode, IUnitPattern unitPattern, short weight = 0)
    : base(parent, createNode, unitPattern, weight) { }

  /// <summary>
  /// Build an object of the specified <paramref name="openGenericType"/> instead.
  /// </summary>
  public override ICreationTuner As(Type openGenericType, object? tag = null)
  {
    this.BuildBranch().UseBuildAction(new RedirectOpenGenericType(openGenericType, tag), BuildStage.Create);

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.OpenGenericPattern + WeightOf.BuildChainPattern.TargetUnit);
    return new BuildingOpenGenericTuner(this, CreateNode, unitPattern);
  }
}
