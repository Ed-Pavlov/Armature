﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
///   The reusable implementation of <see cref="IBuildChainPattern" /> which is used as a root node of the tree.
/// </summary>
/// <remarks>
///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
///   new Builder(...)
///   {
///     new SkipToLastUnit
///     {
///       new IfFirstUnitMatches(new ConstructorPattern(), 0)
///         .AddBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
///       new IfFirstUnitMatches(new MethodArgumentPattern(), ParameterMatchingWeight.Lowest)
///         .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
///     }
///   };
/// </remarks>
public class BuildChainPatternTree : IBuildChainPattern, IEnumerable, ILogPrintable
{
  private readonly Root _root = new();

  public ICollection<IBuildChainPattern> Children => _root.Children;

  public WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> buildChain, int inputWeight = 0)
    => _root.GatherBuildActions(buildChain, 0);

  public void PrintToLog(LogLevel logLevel = LogLevel.None) => _root.PrintToLog(logLevel);

  public BuildActionBag BuildActions                   => throw new NotSupportedException();
  public bool           Equals(IBuildChainPattern other) => throw new NotSupportedException();

  #region Syntax sugar

  public void             Add(IBuildChainPattern buildChainPattern) => Children.Add(buildChainPattern);
  IEnumerator IEnumerable.GetEnumerator()                       => throw new NotSupportedException();

  #endregion

  /// <summary>
  ///   Reuse implementation of <see cref="BuildChainPatternWithChildrenBase" /> to implement <see cref="BuildChainPatternTree" /> public interface
  /// </summary>
  private class Root : BuildChainPatternBase
  {
    [DebuggerStepThrough]
    public Root() : base(0) { }

    [DebuggerStepThrough]
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> buildChain, int inputWeight)
    {
      if(RawChildren is null) return null;

      WeightedBuildActionBag? result = null;
      foreach(var child in RawChildren)
        result = result.Merge(child.GatherBuildActions(buildChain, inputWeight));
      return result;
    }

    [DebuggerStepThrough]
    public override bool Equals(IBuildChainPattern? other) => throw new NotSupportedException();
  }
}