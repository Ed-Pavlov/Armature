using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Base class implementing the logic of adding build actions
/// </summary>
public abstract class BuildChainPatternBase : IBuildChainPattern, IEnumerable, ILogPrintable
{
  private   BuildActionBag?              _buildActions;
  protected HashSet<IBuildChainPattern>? RawChildren;
  private   BuildActionBag               LazyBuildAction => _buildActions ??= new BuildActionBag();

  protected BuildChainPatternBase(int weight) => Weight = weight;

  public    BuildActionBag              BuildActions => LazyBuildAction;
  private   HashSet<IBuildChainPattern> LazyChildren => RawChildren ??= new HashSet<IBuildChainPattern>();
  public    HashSet<IBuildChainPattern> Children     => LazyChildren;
  protected int                         Weight       { [DebuggerStepThrough] get; }

  [PublicAPI]
  protected bool GetOwnBuildActions(int inputWeight, out WeightedBuildActionBag? actionBag)
  {
    actionBag = null;
    if(_buildActions is null) return false;

    var matchingWeight = inputWeight + Weight;
    actionBag = new WeightedBuildActionBag();

    foreach(var pair in _buildActions)
      actionBag.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

    return true;
  }

  protected bool GetOwnAndChildrenBuildActions(BuildChain buildChain, int inputWeight, out WeightedBuildActionBag? actionBag)
  {
    var result = GetOwnBuildActions(inputWeight, out actionBag);
    actionBag.WriteToLog(LogLevel.Verbose, "Actions: ");

    if(RawChildren is not null && buildChain.Length > 0)
    { // pass the rest of the chain to children and return their actions
      result    |= GetChildrenActions(buildChain, inputWeight, out var childrenActionBag);
      actionBag =  actionBag.Merge(childrenActionBag);
    }

    return result;
  }

  public abstract bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight);

  /// <summary>
  /// Gathers and merges build actions from all children nodes.
  /// </summary>
  /// <param name="buildChain">The build chain to pass to children nodes if any.</param>
  /// <param name="inputWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
  /// <param name="actionBag"></param>
  [PublicAPI]
  protected bool GetChildrenActions(BuildChain buildChain, int inputWeight, out WeightedBuildActionBag? actionBag)
  {
    actionBag = null;

    if(RawChildren is null)
    {
      Log.WriteLine(LogLevel.Trace, "Children: null");
      return false;
    }

    var matchingWeight = inputWeight + Weight;

    var hasActions = false;

    // ReSharper disable once AccessToModifiedClosure, yes that's the point
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, "PassTailToChildren"))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"ActualWeight = {matchingWeight.ToHoconString()}, Tail = {buildChain.ToHoconString()}");

      foreach(var child in RawChildren)
      {
        if(child.GatherBuildActions(buildChain, out var childBag, matchingWeight))
          actionBag = actionBag.Merge(childBag);
      }

      hasActions = actionBag is not null;
      return hasActions;
    }
  }
  public void PrintToLog(LogLevel logLevel = LogLevel.None)
  {
    using(Log.NamedBlock(logLevel, () => GetType().GetShortName().QuoteIfNeeded()))
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
    if(RawChildren is not null)
      foreach(var child in RawChildren)
        if(child is ILogPrintable printable)
          printable.PrintToLog(logLevel);
        else
          Log.WriteLine(logLevel, $"Child: {child.ToHoconString()}");
  }
  public string ToHoconString() => GetType().GetShortName().QuoteIfNeeded();

  public virtual bool Equals(IBuildChainPattern? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;

    return other is BuildChainPatternBase otherNode && Weight == otherNode.Weight && GetType() == otherNode.GetType();
  }

  public override bool Equals(object? obj) => Equals(obj as IBuildChainPattern);
  public override int  GetHashCode()       => Weight.GetHashCode();

  public void             Add(IBuildChainPattern buildChainPattern) => Children.Add(buildChainPattern);
  IEnumerator IEnumerable.GetEnumerator()                           => RawChildren?.GetEnumerator() ?? Empty<IBuildChainPattern>.Array.GetEnumerator();
}