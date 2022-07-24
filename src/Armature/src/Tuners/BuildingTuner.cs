using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

public partial class BuildingTuner : IBuildingTuner, ITreatAllTuner, ITuner
{
  private readonly CreateNode _createNode;

  [DebuggerStepThrough]
  [PublicAPI]
  public BuildingTuner(ITuner parent, CreateNode createNode)
  {
    Parent      = parent;
    TreeRoot    = parent.TreeRoot;
    _createNode = createNode ?? throw new ArgumentNullException(nameof(createNode));
  }

  public IBuildingTuner Building(Type type, object? tag = null) => Building(this, type, tag, Weight);

  public IBuildingTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  public IBuildingTuner<object?> Treat(Type type, object? tag = null) => Treat(this, type, tag, Weight);

  public IBuildingTuner<T> Treat<T>(object? tag = null) => Treat<T>(this, tag, Weight);

  public IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null) => TreatOpenGeneric(this, openGenericType, tag, Weight);

  public IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null) => TreatInheritorsOf(this, baseType, tag, Weight);

  public IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null) => TreatInheritorsOf<T>(this, tag, Weight);

  public ITreatAllTuner TreatAll() => this;

  public ITreatAllTuner UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  public ITreatAllTuner UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints) => DependencyTuner.UsingInjectionPoints(this, injectionPoints);

  public ITreatAllTuner Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  IBuildingTuner IBuildingTuner.                  AmendWeight(short delta) => AmendWeight(delta, this);
  ITreatAllTuner IDependencyTuner<ITreatAllTuner>.AmendWeight(short delta) => AmendWeight<ITreatAllTuner>(delta, this);

  protected T AmendWeight<T>(short delta, T inheritor)
  {
    Weight += delta;
    return inheritor;
  }

  public ITuner?            Parent   { get; }
  public IBuildChainPattern TreeRoot { get; }
  public int                Weight   { get; private set; }

  public IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node) => node.GetOrAddNode(_createNode());
}