using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if a building unit is a constructor
/// </summary>
public record IsConstructor : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Constructor;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsConstructor);
}