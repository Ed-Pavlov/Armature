using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Base class implementing <see cref="IBuildStackPattern.BuildActions"/>
/// </summary>
public abstract class BuildStackPatternBase : IBuildStackPattern, IEnumerable, ILogPrintable, IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?>
{
  [PublicAPI]
  protected BuildActionBag?              _buildActions;
  protected HashSet<IBuildStackPattern>? _rawChildren;

  protected BuildStackPatternBase(int weight) => Weight = weight;

  protected long Weight { [DebuggerStepThrough] get; }

  public BuildActionBag              BuildActions => _buildActions ??= new BuildActionBag();
  public HashSet<IBuildStackPattern> Children     => _rawChildren ??= new HashSet<IBuildStackPattern>();

  [PublicAPI]
  protected bool GetOwnBuildActions(long inputWeight, out WeightedBuildActionBag? actionBag)
  {
    actionBag = null;
    if(_buildActions is null) return false;

    var matchingWeight = inputWeight + Weight;
    actionBag = new WeightedBuildActionBag();

    foreach(var pair in _buildActions)
      actionBag.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

    return true;
  }

  protected bool GetOwnAndChildrenBuildActions(BuildSession.Stack stack, long inputWeight, out WeightedBuildActionBag? actionBag)
  {
    var result = GetOwnBuildActions(inputWeight, out actionBag);
    actionBag.WriteToLog(LogLevel.Verbose, "Actions: ");

    if(_rawChildren is not null && stack.Length > 0)
    { // pass the rest of the stack to children and return their actions
      result    |= GetChildrenActions(stack, inputWeight, out var childrenActionBag);
      actionBag =  actionBag.Merge(childrenActionBag);
    }

    return result;
  }

  public abstract bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight);

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

    if(_rawChildren is null)
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

      foreach(var child in _rawChildren)
      {
        if(child.GatherBuildActions(stack, out var childBag, matchingWeight))
          actionBag = actionBag.Merge(childBag);
      }

      var hasActions = actionBag is not null;
      condition.IsMet = hasActions;
      return hasActions;
    }
  }

  public void PrintToLog(LogLevel logLevel = LogLevel.None)
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
    if(_buildActions is not null)
      using(Log.IndentBlock(logLevel, "BuildActions: ", "[]"))
        foreach(var pair in _buildActions)
        foreach(var buildAction in pair.Value)
          Log.WriteLine(LogLevel.Info, $"{{ Action: {buildAction.ToHoconString()}, Stage: {pair.Key} }}");
  }

  [PublicAPI]
  protected void PrintChildrenToLog(LogLevel logLevel)
  {
    if(_rawChildren is not null)
      foreach(var child in _rawChildren)
        if(child is ILogPrintable printable)
          printable.PrintToLog(logLevel);
        else
          Log.WriteLine(logLevel, $"Child: {child.ToHoconString()}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public string ToHoconString() => GetType().GetShortName().QuoteIfNeeded();

  public virtual bool Equals(IBuildStackPattern? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;

    return other is BuildStackPatternBase otherNode && Weight == otherNode.Weight && GetType() == otherNode.GetType();
  }

  public override bool Equals(object? obj) => Equals(obj as IBuildStackPattern);
  public override int  GetHashCode()       => Weight.GetHashCode();

  public void             Add(IBuildStackPattern buildStackPattern) => Children.Add(buildStackPattern);
  IEnumerator IEnumerable.GetEnumerator()                           => _rawChildren?.GetEnumerator() ?? Empty<IBuildStackPattern>.Array.GetEnumerator();

  #region Internal
  long IInternal<long>.Member1 => Weight;
  HashSet<IBuildStackPattern>? IInternal<long, HashSet<IBuildStackPattern>?>.Member2 => _rawChildren;
  BuildActionBag? IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?>.Member3 => _buildActions;
  #endregion
}
