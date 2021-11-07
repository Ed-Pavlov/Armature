using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is a constructor of a type
  /// </summary>
  public record IsConstructor : IUnitPattern
  {
    private static readonly CanBeInstantiated CanBeInstantiated = Static<CanBeInstantiated>.Instance;

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Constructor && CanBeInstantiated.Matches(unitId);

    [DebuggerStepThrough]
    public override string ToString() => nameof(IsConstructor);
  }
}