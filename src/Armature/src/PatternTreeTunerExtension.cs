using System;
using System.Linq;
using Armature.Core;

namespace Armature
{
  public static class PatternTreeTunerExtension
  {
    /// <summary>
    ///   Configure build plans for the unit representing by <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner Treat(this IPatternTreeNode buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var patternMatcher = new FindUnitMatches(new Pattern(type, key));
      return new TreatingTuner(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for the unit representing by type <typeparamref name="T"/>
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> Treat<T>(this IPatternTreeNode buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var patternMatcher = new FindUnitMatches(new Pattern(typeof(T), key));
      return new TreatingTuner<T>(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for the unit representing by <paramref name="unitId"/>
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner Treat(this IPatternTreeNode buildPlans, UnitId unitId)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var patternMatcher = new FindUnitMatches(new Pattern(unitId.Kind, unitId.Key));
      return new TreatingTuner(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for all inheritors of <paramref name="baseType"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner TreatInheritorsOf(this IPatternTreeNode buildPlans, Type baseType, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var patternMatcher = new FindUnitMatches(new SubtypePattern(baseType, key), QueryWeight.WildcardMatchingBaseTypeUnit);
      return new TreatingTuner(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for all inheritors of <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> TreatInheritorsOf<T>(this IPatternTreeNode buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var patternMatcher = new FindUnitMatches(new SubtypePattern(typeof(T), key), QueryWeight.WildcardMatchingBaseTypeUnit);
      return new TreatingTuner<T>(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems. 
    /// </summary>
    public static TreatingTuner TreatOverride(this IPatternTreeNode buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var newPatternMatcher = new FindUnitMatches(new Pattern(type, key));
      var oldPatternMatcher = buildPlans.Children.Single(_ => _.Equals(newPatternMatcher));

      buildPlans.Children.Remove(oldPatternMatcher);
      return new TreatingTuner(buildPlans.AddNode(newPatternMatcher));
    }

    /// <summary>
    ///   Overrides a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems. 
    /// </summary>
    public static TreatingTuner<T> OverrideTreat<T>(this IPatternTreeNode buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var newPatternMatcher = new FindUnitMatches(new Pattern(typeof(T), key));
      var oldPatternMatcher = buildPlans.Children.Single(_ => _.Equals(newPatternMatcher));

      buildPlans.Children.Remove(oldPatternMatcher);
      return new TreatingTuner<T>(buildPlans.AddNode(newPatternMatcher));
    }

    /// <summary>
    ///   Configure build plans for whole class of open generic types.
    ///   How <paramref name="openGenericType" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingOpenGenericTuner TreatOpenGeneric(this IPatternTreeNode buildPlans, Type openGenericType, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var patternMatcher = new FindUnitMatches(new OpenGenericTypePattern(openGenericType, key), QueryWeight.WildcardMatchingOpenGenericUnit);
      return new TreatingOpenGenericTuner(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for any unit building in context of the unit.
    ///   See <see cref="BuildSession"/> for details.
    /// </summary>
    public static Tuner TreatAll(this IPatternTreeNode buildPlans)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var patternMatcher = new SkipToLastUnit();
      return new Tuner(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for the unit representing by <paramref name="type"/>.
    /// </summary>
    public static SequenceTuner Building(this IPatternTreeNode buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var patternMatcher = new FindUnitMatches(new Pattern(type, key));
      return new SequenceTuner(buildPlans.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for the unit representing by type <typeparamref name="T"/>
    /// </summary>
    public static SequenceTuner Building<T>(this IPatternTreeNode buildPlans, object? key = null) => buildPlans.Building(typeof(T), key);
  }
}
