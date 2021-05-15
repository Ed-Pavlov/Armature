namespace Armature.Core
{
  public class IsServiceUnit : IUnitPattern
  {
    public bool Matches(UnitId unitId) => unitId.Key is SpecialKey;
  }
}
