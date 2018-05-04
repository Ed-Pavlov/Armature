using System;
using System.Linq;
using Armature.Core;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public static class BuildPlansCollectionExtension
  {
    /// <summary>
    /// Used to make a build plan for <typeparamref name="T"/>.
    /// How <typeparamref name="T"/> should be treated is specified by subsequence calls using returned object. 
    /// </summary>
    public static TreatingTuner<T> Treat<T>([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      return new TreatingTuner<T>(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
    
    /// <summary>
    /// Used to make a build plan for <typeparamref name="T"/>.
    /// How <typeparamref name="T"/> should be treated is specified by subsequence calls using returned object. 
    /// </summary>
    public static TreatingTuner<T> TreatInheritorsOf<T>([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(
        new BaseTypeMatcher(new UnitInfo(typeof(T), token)),
        UnitSequenceMatchingWeight.WildcardMatchingBaseTypeUnit);
      return new TreatingTuner<T>(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    } 
    
    /// <summary>
    /// Used to override a build plan for <typeparamref name="T"/>
    /// How <typeparamref name="T"/> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public static TreatingTuner<T> Override<T>([NotNull] this BuildPlansCollection buildPlan, object token = null)
    {
      if (buildPlan == null) throw new ArgumentNullException(nameof(buildPlan));

      var newSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      var oldSequenceMatcher = buildPlan.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlan.Children.Remove(oldSequenceMatcher);

      return new TreatingTuner<T>(buildPlan.AddOrGetUnitSequenceMatcher(newSequenceMatcher));
    }

    /// <summary>
    /// Used to make a build plan for whole class of open generic types.
    /// How <paramref name="openGenericType"/> should be treated is specified by subsequence calls using returned object. 
    /// </summary>
    public static TreatingOpenGenericTuner TreatOpenGeneric([NotNull] this BuildPlansCollection container, Type openGenericType, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(
        Match.OpenGenericType(openGenericType, token),
        UnitSequenceMatchingWeight.WildcardMatchingOpenGenericUnit);
      return new TreatingOpenGenericTuner(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    /// Used to add some details to build plan of any building unit. E.g. to specify what constructor to use, or register a dependency needed by any type
    /// in the system. Usually used as a part of other build plan. See <see cref="Building{T}"/> for details.  
    /// </summary>
    public static Tuner TreatAll([NotNull] this BuildPlansCollection container)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new AnyUnitSequenceMatcher();
      return new Tuner(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    /// Used to make a build plan for a unit only if it is building in a context of building <typeparamref name="T"/>.
    /// </summary>
    public static SequenceTuner Building<T>(this BuildPlansCollection container, object token = null) => container.Building(typeof(T), token);

    /// <summary>
    /// Used to make a build plan for a unit only if it is building in a context of building <paramref name="type"/>.
    /// </summary>
    public static SequenceTuner Building([NotNull] this BuildPlansCollection container, Type type, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new SequenceTuner(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
  }
}