using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature;

public class RootTuner : UnitSequenceExtensibility
{
  [PublicAPI] protected int Weight;

  [DebuggerStepThrough]
  public RootTuner(IPatternTreeNode parentNode) : base(parentNode) { }

  public RootTuner AmendWeight(short weight)
  {
    Weight += weight;
    return this;
  }

  /// <summary>
  ///   Configure build plans for the unit representing by <paramref name="type"/>.
  /// </summary>
  public RootTuner Building(Type type, object? key = null)
  {
    if(type is null) throw new ArgumentNullException(nameof(type));

    var patternMatcher = new SkipTillUnit(new UnitPattern(type, key), Weight);
    return new RootTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  ///   Configure build plans for the unit representing by type <typeparamref name="T"/>
  ///   See <see cref="BuildSession"/> for details.
  /// </summary>
  public RootTuner Building<T>(object? key = null) => Building(typeof(T), key);

  /// <summary>
  ///   Configure build plans for Unit of type <paramref name="type"/>.
  ///   How it should be treated is specified by subsequence calls using returned object.
  /// </summary>
  public TreatingTuner Treat(Type type, object? key = null)
  {
    if(type is null) throw new ArgumentNullException(nameof(type));
    if(type.IsGenericTypeDefinition) throw new ArgumentException($"Use {nameof(TreatOpenGeneric)} to setup open generic types.");

    var patternMatcher = new SkipTillUnit(
      new UnitPattern(type, key),
      Weight + WeightOf.BuildingUnitSequencePattern.Neutral + WeightOf.UnitPattern.ExactTypePattern);

    return new TreatingTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  ///   Configure build plans for Unit of type <typeparamref name="T"/>.
  ///   How it should be treated is specified by subsequence calls using returned object.
  /// </summary>
  public TreatingTuner<T> Treat<T>(object? key = null)
    => new(
      ParentNode.GetOrAddNode(
        new SkipTillUnit(
          new UnitPattern(typeof(T), key),
          Weight
        + WeightOf.BuildingUnitSequencePattern.Neutral
        + WeightOf.UnitPattern.ExactTypePattern)));

  /// <summary>
  ///   Configure build plans for whole class of open generic types.
  ///   How <paramref name="openGenericType" /> should be treated is specified by subsequence calls using returned object.
  /// </summary>
  public TreatingOpenGenericTuner TreatOpenGeneric(Type openGenericType, object? key = null)
  {
    var patternMatcher = new SkipTillUnit(
      new IsGenericOfDefinition(openGenericType, key),
      Weight + WeightOf.BuildingUnitSequencePattern.Neutral + WeightOf.UnitPattern.OpenGenericPattern);

    return new TreatingOpenGenericTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  ///   Configure build plans for all inheritors of <paramref name="baseType"/>.
  ///   How it should be treated is specified by subsequence calls using returned object.
  /// </summary>
  public TreatingTuner TreatInheritorsOf(Type baseType, object? key = null)
  {
    var patternMatcher = new SkipTillUnit(
      new IsInheritorOf(baseType, key),
      Weight + WeightOf.BuildingUnitSequencePattern.Neutral + WeightOf.UnitPattern.SubtypePattern);

    return new TreatingTuner(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  ///   Configure build plans for all inheritors of <typeparamref name="T" />.
  ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
  /// </summary>
  public TreatingTuner<T> TreatInheritorsOf<T>(object? key = null)
  {
    var patternMatcher = new SkipTillUnit(
      new IsInheritorOf(typeof(T), key),
      Weight
    + WeightOf.BuildingUnitSequencePattern.Neutral
    + WeightOf.UnitPattern.SubtypePattern);

    return new TreatingTuner<T>(ParentNode.GetOrAddNode(patternMatcher));
  }

  /// <summary>
  ///   Configure build plans for any unit building in context of the unit.
  ///   See <see cref="BuildSession"/> for details.
  /// </summary>
  public FinalTuner TreatAll() => new(ParentNode.GetOrAddNode(new SkipAllUnits(Weight + WeightOf.BuildingUnitSequencePattern.SkipAllUnits)));
}