using System;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property suited for provided value type
  /// </summary>
  public class PropertyByValueMatcher : InjectPointByValueMatcher
  {
    [DebuggerStepThrough]
    public PropertyByValueMatcher([NotNull] object value) : base(value) { }

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.PropertyType;
  }
}