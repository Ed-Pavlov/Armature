using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Base class implementing <see cref="IBuildStackPattern.AddBuildAction"/>
/// </summary>
public abstract class BuildStackPatternBase : IBuildStackPattern, IEnumerable, ILoggable
{
  [PublicAPI]
  protected BuildActionBag? RawBuildActions;
  protected HashSet<IBuildStackPattern>? RawChildren;

  protected BuildStackPatternBase(int weight) => Weight = weight;

  protected long Weight { [DebuggerStepThrough] get; }

  public BuildActionBag BuildActions => RawBuildActions ??= new BuildActionBag();

  /// <summary>
  /// The collection of all children nodes used to find existing one, add new, or replace one with another.
  /// All nodes with their children are a build stack pattern tree.
  /// </summary>
  public HashSet<IBuildStackPattern> Children => RawChildren ??= new HashSet<IBuildStackPattern>();

  /// <summary>
  /// Adds a <paramref name="node" /> as a child node if the node is not already added. Returns the new node, or the existing node if the node already added.
  /// </summary>
  /// <remarks>Call it first and then fill returned <see cref="IBuildStackPattern" /> with build actions or perform other needed actions due to
  /// it can return other instance of <see cref="IBuildStackPattern"/> then passed <paramref name="node"/>.</remarks>
  public virtual T GetOrAddNode<T>(T node) where T : IBuildStackPattern
  {
    if(node is null) throw new ArgumentNullException(nameof(node));

    if(Children.TryGetValue(node, out var actualNode))
      return (T) actualNode;

    Children.Add(node);
    return node;
  }

  public virtual T AddNode<T>(T node) where T : IBuildStackPattern
  {
    if(node is null) throw new ArgumentNullException(nameof(node));

    if(!Children.Add(node))
      BuildStackPatternExtension.ThrowNodeIsAlreadyAddedException(this, node);

    return node;
  }

  public bool AddBuildAction(IBuildAction buildAction, object buildStage)
  {
    if(!BuildActions.TryGetValue(buildStage, out var list))
    {
      list = new List<IBuildAction>();
      RawBuildActions!.Add(buildStage, list); // RawBuildActions is not null at this point
    }

    if(list.Contains(buildAction)) return false;

    list.Add(buildAction);
    return true;
  }

  public abstract bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight);

  [PublicAPI]
  protected bool GetOwnBuildActions(long inputWeight, out WeightedBuildActionBag? actionBag)
  {
    actionBag = null;
    if(RawBuildActions is null) return false;

    var matchingWeight = inputWeight + Weight;
    actionBag = new WeightedBuildActionBag();

    foreach(var pair in RawBuildActions)
    {
      var actions         = pair.Value;
      var weightedActions = new LeanList<Weighted<IBuildAction>>(actions.Count);

      foreach(var buildAction in actions)
        weightedActions.Add(buildAction.WithWeight(matchingWeight));

      actionBag.Add(pair.Key, weightedActions);
    }

    return true;
  }

  protected bool GetOwnAndChildrenBuildActions(BuildSession.Stack stack, long inputWeight, out WeightedBuildActionBag? actionBag)
  {
    var result = GetOwnBuildActions(inputWeight, out actionBag);
    actionBag.WriteToLog(LogLevel.Verbose, "Actions: ");

    if(RawChildren is not null && stack.Count > 0)
    { // pass the rest of the stack to children and return their actions
      result    |= GetChildrenActions(stack, inputWeight, out var childrenActionBag);
      actionBag =  actionBag.Merge(childrenActionBag);
    }

    return result;
  }

  /// <summary>
  /// Gathers and merges build actions from all children nodes.
  /// </summary>
  /// <param name="stack">The build stack to pass to children nodes if any.</param>
  /// <param name="inputWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
  /// <param name="actionBag"></param>
  [PublicAPI]
  protected bool GetChildrenActions(BuildSession.Stack stack, long inputWeight, out WeightedBuildActionBag? actionBag)
  {
    actionBag = null;

    if(RawChildren is null)
    {
      Log.WriteLine(LogLevel.Trace, "Children: null");
      return false;
    }

    var matchingWeight = inputWeight + Weight;

    using(var condition = Log.UnderCondition(LogLevel.Verbose))
    using(Log.NamedBlock(LogLevel.Verbose, "PassTailToChildren"))
    {
      if(Log.IsEnabled(LogLevel.Verbose))
        Log.WriteLine(LogLevel.Verbose, $"ActualWeight = {matchingWeight.ToHoconString()}, Tail = {stack.ToHoconString()}");

      foreach(var child in RawChildren)
      {
        if(child.GatherBuildActions(stack, out var childBag, matchingWeight))
          actionBag = actionBag.Merge(childBag);
      }

      var hasActions = actionBag is not null;
      condition.IsMet = hasActions;
      return hasActions;
    }
  }

  public virtual bool IsStatic(out UnitId unitId)
  {
    unitId = default;
    return false;
  }

  public virtual void PrintToLog(LogLevel logLevel = LogLevel.None)
  {
    using(Log.NamedBlock(logLevel, ToHoconString))
    {
      Log.WriteLine(LogLevel.Info, $"Weight: {Weight.ToHoconString()}");
      PrintContentToLog(logLevel);
      PrintChildrenToLog(logLevel);
    }
  }

  protected virtual void PrintContentToLog(LogLevel logLevel)
  {
    if(RawBuildActions is not null)
      using(Log.IndentBlock(logLevel, "BuildActions: ", "[]"))
        foreach(var pair in RawBuildActions)
        foreach(var buildAction in pair.Value)
          Log.WriteLine(LogLevel.Info, $"{{ Action: {buildAction.ToHoconString()}, Stage: {pair.Key} }}");
  }

  [PublicAPI]
  protected void PrintChildrenToLog(LogLevel logLevel)
  {
    if(RawChildren is not null)
      foreach(var child in RawChildren)
        if(child is ILoggable printable)
          printable.PrintToLog(logLevel);
        else
          Log.WriteLine(logLevel, $"Child: {child.ToHoconString()}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public virtual string ToHoconString() => GetType().GetShortName().QuoteIfNeeded();

  public virtual bool Equals(IBuildStackPattern? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;

    return other is BuildStackPatternBase otherNode && Weight == otherNode.Weight && GetType() == otherNode.GetType();
  }

  public override bool Equals(object? obj) => Equals(obj as IBuildStackPattern);
  public override int  GetHashCode()       => Weight.GetHashCode();

  IEnumerator IEnumerable.GetEnumerator() => RawChildren?.GetEnumerator() ?? Empty<IBuildStackPattern>.Array.GetEnumerator();

  #region Internal

  long IInternal<long>.                                                          Member1 => Weight;
  HashSet<IBuildStackPattern>? IInternal<long, HashSet<IBuildStackPattern>?>.    Member2 => RawChildren;
  BuildActionBag? IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?>.Member3 => RawBuildActions;

  #endregion
}
