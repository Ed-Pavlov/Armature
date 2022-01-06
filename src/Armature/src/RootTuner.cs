using System;
using System.Diagnostics;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature;

public class RootTuner : TunerBase
{
  [PublicAPI]
  protected int Weight;

  [DebuggerStepThrough]
  public RootTuner(IBuildChainPattern parentNode) : base(parentNode) { }

  public RootTuner AmendWeight(short weight)
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

    var patternMatcher = new SkipTillUnit(new UnitPattern(type, tag), Weight + WeightOf.BuildChainPattern.SkipTillUnit);
    return new RootTuner(ParentNode.GetOrAddNode(patternMatcher));
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

    var patternMatcher = new SkipTillUnit(
      new UnitPattern(type, tag),
      Weight + WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);

    return new TreatingTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  /// Add build actions to build a unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  public TreatingTuner<T> Treat<T>(object? tag = null)
    => new(
      ParentNode.GetOrAddNode(
        new SkipTillUnit(
          new UnitPattern(typeof(T), tag),
          Weight
        + WeightOf.BuildChainPattern.TargetUnit
        + WeightOf.UnitPattern.ExactTypePattern)));

  /// <summary>
  /// Add build actions applied all generic types match the generic type definition specified by <paramref name="openGenericType"/> in subsequence calls.
  /// </summary>
  public TreatingOpenGenericTuner TreatOpenGeneric(Type openGenericType, object? tag = null)
  {
    var patternMatcher = new SkipTillUnit(
      new IsGenericOfDefinition(openGenericType, tag),
      Weight + WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.OpenGenericPattern);

    return new TreatingOpenGenericTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  /// Add build actions applied to all inheritors of <paramref name="baseType"/> in subsequence calls.
  /// </summary>
  public TreatingTuner TreatInheritorsOf(Type baseType, object? tag = null)
  {
    var patternMatcher = new SkipTillUnit(
      new IsInheritorOf(baseType, tag),
      Weight + WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.SubtypePattern);

    return new TreatingTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  /// Add build actions applied to all inheritors of <typeparamref name="T"/> in subsequence calls.
  /// </summary>
  public TreatingTuner<T> TreatInheritorsOf<T>(object? tag = null)
  {
    var patternMatcher = new SkipTillUnit(
      new IsInheritorOf(typeof(T), tag),
      Weight
    + WeightOf.BuildChainPattern.TargetUnit
    + WeightOf.UnitPattern.SubtypePattern);

    return new TreatingTuner<T>(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  /// Add build action applied to any building unit in subsequence calls. It's needed to setup common build actions like which constructor to call or
  /// inject dependencies into properties or not.
  /// </summary>
  public FinalTuner TreatAll() => new(ParentNode.GetOrAddNode(new SkipAllUnits(Weight + WeightOf.BuildChainPattern.SkipAllUnits)));
}