using Armature.Core;

namespace Armature.Extensibility
{
  public interface IUnitMatcherExtensibility
  {
    IUnitIdMatcher UnitMatcher { get; }
    int          Weight      { get; }
  }
}
