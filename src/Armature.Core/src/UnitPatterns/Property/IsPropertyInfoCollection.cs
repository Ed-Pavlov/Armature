using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is the list of properties of a type to inject dependencies.
/// </summary>
public record IsPropertyInfoCollection : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == SpecialTag.PropertyCollection && unitId.GetUnitTypeSafe() is not null;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsPropertyInfoCollection);
}