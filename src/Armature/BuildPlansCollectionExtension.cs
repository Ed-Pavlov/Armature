using System;
using System.Linq;
using Armature.Core;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using JetBrains.Annotations;

namespace Armature
{
  public static class BuildPlansCollectionExtension
  {
    /// <summary>
    ///   Used to make a build plan for Unit of type <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner Treat([NotNull] this BuildPlansCollection buildPlans, [NotNull] Type type, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new TreatingTuner(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
    
    /// <summary>
    ///   Used to make a build plan for Unit of type <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> Treat<T>([NotNull] this BuildPlansCollection buildPlans, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      return new TreatingTuner<T>(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for all inheritors of <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner TreatInheritorsOf([NotNull] this BuildPlansCollection buildPlans, [NotNull] Type type, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(
        new BaseTypeMatcher(new UnitInfo(type, token)),
        UnitSequenceMatchingWeight.WildcardMatchingBaseTypeUnit);
      return new TreatingTuner(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
    
    /// <summary>
    ///   Used to make a build plan for all inheritors of <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> TreatInheritorsOf<T>([NotNull] this BuildPlansCollection buildPlans, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(
        new BaseTypeMatcher(new UnitInfo(typeof(T), token)),
        UnitSequenceMatchingWeight.WildcardMatchingBaseTypeUnit);
      return new TreatingTuner<T>(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to override a build plan for <paramref name="type"/>
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner Override([NotNull] this BuildPlansCollection buildPlans, [NotNull] Type type, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var newSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      var oldSequenceMatcher = buildPlans.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlans.Children.Remove(oldSequenceMatcher);

      return new TreatingTuner(buildPlans.AddOrGetUnitSequenceMatcher(newSequenceMatcher));
    }
    
    /// <summary>
    ///   Used to override a build plan for <typeparamref name="T" />
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> Override<T>([NotNull] this BuildPlansCollection buildPlans, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var newSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      var oldSequenceMatcher = buildPlans.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlans.Children.Remove(oldSequenceMatcher);

      return new TreatingTuner<T>(buildPlans.AddOrGetUnitSequenceMatcher(newSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for whole class of open generic types.
    ///   How <paramref name="openGenericType" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingOpenGenericTuner TreatOpenGeneric([NotNull] this BuildPlansCollection buildPlans, Type openGenericType, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(
        Match.OpenGenericType(openGenericType, token),
        UnitSequenceMatchingWeight.WildcardMatchingOpenGenericUnit);
      return new TreatingOpenGenericTuner(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to add some details to build plan of any building unit. E.g. to specify what constructor to use, or register a dependency needed by any type
    ///   in the system. Usually used as a part of other build plan. See <see cref="Building{T}" /> for details.
    /// </summary>
    public static Tuner TreatAll([NotNull] this BuildPlansCollection buildPlans)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new AnyUnitSequenceMatcher();
      return new Tuner(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <paramref name="type" />.
    /// </summary>
    public static SequenceTuner Building([NotNull] this BuildPlansCollection buildPlans, Type type, object token = null)
    {
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new SequenceTuner(buildPlans.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <typeparamref name="T" />.
    /// </summary>
    public static SequenceTuner Building<T>(this BuildPlansCollection buildPlans, object token = null) => buildPlans.Building(typeof(T), token);
  }
}