using System.Diagnostics;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyConstructorMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyConstructorMatcher();

    private AnyConstructorMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Constructor && unitInfo.GetUnitTypeSafe() != null;

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);
  }
}