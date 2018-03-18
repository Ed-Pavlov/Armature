using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Properties
{
  public class PropertyByWeakTypeMatcher : IUnitMatcher
  {
    private readonly object _value;

    public PropertyByWeakTypeMatcher([NotNull] object value) => _value = value ?? throw new ArgumentNullException(nameof(value));

    public bool Matches(UnitInfo unitInfo) =>
      unitInfo.Id is PropertyInfo propertyInfo && unitInfo.Token == SpecialToken.InjectValue && propertyInfo.PropertyType.IsInstanceOfType(_value);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _value.AsLogString());
    
    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher obj) => obj is PropertyByWeakTypeMatcher other && Equals(_value, other._value);
    
    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as PropertyByWeakTypeMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _value.GetHashCode();
    #endregion
  }
}