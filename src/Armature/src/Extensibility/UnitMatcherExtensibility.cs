using Armature.Core;

namespace Armature.Extensibility
{
  public class UnitMatcherExtensibility : IUnitMatcherExtensibility
  {
    protected readonly IUnitIdMatcher UnitMatcher;
    protected readonly int            Weight;

    public UnitMatcherExtensibility(IUnitIdMatcher unitMatcher, int weight)
    {
      UnitMatcher = unitMatcher;
      Weight      = weight;
    }

    IUnitIdMatcher IUnitMatcherExtensibility.UnitMatcher => UnitMatcher;
    int IUnitMatcherExtensibility.           Weight      => Weight;
  }
}
