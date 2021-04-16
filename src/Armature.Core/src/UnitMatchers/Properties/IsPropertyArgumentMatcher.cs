using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches Unit representing "value for property" of the currently building Unit
  /// </summary>
  public record IsPropertyArgumentMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new IsPropertyArgumentMatcher();

    private IsPropertyArgumentMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && unitId.Kind is PropertyInfo;
  }
}
