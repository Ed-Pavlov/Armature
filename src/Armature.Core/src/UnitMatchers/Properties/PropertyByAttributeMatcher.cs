﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;


namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property marked with attribute which satisfies user provided conditions
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class PropertyByAttributeMatcher<T> : InjectPointByAttributeMatcher<T>
    where T : Attribute
  {
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public PropertyByAttributeMatcher(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetInjectPointAttribute(UnitInfo unitInfo) => GetPropertyAttribute(unitInfo);

    public static T? GetPropertyAttribute(UnitInfo unitInfo)
    {
      if(!(unitInfo.Id is PropertyInfo propertyInfo)) return default;

      return propertyInfo.GetCustomAttribute<T>();
    }
  }
}
