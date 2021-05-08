using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is the list of properties of a type to inject dependencies.
  /// </summary>
  public record IsPropertyList : IUnitPattern
  {
    public static readonly IUnitPattern Instance = new IsPropertyList();

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.PropertyList && unitId.GetUnitTypeSafe() is not null;
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
