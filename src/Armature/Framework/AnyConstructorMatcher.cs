using Armature.Core;

namespace Armature.Framework
{
  public class AnyConstructorMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyConstructorMatcher();

    private AnyConstructorMatcher()
    {
    }

    public bool Matches(UnitInfo unitInfo)
    {
      return unitInfo.Token == SpecialToken.Constructor && unitInfo.GetUnitTypeSafe() != null;
    }

    public bool Equals(IUnitMatcher other)
    {
      return ReferenceEquals(this, other);
    }
  }
}