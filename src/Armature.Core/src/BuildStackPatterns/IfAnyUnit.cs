using System;
using System.Collections.Generic;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature.Core;

/// <summary>
/// It's a very special build stack pattern that matches with any 'unit'. See usages for details.
/// </summary>
public class IfAnyUnit : BuildStackPatternBase, IBuildStackPattern
{
  public static readonly IfAnyUnit Instance = new();

  private static readonly UnitId FakeUnitId = new(typeof(IfAnyUnit), null);

  private IfAnyUnit() : base(0) { }

  public override bool IsStatic(out UnitId unitId)
  {
    unitId = FakeUnitId;
    return true;
  }

  public override bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
    => GetOwnBuildActions(inputWeight, out actionBag);

  public override bool Equals(IBuildStackPattern? other) => other is IfAnyUnit;

  public override T GetOrAddNode<T>(T node) => throw new NotSupportedException();
  public override T AddNode<T>(T node) => throw new NotSupportedException();

  long IInternal<long>.                                                         Member1 => 0;
  HashSet<IBuildStackPattern>? IInternal<long, HashSet<IBuildStackPattern>?>.   Member2 => null;
  BuildActionBag IInternal<long, HashSet<IBuildStackPattern>?, BuildActionBag?>.Member3 => BuildActions;
}
