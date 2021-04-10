namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches any type which can be instantiated
  /// </summary>
  public record AnyTypeMatcher : UnitMatcherBase, IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyTypeMatcher();

    private AnyTypeMatcher() { }

    public bool Matches(UnitId unitId)
    {
      var type = unitId.GetUnitTypeSafe();
      return !unitId.Key.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }
  }
}
