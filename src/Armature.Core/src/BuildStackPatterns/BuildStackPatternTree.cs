using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// The reusable implementation of <see cref="IBuildStackPattern" /> which is used as a root node of the tree.
/// </summary>
/// <remarks>
/// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
/// new Builder(...)
/// {
///    new IfFirstUnit(new IsConstructor())
///      .UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
///    new IfFirstUnit(new IsParameterInfoList())
///      .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
///    new IfFirstUnit(new IsParameterInfo())
///      .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
/// };
/// </remarks>
public class BuildStackPatternTree : IBuildStackPattern, IEnumerable, ILogPrintable
{
  private readonly Root _root;

  public BuildStackPatternTree(int weight = 0) => _root = new Root(weight);

  public HashSet<IBuildStackPattern> Children => _root.Children;

  public bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight = 0L)
    => _root.GatherBuildActions(stack, out actionBag, 0);

  public void PrintToLog(LogLevel logLevel = LogLevel.None) => _root.PrintToLog(logLevel);

  public BuildActionBag BuildActions                     => throw new NotSupportedException();
  public bool           Equals(IBuildStackPattern other) => throw new NotSupportedException();

  #region Syntax sugar

  public void             Add(IBuildStackPattern buildStackPattern) => Children.Add(buildStackPattern);
  IEnumerator IEnumerable.GetEnumerator()                           => throw new NotSupportedException();

  #endregion

  /// <summary>
  /// Reuse implementation of <see cref="BuildStackPatternBase" /> to implement <see cref="BuildStackPatternTree" /> public interface
  /// </summary>
  private class Root : BuildStackPatternBase
  {
    [DebuggerStepThrough]
    public Root(int weight) : base(weight) { }

    [DebuggerStepThrough]
    public override bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
    {
      actionBag = null;
      if(RawChildren is null) return false;

      foreach(var child in RawChildren)
      {
        if(child.GatherBuildActions(stack, out var childBag, inputWeight))
          actionBag = actionBag.Merge(childBag);
      }

      return true;
    }

    [DebuggerStepThrough]
    public override bool Equals(IBuildStackPattern? other) => throw new NotSupportedException();
  }

  public string ToHoconString() => GetType().GetShortName().QuoteIfNeeded();
}