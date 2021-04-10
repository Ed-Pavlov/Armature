namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches Unit representing "constructor" of the currently building Unit
  /// </summary>
  public record ConstructorMatcher : UnitMatcherBase, IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialToken.Constructor && unitId.GetUnitTypeSafe() is not null;
  }
}
