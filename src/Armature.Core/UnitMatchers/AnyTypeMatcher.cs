using System.Diagnostics;
using Armature.Logging;

namespace Armature.Core.UnitMatchers
{
  public class AnyTypeMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyTypeMatcher();
    
    private AnyTypeMatcher()
    {
    }

    public bool Matches(UnitInfo unitInfo) 
    {
      var type = unitInfo.GetUnitTypeSafe();
      return !unitInfo.Token.IsSpecial() && type != null && !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
    }

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as AnyTypeMatcher );

    [DebuggerStepThrough]
    public override int GetHashCode() => 0;
    #endregion
  }
}