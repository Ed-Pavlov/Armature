using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Properties
{
  public class PropertyByStrictTypeMatcher : InjectPointByStrictTypeMatcher
  {
    [DebuggerStepThrough]
    public PropertyByStrictTypeMatcher([NotNull] Type propertyType) : base(propertyType){}

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.PropertyType;
  }
}