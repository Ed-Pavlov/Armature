namespace Armature.Core
{
  /// <summary>
  ///   Matches Unit representing "constructor" of the currently building Unit
  /// </summary>
  public record IsConstructorPattern : SimpleToStringImpl, IUnitIdPattern
  {
    public static readonly IUnitIdPattern Instance = new IsConstructorPattern();

    private IsConstructorPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Constructor && unitId.GetUnitTypeSafe() is not null;
  }
}
