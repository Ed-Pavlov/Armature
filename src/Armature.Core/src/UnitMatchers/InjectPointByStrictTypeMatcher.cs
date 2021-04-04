using System;
using System.Diagnostics;
using Armature.Core.Logging;


namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" by exact type matching
  /// </summary>
  public abstract class InjectPointByStrictTypeMatcher : IUnitMatcher
  {
    private readonly Type _type;

    [DebuggerStepThrough]
    protected InjectPointByStrictTypeMatcher(Type parameterType) => _type = parameterType ?? throw new ArgumentNullException(nameof(parameterType));

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && GetInjectPointType(unitInfo) == _type;

    protected abstract Type? GetInjectPointType(UnitInfo unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type.ToLogString());

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher? obj) => obj is InjectPointByStrictTypeMatcher other && obj.GetType() == GetType() && _type == other._type;

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _type.GetHashCode();
    #endregion
  }
}