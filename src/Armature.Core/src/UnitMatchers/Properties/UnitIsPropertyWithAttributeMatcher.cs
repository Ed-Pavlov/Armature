using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches property marked with attribute which satisfies user provided conditions
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record UnitIsPropertyWithAttributeMatcher<T> : UnitByAttributeMatcherBase<T> where T : Attribute
  {
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public UnitIsPropertyWithAttributeMatcher(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetAttribute(UnitId unitId) => GetPropertyAttribute(unitId);

    public static T? GetPropertyAttribute(UnitId unitId) => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttribute<T>() : default;
  }
}
