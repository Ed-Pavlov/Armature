using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Core.Common;
using Armature.Core.Logging;


namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Matches only unit under construction in the sequence and applies passed <see cref="IUnitMatcher" /> to it.
  ///   See <see cref="LastUnitSequenceMatcher(Armature.Core.IUnitMatcher,int)" /> and <see cref="GetBuildActions" /> for details
  /// </summary>
  public class LastUnitSequenceMatcher : UnitSequenceMatcher
  {
    private readonly IUnitMatcher _unitMatcher;

    /// <param name="unitMatcher">Object contains the logic of matching with building unit</param>
    /// <param name="weight">The weight of matching</param>
    [DebuggerStepThrough]
    public LastUnitSequenceMatcher(IUnitMatcher unitMatcher, int weight = 0) : base(weight) =>
      _unitMatcher = unitMatcher ?? throw new ArgumentNullException(nameof(unitMatcher));

    public override ICollection<IUnitSequenceMatcher> Children => throw new NotSupportedException("LastUnitSequenceMatcher can't contain children");

    /// <summary>
    ///   If <paramref name="buildingUnitsSequence" /> contains more then one element return null. This matcher matches only unit under construction which is
    ///   the last one in the <paramref name="buildingUnitsSequence" />.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public override MatchedBuildActions? GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      if (buildingUnitsSequence.Length != 1) return null;

      var unitInfo = buildingUnitsSequence.Last();
      var matches = _unitMatcher.Matches(unitInfo);

      if (!matches)
      {
        Log.WriteLine(LogLevel.Trace, () => string.Format("{0}{{not matched}}", this));
        return null;
      }

      using (Log.Block(LogLevel.Verbose, this.ToString)) // pass method group, do not call ToString
      {
        var buildActions = GetOwnActions(Weight + inputWeight);
        buildActions.ToLog();
        return buildActions;
      }
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>.{2}", GetType().GetShortName(), Weight, _unitMatcher);

    #region Equality
    private bool Equals(LastUnitSequenceMatcher? other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;

      return Weight == other.Weight && _unitMatcher.Equals(other._unitMatcher);
    }

    public override bool Equals(IUnitSequenceMatcher obj) => Equals(obj as LastUnitSequenceMatcher);

    public override bool Equals(object obj) => Equals(obj as LastUnitSequenceMatcher);

    public override int GetHashCode()
    {
      unchecked
      {
        return (Weight * 397) ^ _unitMatcher.GetHashCode();
      }
    }
    #endregion
  }
}