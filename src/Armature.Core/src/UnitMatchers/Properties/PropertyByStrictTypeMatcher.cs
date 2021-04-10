using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property by exact type matching
  /// </summary>
  public record PropertyByStrictTypeMatcher : InjectPointByStrictTypeMatcher
  {
    [DebuggerStepThrough]
    public PropertyByStrictTypeMatcher(Type propertyType) : base(propertyType) { }

    protected override Type? GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.PropertyType;
  }
}
