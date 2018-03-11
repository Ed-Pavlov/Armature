using System;
using System.Diagnostics;
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
      Id = id;
      Token = token;
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

    [DebuggerStepThrough]
    public static bool operator ==(UnitInfo left, UnitInfo right) => Equals(left, right);

    [DebuggerStepThrough]
    public static bool operator !=(UnitInfo left, UnitInfo right) => !Equals(left, right);

    public override string ToString() => string.Format("[Id={0}, Token={1}]", Id, Token ?? "null");
  }
}