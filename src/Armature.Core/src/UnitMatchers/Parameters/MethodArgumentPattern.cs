using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a method parameter.
  /// </summary>
  public record MethodArgumentPattern : SimpleToStringImpl, IUnitPattern
  {
    public static readonly IUnitPattern Instance = new MethodArgumentPattern();

    private MethodArgumentPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is ParameterInfo;
  }
}
