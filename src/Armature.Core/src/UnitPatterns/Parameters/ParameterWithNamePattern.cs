using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument to inject into the parameter with the specified name.
  /// </summary>
  public record ParameterWithNamePattern : InjectPointWithNamePattern
  {
    [DebuggerStepThrough]
    public ParameterWithNamePattern(string name) : base(name) { }

    protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as ParameterInfo)?.Name;
  }
}
