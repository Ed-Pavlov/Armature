using System.Reflection;
using Armature.Core;

namespace Armature;

/// <summary>
/// Checks if a unit is an argument to inject into the property with a specified name
/// </summary>
public record IsPropertyNamed : InjectPointNamedBase
{
  public IsPropertyNamed(string propertyName) : base(propertyName) { }

  protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as PropertyInfo)?.Name;
}
