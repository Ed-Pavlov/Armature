using System.Reflection;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is an argument to inject into the property with a specified name
/// </summary>
public record IsPropertyNamed : InjectPointWithNamePatternBase
{
  public IsPropertyNamed(string propertyName) : base(propertyName) { }

  protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as PropertyInfo)?.Name;
}