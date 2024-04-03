using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Sdk;

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
public class BuildStackPatternTree :
  IBuildStackPattern,
  IEnumerable,
  ILoggable,
  IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?, Dictionary<UnitId, LeanList<IBuildStackPattern>>>
{
  private readonly string _name;
  private readonly Root   _root;

  private readonly Dictionary<UnitId, LeanList<IBuildStackPattern>> _staticMap = new();

  public BuildStackPatternTree(string name, int weight = 0)
  {
    _name = name ?? throw new ArgumentNullException(nameof(name));
    _root = new Root(weight, this);
  }

  ///<inheritdoc />
  public bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
  {
    _root.GatherBuildActions(stack, out actionBag, 0);

    if(_staticMap.TryGetValue(stack.TargetUnit, out var list))
      foreach(var node in list)
      {
        node.GatherBuildActions(stack, out var bag);
        actionBag = actionBag.Merge(bag);
      }

    return actionBag != null;
  }

  bool IStaticPattern.IsStatic(out UnitId unitId)
  {
    unitId = default;
    return false;
  }

  ///<inheritdoc />
  public virtual void PrintToLog(LogLevel logLevel = LogLevel.None)
  {
    using(Log.NamedBlock(logLevel, ToHoconString))
    {
      foreach(var node in _staticMap.Values.SelectMany(list => list))
      {
        if(node is ILoggable loggable)
          loggable.PrintToLog(logLevel);
        else
          Log.WriteLine(logLevel, node.ToHoconString());
      }

      _root.PrintToLog(logLevel);
    }
  }

  public T GetOrAddNode<T>(T node) where T : IBuildStackPattern
  {
    IBuildStackPattern result;

    if(node is not IStaticPattern staticPattern || !staticPattern.IsStatic(out var unitId))
      result = _root.GetOrAddNode(node);
    else
    {
      if(!_staticMap.TryGetValue(unitId, out var list))
      { // no list - no node, add passed one
        list = new LeanList<IBuildStackPattern> {node};
        _staticMap.Add(unitId, list);
        result = node;
      }
      else
      {
        var indexOfExistentNode = list.IndexOf(node);

        if(indexOfExistentNode >= 0)
          result = list[indexOfExistentNode]; // list contains equal node, return it
        else
        {
          list.Add(node); // list presents but doesn't contain a node equal to passed, add passed one
          result = node;
        }
      }
    }

    return (T) result;
  }

  public T AddNode<T>(T node) where T : IBuildStackPattern
  {
    if(node is not IStaticPattern staticPattern || !staticPattern.IsStatic(out var unitId))
      _root.AddNode(node);
    else
    {
      if(!_staticMap.TryGetValue(unitId, out var list))
        _staticMap.Add(unitId, new LeanList<IBuildStackPattern> {node}); // no list - no node, add passed one
      else if(!list.Contains(node))
        list.Add(node); // list presents but doesn't contain a node equal to passed, add passed one
      else
        BuildStackPatternExtension.ThrowNodeIsAlreadyAddedException(this, node);
    }

    return node;
  }

  public virtual string ToHoconString() => _name.ToHoconString();

  bool IBuildStackPattern.            AddBuildAction(IBuildAction buildAction, object buildStage) => throw new NotSupportedException();
  bool IEquatable<IBuildStackPattern>.Equals(IBuildStackPattern   other) => throw new NotSupportedException();

  #region Syntax sugar

  public void             Add(IBuildStackPattern buildStackPattern) => _root.AddNode(buildStackPattern);
  IEnumerator IEnumerable.GetEnumerator()                           => throw new NotSupportedException();

  #endregion

  /// <summary>
  /// Reuse implementation of <see cref="BuildStackPatternBase" /> to implement <see cref="BuildStackPatternTree" /> public interface.
  /// </summary>
  private class Root : BuildStackPatternBase
  {
    private readonly ILogString _logString;

    [DebuggerStepThrough]
    public Root(int weight, ILogString logString) : base(weight) => _logString = logString ?? throw new ArgumentNullException(nameof(logString));

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

    public override void PrintToLog(LogLevel logLevel = LogLevel.None) => PrintChildrenToLog(logLevel);

    [DebuggerStepThrough]
    public override string ToHoconString() => _logString.ToHoconString();

    [DebuggerStepThrough]
    public override bool Equals(IBuildStackPattern? other) => throw new NotSupportedException();
  }

  #region Internals

  long IInternal<long>.Member1 => ((IInternal<long, HashSet<IBuildStackPattern>, BuildActionBag>) _root).Member1;

  HashSet<IBuildStackPattern> IInternal<long, HashSet<IBuildStackPattern>?>.Member2
    => ((IInternal<long, HashSet<IBuildStackPattern>, BuildActionBag>) _root).Member2;

  BuildActionBag? IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?>.Member3 => null;

  Dictionary<UnitId, LeanList<IBuildStackPattern>>
    IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?, Dictionary<UnitId, LeanList<IBuildStackPattern>>>.Member4
    => _staticMap;

  #endregion
}
