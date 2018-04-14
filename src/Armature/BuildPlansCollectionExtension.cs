using System;
using System.Linq;
using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public static class BuildPlansCollectionExtension
  {
    public static TreatingTuner<T> Treat<T>([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      return new TreatingTuner<T>(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
    
    public static TreatingTuner<T> Override<T>([NotNull] this BuildPlansCollection buildPlan, object token = null)
    {
      if (buildPlan == null) throw new ArgumentNullException(nameof(buildPlan));

      var newSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      var oldSequenceMatcher = buildPlan.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlan.Children.Remove(oldSequenceMatcher);

      return new TreatingTuner<T>(buildPlan.AddOrGetUnitSequenceMatcher(newSequenceMatcher));
    }

    public static TreatingOpenGenericTuner TreatOpenGeneric([NotNull] this BuildPlansCollection container, Type openGenericType, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.OpenGenericType(openGenericType, token), UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);
      return new TreatingOpenGenericTuner(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    public static Tuner TreatAll([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new AnyUnitSequenceMatcher();
      return new Tuner(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    public static SequenceTuner Building<T>(this BuildPlansCollection container, object token = null) => container.Building(typeof(T), token);

    public static SequenceTuner Building([NotNull] this BuildPlansCollection container, Type type, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new SequenceTuner(container.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
  }
}