using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property suited for provided value type
  /// </summary>
  public record PropertyAcceptsArgumentMatcher : InjectPointAcceptsArgumentMatcher
  {
    [DebuggerStepThrough]
    public PropertyAcceptsArgumentMatcher(object value) : base(value) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
  }
}
