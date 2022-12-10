using System;
using Armature.Core;

namespace Armature;

public static class BuildStackPatternTreeExtension
{
  /// <inheritdoc cref="ISubjectTuner.Treat"/>
  public static IBuildingTuner<object?> Treat(this BuildStackPatternTree tree, Type type, object? tag = null)
    => SubjectTuner.Treat(new RootTuner(tree), type, tag);

  /// <inheritdoc cref="ISubjectTuner.Treat{T}"/>
  public static IBuildingTuner<T> Treat<T>(this BuildStackPatternTree tree, object? tag = null) => SubjectTuner.Treat<T>(new RootTuner(tree), tag);

  /// <inheritdoc cref="ISubjectTuner.TreatOpenGeneric"/>
  public static IBuildingTuner<object?> TreatOpenGeneric(this BuildStackPatternTree tree, Type openGenericType, object? tag = null)
    => SubjectTuner.TreatOpenGeneric(new RootTuner(tree), openGenericType, tag);

  /// <inheritdoc cref="ISubjectTuner.TreatInheritorsOf"/>
  public static IBuildingTuner<object?> TreatInheritorsOf(this BuildStackPatternTree tree, Type baseType, object? tag = null)
    => SubjectTuner.TreatInheritorsOf(new RootTuner(tree), baseType, tag);

  /// <inheritdoc cref="ISubjectTuner.TreatInheritorsOf{T}"/>
  public static IBuildingTuner<T> TreatInheritorsOf<T>(this BuildStackPatternTree tree, object? tag = null)
    => SubjectTuner.TreatInheritorsOf<T>(new RootTuner(tree), tag);

  /// <inheritdoc cref="ISubjectTuner.Building"/>
  public static ISubjectTuner Building(this BuildStackPatternTree tree, Type type, object? tag = null) => SubjectTuner.Building(new RootTuner(tree), type, tag);

  /// <inheritdoc cref="ISubjectTuner.Building{T}"/>
  public static ISubjectTuner Building<T>(this BuildStackPatternTree tree, object? tag = null) => SubjectTuner.Building(new RootTuner(tree), typeof(T), tag);

  // /// <summary>
  // /// Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems.
  // /// </summary>
  // public static BuildingTuner TreatOverride(this BuildStackPatternTree tree, Type type, object? tag = null)
  // {
  //   if(pattern is null) throw new ArgumentNullException(nameof(pattern));
  //   if(type is null) throw new ArgumentNullException(nameof(type));
  //
  //   var newPatternMatcher = new SkipTillUnit(new UnitPattern(type, tag), WeightOf.BuildStackPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);
  //   var oldPatternMatcher = pattern.Children.Single(_ => _.Equals(newPatternMatcher));
  //
  //   pattern.Children.Remove(oldPatternMatcher);
  //   return new BuildingTuner(pattern.AddNode(newPatternMatcher));
  // }
  //
  // /// <summary>
  // /// Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems.
  // /// </summary>
  // public static BuildingTuner<T> TreatOverride<T>(this BuildStackPatternTree tree, object? tag = null)
  // {
  //   if(pattern is null) throw new ArgumentNullException(nameof(pattern));
  //
  //   var newPatternMatcher = new SkipTillUnit(new UnitPattern(typeof(T), tag), WeightOf.BuildStackPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);
  //   var oldPatternMatcher = pattern.Children.Single(_ => _.Equals(newPatternMatcher));
  //
  //   pattern.Children.Remove(oldPatternMatcher);
  //   return new BuildingTuner<T>(pattern.AddNode(newPatternMatcher));
  // }
}
