using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Common;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  ///   Moves along the building units sequence from left to right skipping units until it encounters a matching unit.
  /// </summary>
  public class WeakUnitSequenceMatcher : UnitSequenceMatcherBase, IEquatable<WeakUnitSequenceMatcher>
  {
    private readonly IUnitMatcher _matcher;

    public WeakUnitSequenceMatcher([NotNull] IUnitMatcher matcher, int weight) : base(weight) => 
      _matcher = matcher ?? throw new ArgumentNullException(nameof(matcher));

    /// <summary>
    ///   Moves along the unit building sequence from left to right skipping units until it encounters a matching unit.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      for (var i = 0; i < buildingUnitsSequence.Length; i++)
      {
        var unitInfo = buildingUnitsSequence[i];
        if (_matcher.Matches(unitInfo))
          return GetActions(buildingUnitsSequence.GetTail(i), inputWeight);
      }

      return null;
    }

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    private MatchedBuildActions GetActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      
        MatchedBuildActions matchedBuildActions;
        if (buildingUnitsSequence.Length > 1)
        {
          Log.Verbose(this.ToString());
          using (Log.AddIndent())
            matchedBuildActions = GetChildrenActions(inputWeight + Weight, buildingUnitsSequence.GetTail(1)); // pass the rest of the sequence to children and return their actions
        }
        else
        {
          matchedBuildActions = GetOwnActions(inputWeight);
          if (matchedBuildActions == null)
            Log.Trace("{0}{{not matched}}", this);
          else
            using (Log.Block(LogLevel.Verbose, this.ToString()))
              matchedBuildActions.ToLog();
        }

        return matchedBuildActions;
      
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}.{1}", GetType().Name, _matcher);

    #region Equality
    public bool Equals(WeakUnitSequenceMatcher other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;

      return Equals(_matcher, other._matcher) && Weight == other.Weight;
    }

    public override bool Equals(IUnitSequenceMatcher other) => Equals(other as WeakUnitSequenceMatcher);

    public override bool Equals(object obj) => Equals(obj as WeakUnitSequenceMatcher);

    public override int GetHashCode()
    {
      unchecked
      {
        return ((_matcher != null ? _matcher.GetHashCode() : 0) * 397) ^ Weight;
      }
    }

    [DebuggerStepThrough]
    public static bool operator ==(WeakUnitSequenceMatcher left, WeakUnitSequenceMatcher right) => Equals(left, right);

    [DebuggerStepThrough]
    public static bool operator !=(WeakUnitSequenceMatcher left, WeakUnitSequenceMatcher right) => !Equals(left, right);
    #endregion
  }
}