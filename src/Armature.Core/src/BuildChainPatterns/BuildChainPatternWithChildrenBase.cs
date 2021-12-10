using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Base class exposing the collection of children nodes, gathering, and merging build actions from children.
/// </summary>
/// <remarks>
/// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
/// new FooNode
/// {
/// new SubNode(Pattern1.Instance, 0)
///  .UseBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
/// new SubNode(Pattern2.Instance, ParameterMatchingWeight.Lowest)
///  .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
/// };
/// </remarks>
public abstract class BuildChainPatternWithChildrenBase : IBuildChainPattern, IEnumerable, ILogPrintable
{
  protected HashSet<IBuildChainPattern>? RawChildren;
  private   HashSet<IBuildChainPattern>  LazyChildren => RawChildren ??= new HashSet<IBuildChainPattern>();

  protected BuildChainPatternWithChildrenBase(int weight) => Weight = weight;

  public abstract WeightedBuildActionBag? GatherBuildActions(BuildChain buildChain, int inputWeight);
  public abstract BuildActionBag          BuildActions { get; }

  public ICollection<IBuildChainPattern> Children => LazyChildren;

  protected int Weight { [DebuggerStepThrough] get; }

  /// <summary>
  /// Gathers and merges build actions from all children nodes.
  /// </summary>
  /// <param name="buildChain">The build chain to pass to children nodes if any.</param>
  /// <param name="inputWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
  protected WeightedBuildActionBag? GetChildrenActions(BuildChain buildChain, int inputWeight)
  {
    if(RawChildren is null)
    {
      Log.WriteLine(LogLevel.Trace, "Children: null");
      return null;
    }

    WeightedBuildActionBag? result = null;

    var matchingWeight = inputWeight + Weight;
    using(Log.NamedBlock(LogLevel.Trace, "PassTailToChildren"))
    {
      Log.WriteLine(LogLevel.Trace, $"ActualWeight = {matchingWeight}, Tail = {buildChain.ToHoconString()}");

      foreach(var child in RawChildren)
        result = result.Merge(child.GatherBuildActions(buildChain, matchingWeight));
    }

    return result;
  }

  public void PrintToLog(LogLevel logLevel = LogLevel.None)
  {
    using(Log.NamedBlock(logLevel, GetType().GetShortName()))
    {
      Log.WriteLine(LogLevel.Info, $"Weight: {Weight:n0}");
      PrintContentToLog(logLevel);
      PrintChildrenToLog(logLevel);
    }
  }

  protected virtual void PrintContentToLog(LogLevel logLevel) { }

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

  public virtual bool Equals(IBuildChainPattern? other)
    => other is BuildChainPatternBase otherNode && Weight == otherNode.Weight && GetType() == otherNode.GetType();

  public override bool Equals(object? obj) => Equals(obj as IBuildChainPattern);

  public override int GetHashCode() => Weight.GetHashCode();

  #region Syntax sugar

  public void Add(IBuildChainPattern buildChainPattern) => Children.Add(buildChainPattern);

  IEnumerator IEnumerable.GetEnumerator() => RawChildren?.GetEnumerator() ?? Empty<IBuildChainPattern>.Array.GetEnumerator();

  #endregion
}