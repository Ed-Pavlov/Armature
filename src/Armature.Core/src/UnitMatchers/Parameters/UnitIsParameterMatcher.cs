using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches Unit representing "value for parameter" for the currently building Unit
  /// </summary>
  public record UnitIsParameterMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new UnitIsParameterMatcher();

    private UnitIsParameterMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is ParameterInfo;
  }
}
