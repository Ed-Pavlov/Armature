using Armature.Core;

namespace Armature.Extensibility
{
  public interface IUnitMatcherExtensibility
  {
    IUnitMatcher UnitMatcher { get; }
    int Weight { get; }
  }
}