using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyParameterMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyParameterMatcher();

    private AnyParameterMatcher()
    {}

    public bool Matches(UnitInfo unitInfo)
    {
      return unitInfo.Token == SpecialToken.ParameterValue && unitInfo.Id is ParameterInfo;
    }

    public bool Equals(IUnitMatcher other)
    {
      return ReferenceEquals(this, other);
    }
  }
}