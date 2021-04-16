using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches Unit representing "value for parameter" for the currently building Unit
  /// </summary>
  public record IsParameterMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new IsParameterMatcher();

    private IsParameterMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is ParameterInfo;
  }
}
