namespace Armature.Core
{
  /// <summary>
  ///   Matches any type which can be instantiated
  /// </summary>
  public record UnitKindIsTypeMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new UnitKindIsTypeMatcher();

    private UnitKindIsTypeMatcher() { }

    public bool Matches(UnitId unitId)
    {
      var type = unitId.GetUnitTypeSafe();
      return !unitId.Key.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }
  }
}
