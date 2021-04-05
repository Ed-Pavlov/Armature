using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches any type which can be instantiated
  /// </summary>
  public class AnyTypeMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyTypeMatcher();

    private AnyTypeMatcher() { }

    public bool Matches(UnitInfo unitInfo)
    {
      var type = unitInfo.GetUnitTypeSafe();

      return !unitInfo.Token.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

#region Equality

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher? other) => ReferenceEquals(this, other);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as AnyTypeMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => 0;

#endregion
  }
}
