using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a method parameter.
  /// </summary>
  public record MethodArgumentListPattern : IUnitPattern
  {
    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Argument && unitId.Kind is MethodBase;
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
