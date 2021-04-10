using System.Reflection;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches Unit representing "value for property" of the currently building Unit
  /// </summary>
  public record PropertyValueMatcher : UnitMatcherBase, IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new PropertyValueMatcher();

    private PropertyValueMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialToken.InjectValue && unitId.Kind is PropertyInfo;
  }
}
