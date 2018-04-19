using System;
using System.Diagnostics;
using Resharper.Annotations;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Base class for matchers matching an "inject point" suited for provided value type
  /// </summary>
  public abstract class InjectPointByValueMatcher : IUnitMatcher
  {
    private readonly object _value;

    [DebuggerStepThrough]
    protected InjectPointByValueMatcher([NotNull] object parameterValue) => _value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));

    [CanBeNull]
    protected abstract Type GetInjectPointType(UnitInfo unitInfo);
    
    public bool Matches(UnitInfo unitInfo)
    {
      var type = GetInjectPointType(unitInfo);
      return unitInfo.Token == SpecialToken.InjectValue &&  type != null && type.IsInstanceOfType(_value);
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _value.AsLogString());

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher obj) => obj is InjectPointByValueMatcher other && GetType() == obj.GetType() && Equals(_value, other._value);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _value.GetHashCode();
    #endregion
  }
}