﻿using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Properties
{
  public class PropertyByNameMatcher : InjectPointByNameMatcher
  {
    public PropertyByNameMatcher([NotNull] string propertyName) : base(propertyName){}

    protected override string GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.Name;
  }
}