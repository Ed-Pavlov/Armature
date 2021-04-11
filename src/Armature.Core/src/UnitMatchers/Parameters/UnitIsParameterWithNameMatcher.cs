using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter by name
  /// </summary>
  public record UnitIsParameterWithNameMatcher : UnitIsInjectPointWithNameMatcherBase
  {
    [DebuggerStepThrough]
    public UnitIsParameterWithNameMatcher(string name) : base(name) { }

    protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as ParameterInfo)?.Name;
  }
}
