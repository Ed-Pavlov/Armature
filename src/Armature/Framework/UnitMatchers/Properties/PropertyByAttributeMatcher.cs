using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Properties
{
  public class PropertyByAttributeMatcher<T> : InjectPointByAttributeMatcher<T>
    where T : Attribute
  {
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public PropertyByAttributeMatcher([CanBeNull] Predicate<T> predicate) : base(predicate){}

    protected override T GetInjectPointAttribute(UnitInfo unitInfo) => GetPropertyAttribute(unitInfo);
    
    public static T GetPropertyAttribute(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is PropertyInfo propertyInfo)) return default;
      return propertyInfo.GetCustomAttribute<T>();
    }
  }
}