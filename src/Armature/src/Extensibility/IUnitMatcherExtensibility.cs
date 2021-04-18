using Armature.Core;

namespace Armature.Extensibility
{
  public interface IUnitMatcherExtensibility
  {
    IUnitPattern UnitPattern { get; }
    int            Weight      { get; }
  }
}
