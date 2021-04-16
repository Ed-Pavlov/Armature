using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches parameter suited for provided value type
  /// </summary>
  public record ParameterAcceptsArgumentMatcher : InjectPointAcceptsArgumentMatcher
  {
    public ParameterAcceptsArgumentMatcher(object argument) : base(argument) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
