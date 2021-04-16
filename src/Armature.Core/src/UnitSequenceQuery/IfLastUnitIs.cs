using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Matches only unit under construction in the sequence and applies passed <see cref="IUnitIdMatcher" /> to it.
  ///   See <see cref="IfLastUnitIs(Armature.Core.IUnitIdMatcher,int)" /> and <see cref="GatherBuildActions" /> for details
  /// </summary>
  public class IfLastUnitIs : Query
  {
    private readonly IUnitIdMatcher _unitMatcher;

    /// <param name="unitMatcher">Object contains the logic of matching with building unit</param>
    /// <param name="weight">The weight of matching</param>
    [DebuggerStepThrough]
    public IfLastUnitIs(IUnitIdMatcher unitMatcher, int weight = QueryWeight.Any) : base(weight)
      => _unitMatcher = unitMatcher ?? throw new ArgumentNullException(nameof(unitMatcher));

    public override ICollection<IQuery> Children => throw new NotSupportedException("LastUnitSequenceMatcher can't contain children");

    /// <summary>
    ///   If <paramref name="unitSequence" /> contains more then one element return null. This matcher matches only unit under construction which is
    ///   the last one in the <paramref name="unitSequence" />.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public override BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      if(unitSequence.Length > 1) return null;

      var unitInfo = unitSequence.Last();
      var matches  = _unitMatcher.Matches(unitInfo);

      if(!matches)
      {
        Log.WriteLine(LogLevel.Trace, () => string.Format("{0}{{not matched}}", this));

        return null;
      }

      using(Log.Block(LogLevel.Verbose, this.ToString)) // pass method group, do not call ToString
      {
        var buildActions = GetOwnActions(Weight + inputWeight);
        buildActions.ToLog();

        return buildActions;
      }
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>.{2}", GetType().GetShortName(), Weight, _unitMatcher);

    #region Equality

    private bool Equals(IfLastUnitIs? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Weight == other.Weight && _unitMatcher.Equals(other._unitMatcher);
    }

    public override bool Equals(IQuery obj) => Equals(obj as IfLastUnitIs);

    public override bool Equals(object obj) => Equals(obj as IfLastUnitIs);

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
