using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BeatyBit.Armature;

/// <summary>
/// Builds arguments for constructor/method parameters one by one in the direct order.
/// </summary>
public sealed record BuildMethodArgumentsInReverseOrder : BuildMethodArgumentsInCustomOrder
{
  public BuildMethodArgumentsInReverseOrder () : base(Reorder) { } // don't change order

  /// <summary>
  /// Doesn't change the order
  /// </summary>
  private static IEnumerable<Tuple<int, ParameterInfo>> Reorder(ParameterInfo[] parameters)
  {
    var count = parameters.Length;
    return parameters.Select((t, i) => Tuple.Create(count - i - 1, t));
  }

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildMethodArgumentsInReverseOrder);
}
