using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner : IBuildingTuner, ITreatAllTuner, ITuner
{
  protected readonly ITuner?    Parent;
  protected readonly IBuildChainPattern TreeRoot;
  private readonly   CreateNode         _createNode;

  //TODO: short or int?
  protected short Weight;

  [DebuggerStepThrough]
  public BuildingTuner(ITuner parent, CreateNode createNode, short weight = 0)
  {
    Parent      = parent;
    TreeRoot    = parent.TreeRoot;
    _createNode = createNode ?? throw new ArgumentNullException(nameof(createNode));
  }

  protected T AmendWeight<T>(short delta, T inheritor)
  {
    Weight += delta;
    return inheritor;
  }

  public IBuildingTuner AmendWeight(short delta) => AmendWeight(delta, this);

  public IBuildingTuner Building(Type type, object? tag = null) => Building(this, type, tag, Weight);

  public IBuildingTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  public IBuildingTuner<object?> Treat(Type type, object? tag = null) => Treat(this, type, tag, Weight);

  public IBuildingTuner<T> Treat<T>(object? tag = null) => Treat<T>(this, tag, Weight);

  public IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null)
  {
    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.OpenGenericPattern;

    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit);

    return new BuildingOpenGenericTuner(this, CreateNode, unitPattern);
  }

  public IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null) => TreatInheritorsOf(this, baseType, tag, Weight);

  public IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null) => TreatInheritorsOf<T>(this, tag, Weight);

  public ITreatAllTuner TreatAll() => this;

  ITreatAllTuner IDependencyTuner<ITreatAllTuner>.AmendWeight(short delta) => AmendWeight<ITreatAllTuner>(delta, this);

  public ITreatAllTuner UsingArguments(params object[] arguments)
  {
    DependencyTuner.UsingArguments(this, arguments);
    return this;
  }

  public ITreatAllTuner UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints)
  {
    DependencyTuner.UsingInjectionPoints(this, injectionPoints);
    return this;
  }

  public ITreatAllTuner Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  ITuner? ITuner.Parent => Parent;

  IBuildChainPattern ITuner.TreeRoot                                => TreeRoot;
  IBuildChainPattern ITuner.GetOrAddNodeTo(IBuildChainPattern node) => node.GetOrAddNode(_createNode());
  int ITuner.               Weight                                  => Weight;
}
