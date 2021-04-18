using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches Unit representing "value for parameter" for the currently building Unit
  /// </summary>
  public record IsParameterPattern : SimpleToStringImpl, IUnitIdPattern
  {
    public static readonly IUnitIdPattern Instance = new IsParameterPattern();

    private IsParameterPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is ParameterInfo;
  }
}
