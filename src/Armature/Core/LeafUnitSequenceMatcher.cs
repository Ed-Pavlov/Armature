﻿using System;
using System.Diagnostics.CodeAnalysis;
using Armature.Common;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <inheritdoc />
  /// <summary>
  /// Applies passed <see cref="IUnitMatcher"/> to the unit under construction. 
  /// <see cref="LeafUnitSequenceMatcher(Armature.Core.IUnitMatcher,int)"/> and <see cref="GetBuildActions"/> for details  
  /// </summary>
  public class LeafUnitSequenceMatcher : UnitSequenceMatcherBase
  {
    private readonly int _matchingWeight;
    private readonly IUnitMatcher _unitMatcher;

    /// <param name="unitMatcher">Object contains the logic of matching with building unit</param>
    /// <param name="matchingWeight">The weight of matching</param>
    public LeafUnitSequenceMatcher([NotNull] IUnitMatcher unitMatcher, int matchingWeight)
    {
      if (unitMatcher == null) throw new ArgumentNullException("unitMatcher");
      
      _matchingWeight = matchingWeight;
      _unitMatcher = unitMatcher;
    }

    /// <summary>
    /// If <paramref name="buildingUnitsSequence"/> contains more then one element return null. This matcher matches only unit under construction which is 
    /// the last one in the <see cref="buildingUnitsSequence"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputMatchingWeight)
    {
      if (buildingUnitsSequence.Length != 1) return null;
      
      var unitInfo = buildingUnitsSequence.GetLastItem();
      var matches = _unitMatcher.Matches(unitInfo);

      if (!matches)
      {
        Log.Trace("{0}: not matched", this);
        return null;
      }
      
      using (Log.Block(this.ToString(), LogLevel.Verbose))
      {
        var buildActions = GetOwnActions(unitInfo, _matchingWeight + inputMatchingWeight);
        buildActions.ToLog();
        return buildActions;
      }
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}", GetType().Name, _unitMatcher);
    }
    
    #region Equality

    private bool Equals(LeafUnitSequenceMatcher other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return _matchingWeight == other._matchingWeight && Equals(_unitMatcher, other._unitMatcher);
    }
    
    public override bool Equals(IUnitSequenceMatcher obj)
    {
      return Equals(obj as LeafUnitSequenceMatcher);
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as LeafUnitSequenceMatcher);
    }
    
    public override int GetHashCode()
    {
      unchecked
      {
        return (_matchingWeight * 397) ^ (_unitMatcher != null ? _unitMatcher.GetHashCode() : 0);
      }
    }

    public static bool operator ==(LeafUnitSequenceMatcher left, LeafUnitSequenceMatcher right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(LeafUnitSequenceMatcher left, LeafUnitSequenceMatcher right)
    {
      return !Equals(left, right);
    }

    #endregion
  }
}