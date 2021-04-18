using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument to inject into the property.
  /// </summary>
  public record PropertyArgumentPattern : SimpleToStringImpl, IUnitPattern
  {
    public static readonly IUnitPattern Instance = new PropertyArgumentPattern();

    private PropertyArgumentPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is PropertyInfo;
  }
}
