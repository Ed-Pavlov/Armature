using System;
using System.Diagnostics;
using Armature.Core.Logging;


namespace Armature.Core
{
  /// <summary>
  ///   Describes a unit to build. <see cref="IUnitSequenceMatcher" /> matches with passed collection of <see cref="UnitInfo" />
  /// </summary>
  [Serializable]
  public readonly struct UnitInfo
  {
    public readonly object? Id;
    public readonly object? Token;

    [DebuggerStepThrough]
    public UnitInfo(object? id, object? token)
    {
      if(id is null && token is null) throw new ArgumentNullException(nameof(id), @"Either id or token should be provided");

      Id    = id;
      Token = token;
    }

    [DebuggerStepThrough]
    public bool Equals(UnitInfo other) => Equals(Id, other.Id) && Equals(Token, other.Token);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => obj is UnitInfo other && Equals(other.Id, Id) && Equals(other.Token, Token);

    [DebuggerStepThrough]
    public override int GetHashCode()
    {
      unchecked
      {
        return ((Id is not null ? Id.GetHashCode() : 0) * 397) ^ (Token is not null ? Token.GetHashCode() : 0);
      }
    }

    public override string ToString() => string.Format("{0}({1}, {2})", nameof(UnitInfo), Id.ToLogString(), Token.ToLogString());
  }
}
