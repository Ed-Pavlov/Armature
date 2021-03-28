using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches Unit representing "property" of the currently building Unit
  /// </summary>
  public sealed record PropertyMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new PropertyMatcher();

    private PropertyMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Property && unitInfo.GetUnitTypeSafe() is not null;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}