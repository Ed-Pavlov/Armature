using System;
using System.Collections.Generic;

namespace Armature.Core;

/// <summary>
/// A pattern used to match a unit or their combination in the build stack. See <see cref="BuildSession.Stack"/> for details.
/// </summary>
public interface IBuildStackPattern : IEquatable<IBuildStackPattern>, ILogString
{
  /// <summary>
  /// The collection of all children nodes used to find existing one, add new, or replace one with another.
  /// All nodes with their children are a build stack pattern tree.
  /// </summary>
  HashSet<IBuildStackPattern> Children { get; }

  /// <summary>
  /// The collection of build actions which should be performed to build a unit.
  /// </summary>
  BuildActionBag BuildActions { get; }

  /// <summary>
  /// Returns build actions which should be performed to build a <see cref="BuildSession.Stack.TargetUnit"/>.
  /// </summary>
  /// <param name="stack">
  /// The stack of units representing a build session. Each unit is the dependency of the "deeper" in the stack one.
  /// </param>
  /// <param name="actionBag"></param>
  /// <param name="inputWeight">
  ///   The weight of matching which used by children matchers to calculate a final weight of matching
  /// </param>
  /// <remarks>
  /// IA -> A -> IB -> B. This stack means that for now Unit of type 'B' is the target unit
  /// but it is built in the "context" of the whole build stack.
  /// </remarks>
  /// <returns>
  /// Returns all matched build actions for the <paramref name="stack" />. All actions are grouped by a building stage
  /// and coupled with a "weight of matching". See <see cref="WeightedBuildActionBag" /> for details.
  /// </returns>
  bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight = 0);
}