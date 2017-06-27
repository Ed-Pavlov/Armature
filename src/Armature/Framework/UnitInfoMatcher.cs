using System;
using Armature.Core;

namespace Armature.Framework
{
  /// <summary>
  /// Delegate matches an UnitInfo with the pattern, the order of parameters is important (sic!)
  /// </summary>
  public delegate bool UnitInfoPatternMatcher(UnitInfo pattern, UnitInfo other);

  /// <summary>
  /// This class is used with <see cref="UnitSequenceWeakMatchingBuildStep"/> which implements a logic of passing throug the collection
  /// of <see cref="UnitInfo"/> but can match with different <see cref="UnitInfo"/>
  /// </summary>
  public class UnitInfoMatcher : IEquatable<UnitInfoMatcher>
  {
    private readonly UnitInfo _pattern;
    private readonly UnitInfoPatternMatcher _matcher;

    public UnitInfoMatcher(UnitInfo pattern, UnitInfoPatternMatcher matcher, int matchingWeight)
    {
      _pattern = pattern;
      _matcher = matcher;
      MatchingWeight = matchingWeight;
    }

    public readonly int MatchingWeight;

    public bool Matches(UnitInfo other)
    {
      return _matcher(_pattern, other);
    }

    public bool Equals(UnitInfoMatcher other)
    {
      return other != null && Equals(_pattern, other._pattern);
    }
  }
}