using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if a building unit is an argument to inject into method or constructor
  /// </summary>
  public record IsParameterOfTypeMatcher : IsInjectPointOfTypeMatcher
  {
    public IsParameterOfTypeMatcher(Type type, bool exactMatch) : base(type, exactMatch) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
