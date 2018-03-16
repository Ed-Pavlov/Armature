using System.Diagnostics;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public class ConstructorMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Constructor && unitInfo.GetUnitTypeSafe() != null;

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}