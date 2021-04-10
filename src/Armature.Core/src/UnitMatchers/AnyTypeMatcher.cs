namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches any type which can be instantiated
  /// </summary>
  public record AnyTypeMatcher : UnitMatcherBase, IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyTypeMatcher();

    private AnyTypeMatcher() { }

    public bool Matches(UnitInfo unitInfo)
    {
      var type = unitInfo.GetUnitTypeSafe();
      return !unitInfo.Token.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }
  }
}
