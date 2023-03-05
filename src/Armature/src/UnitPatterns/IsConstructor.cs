using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature.UnitPatterns;

/// <summary>
/// Checks if a unit is a constructor.
/// </summary>
public record IsConstructor : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Constructor;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsConstructor);
}
