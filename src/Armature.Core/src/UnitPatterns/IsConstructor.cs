using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is a constructor.
/// </summary>
public record IsConstructor : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == SpecialTag.Constructor;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsConstructor);
}
