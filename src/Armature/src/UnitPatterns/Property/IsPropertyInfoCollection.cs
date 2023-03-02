using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature.UnitPatterns.Property;

/// <summary>
/// Checks if a unit is the list of properties of a type to inject dependencies.
/// </summary>
public record IsPropertyInfoCollection : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.PropertyCollection && unitId.GetUnitTypeSafe() is not null;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsPropertyInfoCollection);
}