using System.Diagnostics;
using Armature.Core;
using JetBrains.Annotations;

namespace Tests.Common
{
  /// <summary>
  ///   Utility class to simplify the reading of the code creates different <see cref="IUnitPattern" />s.
  /// </summary>
  public static class Match
  {
    /// <summary>
    ///   Creates a type matcher with <see cref="UnitId" />(typeof(<typeparamref name="T" />), <paramref name="key" />)
    /// </summary>
    [DebuggerStepThrough]
    public static IUnitPattern Type<T>(object? key) => new Pattern(typeof(T), key);
  }
}