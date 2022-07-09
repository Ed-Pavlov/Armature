using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner : IBuildingTuner, ITreatAllTuner, ITunerInternal
{
  protected readonly ITunerInternal?    Parent;
  protected readonly IBuildChainPattern TreeRoot;
  private readonly   CreateNode         _createNode;

  //TODO: short or int?
  protected short Weight;

  [DebuggerStepThrough]
  public BuildingTuner(ITunerInternal parent, CreateNode createNode, short weight = 0)
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

  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  public IBuildingTuner AmendWeight(short delta) => AmendWeight(delta, this);

  /// <summary>
  /// Add build actions for units building in the context of unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner Building(Type type, object? tag = null) => Building(this, type, tag, Weight);

  /// <summary>
  /// Add build actions for units building in the context of unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  /// <summary>
  /// Add build actions to build a unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner<object?> Treat(Type type, object? tag = null) => Treat(this, type, tag, Weight);

  /// <summary>
  /// Add build actions to build a unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner<T> Treat<T>(object? tag = null) => Treat<T>(this, tag, Weight);

  /// <summary>
  /// Add build actions applied all generic types match the generic type definition specified by <paramref name="openGenericType"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null)
  {
    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.OpenGenericPattern;

    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit);

    return new BuildingOpenGenericTuner(this, CreateNode, unitPattern);
  }

  /// <summary>
  /// Add build actions applied to all inheritors of <paramref name="baseType"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null) => TreatInheritorsOf(this, baseType, tag, Weight);

  /// <summary>
  /// Add build actions applied to all inheritors of <typeparamref name="T"/> in subsequence calls.
  /// </summary>
  public IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null) => TreatInheritorsOf<T>(this, tag, Weight);

  /// <summary>
  /// Add build action applied to any building unit in subsequence calls. It's needed to setup common build actions like which constructor to call or
  /// inject dependencies into properties or not.
  /// </summary>
  public ITreatAllTuner TreatAll() => this;

  ITreatAllTuner IDependencyTuner<ITreatAllTuner>.AmendWeight(short delta) => AmendWeight<ITreatAllTuner>(delta, this);

  public ITreatAllTuner UsingArguments(params object[] arguments)
  {
    DependencyTuner.UsingArguments(this, arguments);
    return this;
  }

  public ITreatAllTuner InjectInto(params IInjectPointTuner[] propertyIds)
  {
    DependencyTuner.InjectInto(this, propertyIds);
    return this;
  }

  ITunerInternal? ITunerInternal.Parent => Parent;

  IBuildChainPattern ITunerInternal.TreeRoot                                => TreeRoot;
  IBuildChainPattern ITunerInternal.GetOrAddNodeTo(IBuildChainPattern node) => node.GetOrAddNode(_createNode());
  int ITunerInternal.               Weight                                  => Weight;
}
