using System;
using System.Diagnostics;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  /// <summary>
  ///   Utility class to simplify the reading of the code creates different <see cref="IUnitMatcher" />s.
  /// </summary>
  public static class Match
  {
    /// <summary>
    ///   Creates a matcher with <see cref="UnitInfo" />(typeof(<see cref="T" />), <see cref="token" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitMatcher Type<T>(object token) => Type(typeof(T), token);

    /// <summary>
    ///   Creates a matcher with <see cref="UnitInfo" />(<see cref="type" />, <see cref="token" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitMatcher Type([NotNull] Type type, object token)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));

      return new UnitInfoMatcher(new UnitInfo(type, token));
    }

    /// <summary>
    ///   Creates a matcher with <see cref="UnitInfo" />(<see cref="type" />, <see cref="token" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitMatcher OpenGenericType(Type type, object token) => new OpenGenericTypeMatcher(new UnitInfo(type, token));
  }
}