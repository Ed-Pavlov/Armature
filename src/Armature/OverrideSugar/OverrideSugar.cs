using System;
using System.Diagnostics;
using System.Linq;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature.OverrideSugar
{
  public static class Ext
  {
    public static OverrideSugar<T> Override<T>([NotNull] this BuildPlansCollection buildPlan, object token = null)
    {
      if (buildPlan == null) throw new ArgumentNullException(nameof(buildPlan));

      var newSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      var oldSequenceMatcher = buildPlan.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlan.Children.Remove(oldSequenceMatcher);

      return new OverrideSugar<T>(buildPlan.AddOrGetUnitMatcher(newSequenceMatcher), buildPlan);
    }
  }

  public class OverrideSugar<T> : TreatSugar<T>
  {
    [DebuggerStepThrough]
    public OverrideSugar(WildcardUnitSequenceMatcher sequenceMatcher, [NotNull] BuildPlansCollection container) : base(sequenceMatcher) { }
  }
}