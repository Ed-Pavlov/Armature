using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument to inject into the property.
  /// </summary>
  public record PropertyArgumentPattern : IUnitPattern
  {
    public static readonly IUnitPattern Instance = new PropertyArgumentPattern();

    private PropertyArgumentPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Argument && unitId.Kind is PropertyInfo;
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
