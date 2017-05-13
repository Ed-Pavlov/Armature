namespace Armature.Core
{
  /// <summary>
  /// Delegate matches an UnitInfo with the pattern, the order of parameters is important (sic!)
  /// </summary>
  public delegate bool UnitInfoPatternMatcher(UnitInfo pattern, UnitInfo other);

  public class UnitInfoMatcher
  {
    private readonly UnitInfo _pattern;
    private readonly UnitInfoPatternMatcher _matcher;

    public UnitInfoMatcher(UnitInfo pattern, UnitInfoPatternMatcher matcher, int weight)
    {
      _pattern = pattern;
      _matcher = matcher;
      Weight = weight;
    }

    public readonly int Weight;

    public bool Matches(UnitInfo other)
    {
      return _matcher(_pattern, other);
    }

    public bool Euqals(UnitInfoMatcher other)
    {
      return Equals(_pattern, other._pattern);
    }
  }
}