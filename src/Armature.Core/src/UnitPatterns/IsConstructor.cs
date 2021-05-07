using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is a constructor needed to build some other unit.
  /// </summary>
  public record IsConstructor : IUnitPattern
  {
    public static readonly IUnitPattern Instance = new IsConstructor();

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Constructor && unitId.GetUnitTypeSafe() is not null;
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
