using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is the list of properties of a type to inject dependencies.
  /// </summary>
  public record PropertiesListPattern : IUnitPattern
  {
    public static readonly IUnitPattern Instance = new PropertiesListPattern();

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.PropertiesList && unitId.GetUnitTypeSafe() is not null;
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
