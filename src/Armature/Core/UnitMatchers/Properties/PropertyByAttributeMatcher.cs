using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Properties
{
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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