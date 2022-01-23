using System;
using Armature.Core;

namespace Armature;

public static class PatternTreeTunerExtension
{
  public static TreatingTuner Treat(this IBuildChainPattern pattern, Type type, object? tag = null) => new RootTuner(pattern).Treat(type, tag);

  public static TreatingTuner<T> Treat<T>(this IBuildChainPattern pattern, object? tag = null) => new RootTuner(pattern).Treat<T>(tag);

  public static TreatingOpenGenericTuner TreatOpenGeneric(this IBuildChainPattern pattern, Type openGenericType, object? tag = null)
    => new RootTuner(pattern).TreatOpenGeneric(openGenericType, tag);

  public static TreatingTuner TreatInheritorsOf(this IBuildChainPattern pattern, Type baseType, object? tag = null)
    => new RootTuner(pattern).TreatInheritorsOf(baseType, tag);

  public static TreatingTuner<T> TreatInheritorsOf<T>(this IBuildChainPattern pattern, object? tag = null) => new RootTuner(pattern).TreatInheritorsOf<T>(tag);

  public static DependencyTuner TreatAll(this IBuildChainPattern pattern) => new RootTuner(pattern).TreatAll();

  public static RootTuner Building(this IBuildChainPattern pattern, Type type, object? tag = null) => new RootTuner(pattern).Building(type, tag);

  public static RootTuner Building<T>(this IBuildChainPattern pattern, object? tag = null) => new RootTuner(pattern).Building<T>(tag);

  // /// <summary>
  // /// Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems.
  // /// </summary>
  // public static TreatingTuner TreatOverride(this IBuildChainPattern pattern, Type type, object? tag = null)
  // {
  //   if(pattern is null) throw new ArgumentNullException(nameof(pattern));
  //   if(type is null) throw new ArgumentNullException(nameof(type));
  //
  //   var newPatternMatcher = new SkipTillUnit(new UnitPattern(type, tag), WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);
  //   var oldPatternMatcher = pattern.Children.Single(_ => _.Equals(newPatternMatcher));
  //
  //   pattern.Children.Remove(oldPatternMatcher);
  //   return new TreatingTuner(pattern.AddNode(newPatternMatcher));
  // }
  //
  // /// <summary>
  // /// Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems.
  // /// </summary>
  // public static TreatingTuner<T> TreatOverride<T>(this IBuildChainPattern pattern, object? tag = null)
  // {
  //   if(pattern is null) throw new ArgumentNullException(nameof(pattern));
  //
  //   var newPatternMatcher = new SkipTillUnit(new UnitPattern(typeof(T), tag), WeightOf.BuildChainPattern.TargetUnit + WeightOf.UnitPattern.ExactTypePattern);
  //   var oldPatternMatcher = pattern.Children.Single(_ => _.Equals(newPatternMatcher));
  //
  //   pattern.Children.Remove(oldPatternMatcher);
  //   return new TreatingTuner<T>(pattern.AddNode(newPatternMatcher));
  // }
}