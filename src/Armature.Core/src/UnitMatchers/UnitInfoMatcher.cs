using System;
using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Matches <see cref="UnitInfo" /> with a pattern.
  /// </summary>
  public class UnitInfoMatcher : IUnitMatcher
  {
    [CanBeNull]
    private readonly object _id;
    [CanBeNull]
    private readonly object _token;

    [DebuggerStepThrough]
    public UnitInfoMatcher([CanBeNull] object id, [CanBeNull] object token)
    {
      if (id == null && token == null) throw new ArgumentNullException(nameof(id), @"Either id or token should be provided");
      
      _id = id;
      _token = token;
    }

    public virtual bool Matches(UnitInfo unitInfo) => Equals(_id, unitInfo.Id) && MatchesToken(unitInfo);

    protected bool MatchesToken(UnitInfo unitInfo) => Equals(_token, unitInfo.Token) || Equals(_token, Token.Any);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.TwoParameterFormat, GetType().GetShortName(), _id.ToLogString(), _token.ToLogString());

    [DebuggerStepThrough]
    public virtual bool Equals(IUnitMatcher obj) => obj is UnitInfoMatcher other && Equals(_id, other._id) && Equals(_token, other._token);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as UnitInfoMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode()
    {
      unchecked
      {
        return ((_id != null ? _id.GetHashCode() : 0) * 397) ^ (_token != null ? _token.GetHashCode() : 0);
      }
    }
  }
}