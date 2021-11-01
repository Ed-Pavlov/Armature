using System.Diagnostics;

namespace Armature.Core
{
  public record IsServiceUnit : IUnitPattern
  {
    public bool Matches(UnitId unitId) => unitId.Key is SpecialKey;

    [DebuggerStepThrough]
    public override string ToString() => nameof(IsServiceUnit);
  }
}