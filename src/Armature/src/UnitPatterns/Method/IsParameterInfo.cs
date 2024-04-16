using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Checks if a unit is an argument for a constructor/method parameter.
/// </summary>
public record IsParameterInfo : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Argument && unitId.Kind is ParameterInfo;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsParameterInfo);
}
