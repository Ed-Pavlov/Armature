using System.Diagnostics;
using Armature.Core;
using Armature.Core.UnitMatchers;
using JetBrains.Annotations;

namespace Tests.Common
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
    public static IUnitMatcher Type<T>([CanBeNull]object key) => new UnitInfoMatcher(new UnitId(typeof(T), key));
  }
}
