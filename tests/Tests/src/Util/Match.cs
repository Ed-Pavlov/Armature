using System.Diagnostics;
using Armature.Core;

namespace Tests.Util
{
  /// <summary>
  /// Utility class to simplify the reading of the code creates different <see cref="IUnitPattern" />s.
  /// </summary>
  public static class Match
  {
    /// <summary>
    /// Creates a type matcher with <see cref="UnitId" />(typeof(<typeparamref name="T" />), <paramref name="tag" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitPattern Type<T>(object? tag) => new UnitPattern(typeof(T), tag);
  }
}