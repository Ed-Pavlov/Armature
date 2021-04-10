using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches Unit representing "value for parameter" for the currently building Unit
  /// </summary>
  public record ParameterValueMatcher : UnitMatcherBase, IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ParameterValueMatcher();

    private ParameterValueMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is ParameterInfo;
  }
}
