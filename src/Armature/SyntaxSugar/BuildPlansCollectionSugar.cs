using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public static class BuildPlansCollectionSugar
  {
    public static TreatSugar<T> Treat<T>([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var unitSequenceMatcher = new WeakUnitSequenceMatcher(Match.Type<T>(token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      return new TreatSugar<T>(container.AddOrGetUnitMatcher(unitSequenceMatcher));
    }

    public static TreatOpenGenericSugar TreatOpenGeneric([NotNull] this BuildPlansCollection container, Type openGenericType, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var buildStep = new WeakUnitSequenceMatcher(Match.OpenGenericType(openGenericType, token), UnitSequenceMatchingWeight.WeakMatchingOpenGenericUnit);
      return new TreatOpenGenericSugar(container.AddOrGetUnitMatcher(buildStep), container);
    }

    public static AdjusterSugar TreatAll([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var buildStep = new AnyUnitSequenceMatcher();
      return new AdjusterSugar(container.AddOrGetUnitMatcher(buildStep));
    }

    public static BuildingSugar Building<T>(this BuildPlansCollection container, object token = null) => container.Building(typeof(T), token);

    public static BuildingSugar Building([NotNull] this BuildPlansCollection container, Type type, object token = null)
    {
      if (container == null) throw new ArgumentNullException(nameof(container));

      var buildStep = new WeakUnitSequenceMatcher(Match.Type(type, token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      return new BuildingSugar(container.AddOrGetUnitMatcher(buildStep));
    }
  }
}