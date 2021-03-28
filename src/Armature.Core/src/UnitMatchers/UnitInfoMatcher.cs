using System.Diagnostics;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Matches <see cref="UnitInfo" /> with a pattern.
  /// </summary>
  public sealed record  UnitInfoMatcher : UnitInfoMatcherBase, IUnitMatcher
  {
    private readonly UnitInfo _unitInfo;

    [DebuggerStepThrough]
    public UnitInfoMatcher(object? id, object? token) : this(new UnitInfo(id, token)){}
    
    [DebuggerStepThrough]
    private UnitInfoMatcher(UnitInfo unitInfo) : base(unitInfo.Token) => _unitInfo = unitInfo;

    public bool Matches(UnitInfo unitInfo) => Equals(_unitInfo.Id, unitInfo.Id) && MatchesToken(unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => _unitInfo.ToString();
  }
}