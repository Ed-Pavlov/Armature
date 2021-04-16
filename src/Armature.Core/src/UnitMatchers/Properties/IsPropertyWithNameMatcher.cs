using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Matches that a building unit is an argument to inject into the property with a specified name
  /// </summary>
  public record IsPropertyWithNameMatcher : IsInjectPointWithNameMatcher
  {
    public IsPropertyWithNameMatcher(string propertyName) : base(propertyName) { }

    protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as PropertyInfo)?.Name;
  }
}
