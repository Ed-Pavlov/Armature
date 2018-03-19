using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Properties
{
  public class PropertyByValueMatcher : InjectPointByValueMatcher
  {
    [DebuggerStepThrough]
    public PropertyByValueMatcher([NotNull] object value) : base(value){}

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.PropertyType;
  }
}