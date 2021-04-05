using System.Reflection;


namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property by name
  /// </summary>
  public class PropertyByNameMatcher : InjectPointByNameMatcher
  {
    public PropertyByNameMatcher(string propertyName) : base(propertyName) { }

    protected override string? GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.Name;
  }
}
