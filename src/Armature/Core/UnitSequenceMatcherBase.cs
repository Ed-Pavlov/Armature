using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Base class exposing the collection of children matchers and implementing the logic of adding build action factories,
  ///   gathering and merging build actions from children with its own.
  /// </summary>
  public abstract class UnitSequenceMatcherBase : IUnitSequenceMatcher
  {
    private Dictionary<object, IBuildAction> _buildActions;
    private HashSet<IUnitSequenceMatcher> _children;

    protected UnitSequenceMatcherBase(int weight) => Weight = weight;

    protected int Weight { get; }

    private HashSet<IUnitSequenceMatcher> LazyChildren
    {
      [DebuggerStepThrough]
      get => _children ?? (_children = new HashSet<IUnitSequenceMatcher>());
    }

    private Dictionary<object, IBuildAction> LazyBuildAction { [DebuggerStepThrough] get => _buildActions ?? (_buildActions = new Dictionary<object, IBuildAction>()); }

    [NotNull]
    public ICollection<IUnitSequenceMatcher> Children { [DebuggerStepThrough] get { return LazyChildren; } }

    [CanBeNull]
    public abstract MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight);

    [DebuggerStepThrough]
    public IUnitSequenceMatcher AddBuildAction(object buildStage, IBuildAction buildAction)
    {
      if (LazyBuildAction.ContainsKey(buildAction))
        throw new ArgumentException(string.Format($"Already contains build action for stage {buildStage}"));

      LazyBuildAction.Add(buildStage, buildAction);
      return this;
    }

    public abstract bool Equals(IUnitSequenceMatcher other);

    [DebuggerStepThrough]
    [CanBeNull]
    protected MatchedBuildActions GetOwnActions(int inputWeight)
    {
      if (_buildActions == null) return null;

      var matchingWeight = Weight + inputWeight;
      var result = new MatchedBuildActions();
      foreach (var pair in _buildActions)
        result.Add(pair.Key, new List<Weighted<IBuildAction>> {pair.Value.WithWeight(matchingWeight)});

      return result;
    }

    /// <summary>
    ///   Gets and merges matched actions from all children matchers
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which used by children build steps to calculate a final weight of matching</param>
    /// <param name="unitBuildingSequence">The sequence of unit infos to match with build steps and find suitable one</param>
    [DebuggerStepThrough]
    [CanBeNull]
    protected MatchedBuildActions GetChildrenActions(int inputMatchingWeight, ArrayTail<UnitInfo> unitBuildingSequence) =>
      _children?.Aggregate(
        (MatchedBuildActions)null,
        (current, child) => current.Merge(child.GetBuildActions(unitBuildingSequence, inputMatchingWeight)));
  }
}