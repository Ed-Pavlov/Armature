﻿using System;
using System.Collections;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.Logging;

// ReSharper disable ArrangeThisQualifier

namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Matches any sequence of building units, thus passing the unit under construction to <see cref="IUnitSequenceMatcher.Children" /> and merge their
  ///   build actions with its own.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new AnyUnitSequenceMatcher
  ///   {
  ///   new LeafUnitSequenceMatcher(ConstructorMatcher.Instance, 0)
  ///   .AddBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
  ///   new LeafUnitSequenceMatcher(ParameterMatcher.Instance, ParameterMatchingWeight.Lowest)
  ///   .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  ///   };
  /// </remarks>
  public class AnyUnitSequenceMatcher : UnitSequenceMatcherWithChildren, IEnumerable
  {
    public AnyUnitSequenceMatcher() : this(UnitSequenceMatchingWeight.AnyUnit) { }

    public AnyUnitSequenceMatcher(int weight) : base(weight) { }

    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

    /// <summary>
    ///   Matches any <see cref="UnitInfo" />, so it pass the building unit info into its children and returns merged result
    /// </summary>
    public override MatchedBuildActions? GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      var unitsToSkip = buildingUnitsSequence.Length;

      // decrease matching weight depending on how many unit in the sequence were skipped by this matcher
      var matchingWeight = inputWeight + Weight * unitsToSkip;

      var                  lastItemAsTail = buildingUnitsSequence.GetTail(buildingUnitsSequence.Length - 1);
      var                  ownActions     = GetOwnActions(matchingWeight);
      MatchedBuildActions? childrenActions;

      if(ownActions is null)
      {
        Log.WriteLine(LogLevel.Verbose, this.ToString, unitsToSkip); // pass group method, do not call ToString

        using(Log.AddIndent())
        {
          childrenActions = GetChildrenActions(matchingWeight, lastItemAsTail);
        }
      }
      else
      {
        using(Log.Block(LogLevel.Verbose, this.ToString, unitsToSkip)) // pass group method, do not call ToString
        {
          // ReSharper disable once RedundantArgumentDefaultValue
          ownActions.ToLog(LogLevel.Verbose);
          childrenActions = GetChildrenActions(matchingWeight, lastItemAsTail);
        }
      }

      return childrenActions.Merge(ownActions);
    }

    public void Add(IUnitSequenceMatcher unitSequenceMatcher) => Children.Add(unitSequenceMatcher);

    private string ToString(int unitsToSkip) => string.Format("{0}<x{1:n0}>", base.ToString(), unitsToSkip);

#region Equality

    [DebuggerStepThrough]
    public override bool Equals(IUnitSequenceMatcher other) => Equals((object) other);

    [DebuggerStepThrough]
    public override bool Equals(object? obj)
    {
      if(ReferenceEquals(null, obj)) return false;
      if(ReferenceEquals(this, obj)) return true;

      return obj is AnyUnitSequenceMatcher other && Weight == other.Weight;
    }

    [DebuggerStepThrough]
    public override int GetHashCode() => Weight.GetHashCode();

#endregion
  }
}
