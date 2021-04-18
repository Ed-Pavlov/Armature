using Armature.Core;

namespace Armature.Extensibility
{
  public class UnitMatcherExtensibility : IUnitMatcherExtensibility
  {
    protected readonly IUnitIdPattern UnitPattern;
    protected readonly int            Weight;

    public UnitMatcherExtensibility(IUnitIdPattern unitPattern, int weight)
    {
      UnitPattern = unitPattern;
      Weight      = weight;
    }

    IUnitIdPattern IUnitMatcherExtensibility.UnitPattern => UnitPattern;
    int IUnitMatcherExtensibility.           Weight      => Weight;
  }
}
