using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Logging;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers
{
  public abstract class InjectPointByStrictTypeMatcher : IUnitMatcher
  {
    private readonly Type _type;

    [DebuggerStepThrough]
    protected InjectPointByStrictTypeMatcher([NotNull] Type parameterType) => _type = parameterType ?? throw new ArgumentNullException(nameof(parameterType));

    [CanBeNull]
    protected abstract Type GetInjectPointType(UnitInfo unitInfo);
    
    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && GetInjectPointType(unitInfo) == _type;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type.AsLogString());

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher obj) => obj is InjectPointByStrictTypeMatcher other && obj.GetType() == GetType() && _type == other._type;

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _type.GetHashCode();
    #endregion
  }
}