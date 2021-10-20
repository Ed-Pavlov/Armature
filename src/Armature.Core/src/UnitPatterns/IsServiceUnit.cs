using Armature.Core.Logging;

namespace Armature.Core
{
  public record IsServiceUnit : IUnitPattern
  {
    public bool Matches(UnitId unitId) => unitId.Key is SpecialKey;

    public override string ToString() => GetType().GetShortName();
  }
}
