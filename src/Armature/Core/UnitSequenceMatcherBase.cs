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
    private Dictionary<object, List<Weighted<IBuildAction>>> _actionFactories;
    private HashSet<IUnitSequenceMatcher> _children;

    private HashSet<IUnitSequenceMatcher> LazyChildren { [DebuggerStepThrough] get => _children ?? (_children = new HashSet<IUnitSequenceMatcher>()); }

    private Dictionary<object, List<Weighted<IBuildAction>>> LazyActionFactories
    {
      [DebuggerStepThrough] get => _actionFactories ?? (_actionFactories = new Dictionary<object, List<Weighted<IBuildAction>>>());
    }

    [NotNull]
    public ICollection<IUnitSequenceMatcher> Children { [DebuggerStepThrough] get { return LazyChildren; } }

    [CanBeNull]
    public abstract MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputMatchingWeight);

    [DebuggerStepThrough]
    public IUnitSequenceMatcher AddBuildAction(object buildStage, IBuildAction buildAction, int weight)
    {
      LazyActionFactories
        .GetOrCreateValue(buildStage, () => new List<Weighted<IBuildAction>>())
        .Add(buildAction.WithWeight(weight));
      return this;
    }

    public abstract bool Equals(IUnitSequenceMatcher other);

    [DebuggerStepThrough]
    protected MatchedBuildActions GetOwnActions(UnitInfo unitInfo, int inputWeight) //TODO: can be null or what? 
    {
      var result = new MatchedBuildActions();
      foreach (var pair in LazyActionFactories) //TODO: use _actionFactories instead in order to not create it if it is not neeeded
      {
        var actions = pair.Value
          .Select(_ => _.Entity.WithWeight(_.Weight + inputWeight))
          .ToList();

        if (actions.Count > 0)
          result.Add(pair.Key, actions);
      }

      return result;
    }

    /// <summary>
    ///   Gets and merges matched actions from all children matchers
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which used by children build steps to calculate a final weight of matching</param>
    /// <param name="unitBuildingSequence">The sequence of unit infos to match with build steps and find suitable one</param>
    [DebuggerStepThrough]
    protected MatchedBuildActions GetChildrenActions(int inputMatchingWeight, ArrayTail<UnitInfo> unitBuildingSequence) =>
      LazyChildren.Aggregate((MatchedBuildActions)null, (current, child) => current.Merge(child.GetBuildActions(unitBuildingSequence, inputMatchingWeight)));
  }
}