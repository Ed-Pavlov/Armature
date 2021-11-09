using Armature.Core;

namespace Armature.Extensibility;

public class UnitMatcherExtensibility : IUnitMatcherExtensibility
{
  protected readonly IUnitPattern UnitPattern;
  protected readonly int          Weight;

  public UnitMatcherExtensibility(IUnitPattern unitPattern, int weight)
  {
    UnitPattern = unitPattern;
    Weight      = weight;
  }

  IUnitPattern IUnitMatcherExtensibility.UnitPattern => UnitPattern;
  int IUnitMatcherExtensibility.         Weight      => Weight;
}