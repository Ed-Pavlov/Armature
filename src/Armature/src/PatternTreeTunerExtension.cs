using System;
using Armature.Core;

namespace Armature;

public static class PatternTreeTunerExtension
{
  public static IBuildingTuner<object?> Treat(this BuildChainPatternTree tree, Type type, object? tag = null)
    => SubjectTuner.Treat(new RootTuner(tree), type, tag);

  public static IBuildingTuner<T> Treat<T>(this BuildChainPatternTree tree, object? tag = null) => SubjectTuner.Treat<T>(new RootTuner(tree), tag);

  public static IBuildingTuner<object?> TreatOpenGeneric(this BuildChainPatternTree tree, Type openGenericType, object? tag = null)
    => SubjectTuner.TreatOpenGeneric(new RootTuner(tree), openGenericType, tag);

  public static IBuildingTuner<object?> TreatInheritorsOf(this BuildChainPatternTree tree, Type baseType, object? tag = null)
    => SubjectTuner.TreatInheritorsOf(new RootTuner(tree), baseType, tag);

  public static IBuildingTuner<T> TreatInheritorsOf<T>(this BuildChainPatternTree tree, object? tag = null)
    => SubjectTuner.TreatInheritorsOf<T>(new RootTuner(tree), tag);

  public static ISubjectTuner Building(this BuildChainPatternTree tree, Type type, object? tag = null) => SubjectTuner.Building(new RootTuner(tree), type, tag);

  public static ISubjectTuner Building<T>(this BuildChainPatternTree tree, object? tag = null) => SubjectTuner.Building(new RootTuner(tree), typeof(T), tag);

  // /// <summary>
  // /// Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems.
  // /// </summary>
  // public static BuildingTuner TreatOverride(this BuildChainPatternTree tree, Type type, object? tag = null)
  // {
  //   if(pattern is null) throw new ArgumentNullException(nameof(pattern));
  //   if(type is null) throw new ArgumentNullException(nameof(type));
  //
  //   var newPatternMatcher = new SkipTillUnit(new UnitPattern(type, tag), WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);
  //   var oldPatternMatcher = pattern.Children.Single(_ => _.Equals(newPatternMatcher));
  //
  //   pattern.Children.Remove(oldPatternMatcher);
  //   return new BuildingTuner(pattern.AddNode(newPatternMatcher));
  // }
  //
  // /// <summary>
  // /// Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems.
  // /// </summary>
  // public static BuildingTuner<T> TreatOverride<T>(this BuildChainPatternTree tree, object? tag = null)
  // {
  //   if(pattern is null) throw new ArgumentNullException(nameof(pattern));
  //
  //   var newPatternMatcher = new SkipTillUnit(new UnitPattern(typeof(T), tag), WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);
  //   var oldPatternMatcher = pattern.Children.Single(_ => _.Equals(newPatternMatcher));
  //
  //   pattern.Children.Remove(oldPatternMatcher);
  //   return new BuildingTuner<T>(pattern.AddNode(newPatternMatcher));
  // }
}