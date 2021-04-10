namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches Unit representing "property" of the currently building Unit
  /// </summary>
  public record PropertyMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new PropertyMatcher();

    private PropertyMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Property && unitId.GetUnitTypeSafe() is not null;
  }
}
