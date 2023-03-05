using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Armature.Core;

/// <summary>
/// The root node of the tree of <see cref="IBuildStackPattern"/> nodes.
/// </summary>
/// <remarks>
/// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
/// new BuildStackPatternTree(...)
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
  private readonly RootNode _rootNode;

  public BuildStackPatternTree(int weight = 0) => _rootNode = new RootNode(weight, this);

  HashSet<IBuildStackPattern> IBuildStackPattern.Children => _rootNode.Children;

  ///<inheritdoc />
  bool IBuildStackPattern.GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
    => _rootNode.GatherBuildActions(stack, out actionBag, 0);

  ///<inheritdoc />
  public virtual void PrintToLog(LogLevel logLevel = LogLevel.None) => _rootNode.PrintToLog(logLevel);

  ///<inheritdoc />
  BuildActionBag IBuildStackPattern.BuildActions => throw new NotSupportedException();

  bool IEquatable<IBuildStackPattern>.Equals(IBuildStackPattern other) => throw new NotSupportedException();

  #region Syntax sugar

  public void             Add(IBuildStackPattern buildStackPattern) => ((IBuildStackPattern) this).Children.Add(buildStackPattern);
  IEnumerator IEnumerable.GetEnumerator()                           => throw new NotSupportedException();

  #endregion

  /// <summary>
  /// Reuse implementation of <see cref="BuildStackPatternBase" /> to implement <see cref="BuildStackPatternTree" /> public interface.
  /// </summary>
  private class RootNode : BuildStackPatternBase
  {
    private readonly ILogString _logString;

    [DebuggerStepThrough]
    public RootNode(int weight, ILogString logString) : base(weight) => _logString = logString ?? throw new ArgumentNullException(nameof(logString));

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
    public override string ToHoconString() => _logString.ToHoconString();

    [DebuggerStepThrough]
    public override bool Equals(IBuildStackPattern? other) => throw new NotSupportedException();
  }

  public virtual string ToHoconString() => GetType().GetShortName().QuoteIfNeeded();
}
