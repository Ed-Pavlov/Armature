using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers.UnitType
{
  /// <summary>
  /// Base class for unit info matchers operating with a <see cref="Type"/> of an unit
  /// </summary>
  public abstract record UnitByTypeMatcherBase
  {
    /// <summary>
    /// The type of the Unit to match
    /// </summary>
    protected readonly Type UnitType;

    protected readonly object? Key;

    protected UnitByTypeMatcherBase(Type type, object? key)
    {
      UnitType = type ?? throw new ArgumentNullException(nameof(type));
      Key    = key;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}:{1}", UnitType.ToLogString(), Key.ToLogString());
  }
}
