using System;
using Armature.Core;

namespace Armature.Extensibility
{
  public class UnitMatcherExtensibility : IUnitMatcherExtensibility
  {
    protected readonly IUnitMatcher UnitMatcher;
    protected readonly int Weight;

    public UnitMatcherExtensibility(IUnitMatcher unitMatcher, int weight)
    {
      UnitMatcher = unitMatcher ?? throw new ArgumentNullException(nameof(unitMatcher));
      Weight = weight;
    }

    IUnitMatcher IUnitMatcherExtensibility.UnitMatcher => UnitMatcher;
    int IUnitMatcherExtensibility.Weight => Weight;
  }
}