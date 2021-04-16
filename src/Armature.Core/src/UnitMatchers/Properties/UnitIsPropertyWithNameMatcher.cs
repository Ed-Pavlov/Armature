using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches property by name
  /// </summary>
  public record UnitIsPropertyWithNameMatcher : UnitIsInjectPointWithNameMatcherBase
  {
    public UnitIsPropertyWithNameMatcher(string propertyName) : base(propertyName) { }

    protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as PropertyInfo)?.Name;
  }
}
