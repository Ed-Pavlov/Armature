using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Properties
{
  public class PropertyByNameMatcher : IUnitMatcher
  {
    private readonly string _name;

    [DebuggerStepThrough]
    public PropertyByNameMatcher([NotNull] string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public bool Matches(UnitInfo unitInfo) => unitInfo.Id is PropertyInfo propertyInfo && unitInfo.Token == SpecialToken.InjectValue && propertyInfo.Name == _name;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _name);

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is PropertyByNameMatcher matcher && _name == matcher._name;
    
    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as PropertyByNameMatcher);

    public override int GetHashCode() => _name.GetHashCode();
    #endregion
  }
}