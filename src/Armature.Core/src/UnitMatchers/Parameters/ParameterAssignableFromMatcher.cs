using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if a building unit is an argument to inject into method or constructor
  /// </summary>
  public record ParameterAssignableFromMatcher : InjectPointAssignableFromMatcher
  {
    public ParameterAssignableFromMatcher(Type type) : base(type) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
