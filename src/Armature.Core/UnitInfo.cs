using System;
using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Describes an unit to build. <see cref="IUnitSequenceMatcher" /> matches with passed collection of <see cref="UnitInfo" />
  /// </summary>
  [Serializable]
  public class UnitInfo : IEquatable<UnitInfo>
  {
    public readonly object Id;
    public readonly object Token;

    [DebuggerStepThrough]
    public UnitInfo([CanBeNull] object id, [CanBeNull] object token)
    {
      if (id == null && token == null) throw new ArgumentNullException(nameof(id), @"Either id or token should be provided");

      Id = id;
      Token = token;
    }

    /// <summary>
    /// Matching, unlike equality, takes into consideration <see cref="Armature.Core.Token.Any"/>. Use <see cref="Equals(UnitInfo)"/>
    /// to add build plans and <see cref="Matches"/> to build a unit
    /// </summary>
    public bool Matches(UnitInfo other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;

      return Equals(Id, other.Id) && (Equals(Token, other.Token) || Equals(Token, Core.Token.Any) || Equals(Core.Token.Any, other.Token));
    }
    
    [DebuggerStepThrough]
    public bool Equals(UnitInfo other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;

      return Equals(Id, other.Id) && Equals(Token, other.Token);
    }

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as UnitInfo);

    [DebuggerStepThrough]
    public override int GetHashCode()
    {
      unchecked
      {
        return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Token != null ? Token.GetHashCode() : 0);
      }
    }

    public override string ToString() => string.Format("{0}, {1}", Id.ToLogString(), Token.ToLogString());
  }
}