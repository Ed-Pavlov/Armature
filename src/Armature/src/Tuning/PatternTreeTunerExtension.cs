using System;
using System.Linq;
using Armature.Core;

namespace Armature
{
  public static class PatternTreeTunerExtension
  {
    public static TreatingTuner Treat(this IPatternTreeNode buildPlans, Type type, object? key = null) => new RootTuner(buildPlans).Treat(type, key);

    public static TreatingTuner<T> Treat<T>(this IPatternTreeNode buildPlans, object? key = null) => new RootTuner(buildPlans).Treat<T>(key);

    public static TreatingOpenGenericTuner TreatOpenGeneric(this IPatternTreeNode buildPlans, Type openGenericType, object? key = null)
      => new RootTuner(buildPlans).TreatOpenGeneric(openGenericType, key);

    public static TreatingTuner TreatInheritorsOf(this IPatternTreeNode buildPlans, Type baseType, object? key = null)
      => new RootTuner(buildPlans).TreatInheritorsOf(baseType, key);

    public static TreatingTuner<T> TreatInheritorsOf<T>(this IPatternTreeNode buildPlans, object? key = null)
      => new RootTuner(buildPlans).TreatInheritorsOf<T>(key);

    public static FinalTuner TreatAll(this IPatternTreeNode buildPlans) => new RootTuner(buildPlans).TreatAll();

    public static RootTuner Building(this IPatternTreeNode buildPlans, Type type, object? key = null) => new RootTuner(buildPlans).Building(type, key);

    public static RootTuner Building<T>(this IPatternTreeNode buildPlans, object? key = null) => new RootTuner(buildPlans).Building<T>(key);

    /// <summary>
    ///   Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems. 
    /// </summary>
    public static TreatingTuner TreatOverride(this IPatternTreeNode buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var newPatternMatcher = new SkipTillUnit(new Pattern(type, key));
      var oldPatternMatcher = buildPlans.Children.Single(_ => _.Equals(newPatternMatcher));

      buildPlans.Children.Remove(oldPatternMatcher);
      return new TreatingTuner(buildPlans.AddNode(newPatternMatcher));
    }

    /// <summary>
    ///   Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems. 
    /// </summary>
    public static TreatingTuner<T> TreatOverride<T>(this IPatternTreeNode buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var newPatternMatcher = new SkipTillUnit(new Pattern(typeof(T), key));
      var oldPatternMatcher = buildPlans.Children.Single(_ => _.Equals(newPatternMatcher));

      buildPlans.Children.Remove(oldPatternMatcher);
      return new TreatingTuner<T>(buildPlans.AddNode(newPatternMatcher));
    }
  }
}
