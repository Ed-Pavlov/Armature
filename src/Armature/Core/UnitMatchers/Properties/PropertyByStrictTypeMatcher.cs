using System;
using System.Diagnostics;
using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Properties
{
  public class PropertyByStrictTypeMatcher : InjectPointByStrictTypeMatcher
  {
    [DebuggerStepThrough]
    public PropertyByStrictTypeMatcher([NotNull] Type propertyType) : base(propertyType){}

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.PropertyType;
  }
}