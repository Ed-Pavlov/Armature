using System;
using System.Linq;
using Armature.Core;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.UnitType;
using Armature.Core.UnitSequenceMatcher;

namespace Armature
{
  public static class BuildPlansCollectionExtension
  {
    /// <summary>
    ///   Used to make a build plan for Unit of type <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner Treat(this BuildPlansCollection buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(new UnitId(type, key)));
      return new TreatingTuner(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for Unit of type <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> Treat<T>(this BuildPlansCollection buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(typeof(T), key));
      return new TreatingTuner<T>(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for <paramref name="unitId"/>
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner Treat(this BuildPlansCollection buildPlans, UnitId unitId)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(unitId));
      return new TreatingTuner(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for all inheritors of <paramref name="baseType"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner TreatInheritorsOf(this BuildPlansCollection buildPlans, Type baseType, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new SkipToUnit(
        new UnitIsSubTypeOfMatcher(baseType, key),
        UnitSequenceMatchingWeight.WildcardMatchingBaseTypeUnit);

      return new TreatingTuner(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for all inheritors of <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> TreatInheritorsOf<T>(this BuildPlansCollection buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new SkipToUnit(
        new UnitIsSubTypeOfMatcher(typeof(T), key),
        UnitSequenceMatchingWeight.WildcardMatchingBaseTypeUnit);

      return new TreatingTuner<T>(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to override a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems. 
    /// </summary>
    public static TreatingTuner TreatOverride(this BuildPlansCollection buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var newSequenceMatcher = new SkipToUnit(new UnitIdMatcher(type, key));
      var oldSequenceMatcher = buildPlans.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlans.Children.Remove(oldSequenceMatcher);
      return new TreatingTuner(buildPlans.AddItem(newSequenceMatcher));
    }

    [Obsolete("Renamed to OverrideTreat, use it instead. Will be deleted in future releases.")]
    public static TreatingTuner<T> Override<T>(this BuildPlansCollection buildPlans, object? key = null) => OverrideTreat<T>(buildPlans, key);

    /// <summary>
    ///   Used to override a previously registered <see cref="Treat{T}"/>. Mostly used in test environment to use mocks instead of real subsystems. 
    /// </summary>
    public static TreatingTuner<T> OverrideTreat<T>(this BuildPlansCollection buildPlans, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var newSequenceMatcher = new SkipToUnit(new UnitIdMatcher(typeof(T), key));
      var oldSequenceMatcher = buildPlans.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlans.Children.Remove(oldSequenceMatcher);
      return new TreatingTuner<T>(buildPlans.AddItem(newSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for whole class of open generic types.
    ///   How <paramref name="openGenericType" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingOpenGenericTuner TreatOpenGeneric(this BuildPlansCollection buildPlans, Type openGenericType, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new SkipToUnit(
        new UnitKindIsOpenGenericTypeMatcher(openGenericType, key),
        UnitSequenceMatchingWeight.WildcardMatchingOpenGenericUnit);

      return new TreatingOpenGenericTuner(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to add some details to build plan of any building unit. E.g. to specify what constructor to use, or register a dependency needed by any type
    ///   in the system. Usually used as a part of other build plan. See <see cref="Building{T}" /> for details.
    /// </summary>
    public static Tuner TreatAll(this BuildPlansCollection buildPlans)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new SkipToLastUnit();
      return new Tuner(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <paramref name="type" />.
    /// </summary>
    public static SequenceTuner Building(this BuildPlansCollection buildPlans, Type type, object? key = null)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(type, key));
      return new SequenceTuner(buildPlans.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <typeparamref name="T" />.
    /// </summary>
    public static SequenceTuner Building<T>(this BuildPlansCollection buildPlans, object? key = null) => buildPlans.Building(typeof(T), key);
  }
}
