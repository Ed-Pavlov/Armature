using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" by name
  /// </summary>
  public abstract record InjectPointByNameMatcher : IUnitMatcher
  {
    private readonly string _name;

    [DebuggerStepThrough]
    protected InjectPointByNameMatcher(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && GetInjectPointName(unitInfo) == _name;

    protected abstract string? GetInjectPointName(UnitInfo unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _name);
  }
}