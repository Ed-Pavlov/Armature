using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Checks if a unit is a list of arguments for a constructor/method.
/// </summary>
public record IsParameterInfoArray : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Argument && unitId.Kind is ParameterInfo[];

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsParameterInfoArray);
}