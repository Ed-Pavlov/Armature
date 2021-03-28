using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches Unit representing "constructor" of the currently building Unit
  /// </summary>
  public  sealed record ConstructorMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Constructor && unitInfo.GetUnitTypeSafe() is not null;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}