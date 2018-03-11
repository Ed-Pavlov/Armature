using System;
using System.Diagnostics;
using Armature.Framework;

namespace Armature.Core
{
  /// <summary>
  ///   Matches a unit representing parameter
  /// </summary>
  public class ParameterMatcher : IUnitMatcher
  {
    public static readonly ParameterMatcher Instance = new ParameterMatcher();

    private ParameterMatcher() { }

    public bool Matches(UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      return unitInfo.Token == SpecialToken.ParameterValue;
    }

    public bool Equals(IUnitMatcher other) => other is ParameterMatcher;

    [DebuggerStepThrough]
    public override string ToString() => typeof(ParameterMatcher).Name;
  }
}