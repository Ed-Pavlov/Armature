﻿using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

public record IsServiceUnit : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag is SpecialTag;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsServiceUnit);
}
