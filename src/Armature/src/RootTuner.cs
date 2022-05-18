using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public class RootTuner : TunerBase
{
  [DebuggerStepThrough]
  public RootTuner(IBuildChainPattern treeRoot) : base(treeRoot, treeRoot, null, null) { }

  [DebuggerStepThrough]
  public RootTuner(IBuildChainPattern treeRoot, AddContextPatterns getContextNode, IUnitPattern unitPattern) : base(
      treeRoot,
      treeRoot,
      getContextNode,
      unitPattern) { }

  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  public RootTuner AmendWeight(int weight)
  {
    Weight += weight;
    return this;
  }

  /// <summary>
  /// Add build actions for units building in the context of unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public RootTuner Building(Type type, object? tag = null)
  {
    if(type is null) throw new ArgumentNullException(nameof(type));

    var unitPattern = new UnitPattern(type, tag);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node
        .GetOrAddNode(new SkipTillUnit(unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.SkipTillUnit))
        .TryAddContext(ContextFactory);

    return new RootTuner(TreeRoot, AddContextTo, unitPattern);
  }

  /// <summary>
  /// Add build actions for units building in the context of unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public RootTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  /// <summary>
  /// Add build actions to build a unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public TreatingTuner Treat(Type type, object? tag = null)
  {
    if(type is null) throw new ArgumentNullException(nameof(type));
    if(type.IsGenericTypeDefinition) throw new ArgumentException($"Use {nameof(TreatOpenGeneric)} to setup open generic types.");

    var unitPattern = new UnitPattern(type, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.ExactTypePattern;

    var targetUnitNode = TreeRoot.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit)).TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new TreatingTuner(TreeRoot, targetUnitNode, AddContextTo, unitPattern);
  }

  /// <summary>
  /// Add build actions to build a unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public TreatingTuner<T> Treat<T>(object? tag = null)
  {
    var unitPattern = new UnitPattern(typeof(T), tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.ExactTypePattern;

    var targetUnitNode = TreeRoot.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                 .TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new TreatingTuner<T>(TreeRoot, targetUnitNode, AddContextTo, unitPattern);
  }

  /// <summary>
  /// Add build actions applied all generic types match the generic type definition specified by <paramref name="openGenericType"/> in subsequence calls.
  /// </summary>
  public TreatingOpenGenericTuner TreatOpenGeneric(Type openGenericType, object? tag = null)
  {
    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.OpenGenericPattern;

    var targetUnitNode = TreeRoot.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                 .TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new TreatingOpenGenericTuner(TreeRoot, targetUnitNode, AddContextTo, unitPattern);
  }

  /// <summary>
  /// Add build actions applied to all inheritors of <paramref name="baseType"/> in subsequence calls.
  /// </summary>
  public TreatingTuner TreatInheritorsOf(Type baseType, object? tag = null)
  {
    var unitPattern = new IsInheritorOf(baseType, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.SubtypePattern;

    var targetUnitNode = TreeRoot.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                 .TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new TreatingTuner(TreeRoot, targetUnitNode, AddContextTo, unitPattern);
  }

  /// <summary>
  /// Add build actions applied to all inheritors of <typeparamref name="T"/> in subsequence calls.
  /// </summary>
  public TreatingTuner<T> TreatInheritorsOf<T>(object? tag = null)
  {
    var baseWeight  = Weight + WeightOf.UnitPattern.SubtypePattern;
    var unitPattern = new IsInheritorOf(typeof(T), tag);

    var targetUnitNode = TreeRoot.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                 .TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new TreatingTuner<T>(TreeRoot, targetUnitNode, AddContextTo, unitPattern);
  }

  /// <summary>
  /// Add build action applied to any building unit in subsequence calls. It's needed to setup common build actions like which constructor to call or
  /// inject dependencies into properties or not.
  /// </summary>
  public DependencyTuner TreatAll() => new DependencyTuner(TreeRoot, TunedNode, ContextFactory!, UnitPattern);
}