using System;
using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Base class for unit info matchers operating with a <see cref="Type"/> of an unit
  /// </summary>
  public abstract class UnitInfoByTypeMatcherBase : UnitInfoMatcherBase
  {
    /// <summary>
    /// The type of the Unit to match
    /// </summary>
    protected readonly Type UnitType;

    protected UnitInfoByTypeMatcherBase([NotNull]Type type, [CanBeNull] object token) : base(token) => 
      UnitType = type ?? throw new ArgumentNullException(nameof(type));
    
    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}:{1}", UnitType.ToLogString(), Token.ToLogString());

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher obj)
    {
      if (obj is not UnitInfoByTypeMatcherBase other) return false;
      if (obj.GetType() != this.GetType()) return false;

      return UnitType == other.UnitType && Equals(Token, other.Token);
    }

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode()
    {
      unchecked
      {
        return (UnitType.GetHashCode() * 397) ^ (Token != null ? Token.GetHashCode() : 0);
      }
    }
  }
}