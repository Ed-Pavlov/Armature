using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Properties
{
  public class PropertyByStrictTypeMatcher : IUnitMatcher
  {
    private readonly Type _type;

    [DebuggerStepThrough]
    public PropertyByStrictTypeMatcher([NotNull] Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

    public bool Matches(UnitInfo unitInfo) =>
      unitInfo.Id is PropertyInfo propertyInfo && unitInfo.Token == SpecialToken.InjectValue && propertyInfo.PropertyType == _type;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type.AsLogString());

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is PropertyByStrictTypeMatcher matcher && _type == matcher._type;

    public override bool Equals(object obj) => Equals(obj as PropertyByStrictTypeMatcher);

    public override int GetHashCode() => _type.GetHashCode();
    #endregion
  }
}