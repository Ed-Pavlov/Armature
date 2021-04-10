namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches Unit representing "constructor" of the currently building Unit
  /// </summary>
  public record ConstructorMatcher : UnitMatcherBase, IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Constructor && unitInfo.GetUnitTypeSafe() is not null;
  }
}
