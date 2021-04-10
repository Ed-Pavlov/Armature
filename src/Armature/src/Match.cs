using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.UnitType;

namespace Armature
{
  /// <summary>
  ///   Utility class to simplify the reading of the code creates different <see cref="IUnitMatcher" />s.
  /// </summary>
  public static class Match
  {
    /// <summary>
    ///   Creates a type matcher with <see cref="UnitId" />(typeof(<typeparamref name="T" />), <paramref name="key" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitMatcher Type<T>(object? key)
    {
      Type type = typeof(T);
      if(type is null) throw new ArgumentNullException(nameof(type));

      return new UnitInfoMatcher(new UnitId(type, key));
    }

    /// <summary>
    ///   Creates a open generic type matcher with <see cref="UnitId" />(<paramref name="type" />, <paramref name="key" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitMatcher OpenGenericType(Type type, object? key) => new OpenGenericTypeMatcher(type, key);
  }
}
