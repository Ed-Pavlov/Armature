using System.Reflection;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit to be built is an argument to inject into the property with a specified name
/// </summary>
public record IsPropertyNamed : InjectPointNamedBase
{
  public IsPropertyNamed(string propertyName) : base(propertyName) { }

  protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as PropertyInfo)?.Name;
}
