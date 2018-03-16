using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Common;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Applies passed <see cref="IUnitMatcher" /> to the unit under construction.
  ///   <see cref="LeafUnitSequenceMatcher(Armature.Core.IUnitMatcher,int)" /> and <see cref="GetBuildActions" /> for details
  /// </summary>
  public class LeafUnitSequenceMatcher : UnitSequenceMatcherBase
  {
    private readonly IUnitMatcher _unitMatcher;

    /// <param name="unitMatcher">Object contains the logic of matching with building unit</param>
    /// <param name="weight">The weight of matching</param>
    [DebuggerStepThrough]
    public LeafUnitSequenceMatcher([NotNull] IUnitMatcher unitMatcher, int weight) : base(weight) => 
      _unitMatcher = unitMatcher ?? throw new ArgumentNullException(nameof(unitMatcher));

    /// <summary>
    ///   If <paramref name="buildingUnitsSequence" /> contains more then one element return null. This matcher matches only unit under construction which is
    ///   the last one in the <see cref="buildingUnitsSequence" />.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      if (buildingUnitsSequence.Length != 1) return null;

      var unitInfo = buildingUnitsSequence.GetLastItem();
      var matches = _unitMatcher.Matches(unitInfo);

      if (!matches)
      {
        Log.Trace("{0}{{not matched}}", this);
        return null;
      }

      using (Log.Block(LogLevel.Verbose, this.ToString()))
      {
        var buildActions = GetOwnActions(inputWeight);
        buildActions.ToLog();
        return buildActions;
      }
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}.{1}", GetType().GetShortName(), _unitMatcher);

    #region Equality
    private bool Equals(LeafUnitSequenceMatcher other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;

      return Weight == other.Weight && Equals(_unitMatcher, other._unitMatcher);
    }

    public override bool Equals(IUnitSequenceMatcher obj) => Equals(obj as LeafUnitSequenceMatcher);

    public override bool Equals(object obj) => Equals(obj as LeafUnitSequenceMatcher);

    public override int GetHashCode()
    {
      unchecked
      {
        return (Weight * 397) ^ (_unitMatcher != null ? _unitMatcher.GetHashCode() : 0);
      }
    }

    [DebuggerStepThrough]
    public static bool operator ==(LeafUnitSequenceMatcher left, LeafUnitSequenceMatcher right) => Equals(left, right);

    [DebuggerStepThrough]
    public static bool operator !=(LeafUnitSequenceMatcher left, LeafUnitSequenceMatcher right) => !Equals(left, right);
    #endregion
  }
}