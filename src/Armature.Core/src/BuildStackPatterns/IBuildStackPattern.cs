using System;

namespace Armature.Core;

/// <summary>
/// A pattern used to match a unit or their combination in the build stack. See <see cref="BuildSession.Stack"/> for details.
/// </summary>
public interface IBuildStackPattern : IEquatable<IBuildStackPattern>, IStaticPattern, ILogString
{
  /// <summary>
  /// Adds a <paramref name="node" /> as a child node if the node is not already added. Returns the new node, or the existing node if the node already added.
  /// </summary>
  /// <remarks>Call it first and then fill returned <see cref="IBuildStackPattern" /> with build actions or perform other needed actions due to
  /// it can return other instance of <see cref="IBuildStackPattern"/> then passed <paramref name="node"/>.</remarks>
  T GetOrAddNode<T>(T node) where T : IBuildStackPattern;

  /// <summary>
  /// Adds the <paramref name="node" /> as a child node.
  /// </summary>
  /// <exception cref="ArmatureException">A node is already in the tree.</exception>
  T AddNode<T>(T node) where T : IBuildStackPattern;

  /// <summary>
  /// Adds a <see cref="IBuildAction" /> which will be called to build a Target Unit matched by the branch of the build stack pattern tree represented
  /// by this node with its parents.
  /// </summary>
  /// <param name="buildAction">A build action.</param>
  /// <param name="buildStage">A build stage in which the build action is executed.</param>
  /// <returns>Returns true if build action was added, false if the equal build action is already in the collection.</returns>
  bool AddBuildAction(IBuildAction buildAction, object buildStage);

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