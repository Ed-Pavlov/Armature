using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Matches that a building unit is an argument to inject into the parameter with a specified name
  /// </summary>
  public record IsParameterWithNameMatcher : IsInjectPointWithNameMatcher
  {
    [DebuggerStepThrough]
    public IsParameterWithNameMatcher(string name) : base(name) { }

    protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as ParameterInfo)?.Name;
  }
}
