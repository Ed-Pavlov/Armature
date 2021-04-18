using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches Unit representing "value for property" of the currently building Unit
  /// </summary>
  public record IsPropertyArgumentPattern : SimpleToStringImpl, IUnitIdPattern
  {
    public static readonly IUnitIdPattern Instance = new IsPropertyArgumentPattern();

    private IsPropertyArgumentPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is PropertyInfo;
  }
}
