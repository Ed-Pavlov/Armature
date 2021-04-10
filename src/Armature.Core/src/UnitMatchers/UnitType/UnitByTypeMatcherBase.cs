using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers.UnitType
{
  /// <summary>
  /// Base class for unit info matchers operating with a <see cref="Type"/> of an unit
  /// </summary>
  public abstract record UnitInfoByTypeMatcherBase
  {
    /// <summary>
    /// The type of the Unit to match
    /// </summary>
    protected readonly Type UnitType;
    protected readonly object? Token;

    protected UnitInfoByTypeMatcherBase(Type type, object? token)
    {
      UnitType = type ?? throw new ArgumentNullException(nameof(type));
      Token   = token;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}:{1}", UnitType.ToLogString(), Token.ToLogString());
  }
}
