using System.Diagnostics;
using System.Reflection;
using Armature.Core.Sdk;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a method parameter.
  /// </summary>
  public record IsParameterInfoList : IUnitPattern
  {
    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Argument && unitId.Kind is ParameterInfo[];

    [DebuggerStepThrough]
    public override string ToString() => nameof(IsParameterInfoList);
  }
}