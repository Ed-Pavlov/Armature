using System;
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
      if (buildPlan == null) throw new ArgumentNullException("buildPlan");

      var newSequenceMatcher = new WeakUnitSequenceMatcher(Match.Type<T>(token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      var oldSequenceMatcher = buildPlan.Children.Single(_ => _.Equals(newSequenceMatcher));

      buildPlan.Children.Remove(oldSequenceMatcher);
      
      return new OverrideSugar<T>(buildPlan.AddOrGetUnitMatcher(newSequenceMatcher));
    }
  }
  
  public class OverrideSugar<T> : TreatSugar<T>
  {
    public OverrideSugar(WeakUnitSequenceMatcher sequenceMatcher) : base(sequenceMatcher)
    {}
  }
}