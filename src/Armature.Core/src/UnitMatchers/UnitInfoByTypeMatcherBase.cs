using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Base class for unit info matchers operating with a <see cref="Type"/> of an unit
  /// </summary>
  public abstract record UnitInfoByTypeMatcherBase : UnitInfoMatcherBase
  {
    /// <summary>
    /// The type of the Unit to match
    /// </summary>
    protected readonly Type UnitType;

    protected UnitInfoByTypeMatcherBase(Type type, object? token) : base(token) => 
      UnitType = type ?? throw new ArgumentNullException(nameof(type));
    
    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}:{1}", UnitType.ToLogString(), Token.ToLogString());
  }
}