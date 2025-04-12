using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BeatyBit.Armature;

/// <summary>
/// Builds arguments for constructor/method parameters one by one in the direct order.
/// </summary>
public sealed record BuildMethodArgumentsInDirectOrder : BuildMethodArgumentsInCustomOrder
{
  public BuildMethodArgumentsInDirectOrder() : base(Reorder) { }

  /// <summary>
  /// Doesn't change the order
  /// </summary>
  private static IEnumerable<Tuple<int, ParameterInfo>> Reorder(ParameterInfo[] parameters) => parameters.Select((t, i) => Tuple.Create(i, t));

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildMethodArgumentsInDirectOrder);
}