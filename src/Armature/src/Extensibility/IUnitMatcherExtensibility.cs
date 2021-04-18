using Armature.Core;

namespace Armature.Extensibility
{
  public interface IUnitMatcherExtensibility
  {
    IUnitIdPattern UnitPattern { get; }
    int            Weight      { get; }
  }
}
