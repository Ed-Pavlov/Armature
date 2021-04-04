using System;
using System.Diagnostics;
using Armature.Core.Logging;


namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" by name
  /// </summary>
  public abstract class InjectPointByNameMatcher : IUnitMatcher
  {
    private readonly string _name;

    [DebuggerStepThrough]
    protected InjectPointByNameMatcher(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && GetInjectPointName(unitInfo) == _name;

    protected abstract string? GetInjectPointName(UnitInfo unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _name);

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher? obj) => obj is InjectPointByNameMatcher other && GetType() == obj.GetType() && _name == other._name;

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _name.GetHashCode();
    #endregion
  }
}