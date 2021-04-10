using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;


namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property marked with attribute which satisfies user provided conditions
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record PropertyByAttributeMatcher<T> : InjectPointByAttributeMatcher<T>
    where T : Attribute
  {
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public PropertyByAttributeMatcher(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetInjectPointAttribute(UnitId unitId) => GetPropertyAttribute(unitId);

    public static T? GetPropertyAttribute(UnitId unitId) => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttribute<T>() : default;
  }
}
