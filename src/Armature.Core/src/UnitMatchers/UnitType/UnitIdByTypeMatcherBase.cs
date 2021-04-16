using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for matchers matching unit with a <see cref="System.Type"/> pattern
  /// </summary>
  public abstract record UnitIdByTypeMatcherBase
  {
    protected readonly Type    Type;
    protected readonly object? Key;

    protected UnitIdByTypeMatcherBase(Type type, object? key)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      Key  = key;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}:{1}", Type.ToLogString(), Key.ToLogString());
  }
}
