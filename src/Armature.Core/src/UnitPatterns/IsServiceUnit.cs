using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

public record IsServiceUnit : IUnitPattern, ILogString
{
  public bool Matches(UnitId unitId) => unitId.Tag is SpecialTag;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsServiceUnit);

  [DebuggerStepThrough]
  public string ToHoconString() => ToString();
}